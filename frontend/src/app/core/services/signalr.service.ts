import { Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import { LocalStorageConfig } from 'src/app/library/clientconfig/localstorageconfig';

export interface NotificationItem {
    id: string;
    message: string;
    timestamp: Date;
    isRead: boolean;
    type: string;
}

@Injectable({ providedIn: 'root' })
export class SignalRService implements OnDestroy {
    private hubConnection: signalR.HubConnection | null = null;
    private notificationsSubject = new BehaviorSubject<NotificationItem[]>([]);
    private unreadCountSubject = new BehaviorSubject<number>(0);

    notifications$: Observable<NotificationItem[]> = this.notificationsSubject.asObservable();
    unreadCount$: Observable<number> = this.unreadCountSubject.asObservable();

    startConnection(): void {
        const user = LocalStorageConfig.GetUser();
        if (!user?.token) return;

        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl(environment.baseUrlHub + 'hubConfigProject', {
                accessTokenFactory: () => user.token || ''
            })
            .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
            .configureLogging(signalR.LogLevel.Warning)
            .build();

        this.hubConnection.on('ReceiveNotification', (message: string) => {
            const notification: NotificationItem = {
                id: Date.now().toString(),
                message,
                timestamp: new Date(),
                isRead: false,
                type: 'general'
            };
            const current = this.notificationsSubject.value;
            this.notificationsSubject.next([notification, ...current]);
            this.updateUnreadCount();
        });

        this.hubConnection.on('CourseUpdate', (courseId: number, message: string) => {
            const notification: NotificationItem = {
                id: Date.now().toString(),
                message: `Course #${courseId}: ${message}`,
                timestamp: new Date(),
                isRead: false,
                type: 'course'
            };
            const current = this.notificationsSubject.value;
            this.notificationsSubject.next([notification, ...current]);
            this.updateUnreadCount();
        });

        this.hubConnection.onreconnecting(() => {
            console.log('[SignalR] Reconnecting...');
        });

        this.hubConnection.onreconnected(() => {
            console.log('[SignalR] Reconnected');
        });

        this.hubConnection
            .start()
            .then(() => console.log('[SignalR] Connected'))
            .catch(err => console.warn('[SignalR] Connection failed:', err));
    }

    stopConnection(): void {
        if (this.hubConnection) {
            this.hubConnection.stop();
            this.hubConnection = null;
        }
    }

    markAsRead(id: string): void {
        const notifications = this.notificationsSubject.value.map(n =>
            n.id === id ? { ...n, isRead: true } : n
        );
        this.notificationsSubject.next(notifications);
        this.updateUnreadCount();
    }

    markAllAsRead(): void {
        const notifications = this.notificationsSubject.value.map(n => ({ ...n, isRead: true }));
        this.notificationsSubject.next(notifications);
        this.updateUnreadCount();
    }

    clearAll(): void {
        this.notificationsSubject.next([]);
        this.unreadCountSubject.next(0);
    }

    private updateUnreadCount(): void {
        const count = this.notificationsSubject.value.filter(n => !n.isRead).length;
        this.unreadCountSubject.next(count);
    }

    ngOnDestroy(): void {
        this.stopConnection();
    }
}
