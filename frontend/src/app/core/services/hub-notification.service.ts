import { Injectable } from "@angular/core";
import { HubConnection, HubConnectionBuilder, HubConnectionState, LogLevel } from "@microsoft/signalr";
import { environment } from "src/environments/environment";

@Injectable({
    providedIn: 'root'
})

export class NotificationHubService {

    private hubConnection: HubConnection | undefined;
    private isStarting = false;
    public notificationItems: Notification[] = [];
    public get count(): number {
        return this.notificationItems.length;
    };
    constructor() { }

    public startConnection(): Promise<void> {
        if (this.hubConnection) {
            const state = this.hubConnection.state;
            if (
                state === HubConnectionState.Connected ||
                state === HubConnectionState.Connecting ||
                state === HubConnectionState.Reconnecting ||
                this.isStarting
            ) {
                return Promise.resolve();
            }
        }

        this.isStarting = true;
        const baseUrl = environment.baseUrlHub;
        this.hubConnection = new HubConnectionBuilder()
            .withUrl(`${baseUrl}hub-notification`).withAutomaticReconnect().configureLogging(LogLevel.Information)
            .build();

        return this.hubConnection
            .start()
            .then(() => undefined)
            .catch(err => {
                this.hubConnection = undefined;
                console.error('Error while starting notification hub connection', err);
                return undefined;
            })
            .finally(() => {
                this.isStarting = false;
            });
    }

}
