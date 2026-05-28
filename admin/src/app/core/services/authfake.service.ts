import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { User } from '../../store/Authentication/auth.models';

@Injectable({ providedIn: 'root' })
export class AuthfakeauthenticationService {

    private currentUserSubject: BehaviorSubject<User | null>;
    public currentUser: Observable<User | null>;

    constructor(private http: HttpClient) {
        this.currentUserSubject = new BehaviorSubject<User | null>(this.getStoredCurrentUser());
        this.currentUser = this.currentUserSubject.asObservable();
    }

    /**
     * current user
     */
    public get currentUserValue(): User | null {
        return this.currentUserSubject.value;
    }

    /**
     * Performs the auth
     * @param email email of user
     * @param password password of user
     */
    login(email: string, password: string) {
        return this.http.post<any>(`/users/authenticate`, { email, password }).pipe(map(user => {
            // login successful if there's a jwt token in the response
            if (user && user.token) {
                // store user details and jwt token in local storage to keep user logged in between page refreshes
                localStorage.setItem('toast', 'true');
                sessionStorage.setItem('currentUser', JSON.stringify(user));
                localStorage.removeItem('currentUser');
                this.currentUserSubject.next(user);
            }
            return user;
        }));
    }

    /**
     * Logout the user
     */
    logout() {
        // remove user from local storage to log user out
        localStorage.removeItem('currentUser');
        sessionStorage.removeItem('currentUser');
        this.currentUserSubject.next(null);
    }

    private getStoredCurrentUser(): User | null {
        const currentUser = sessionStorage.getItem('currentUser') ?? localStorage.getItem('currentUser');

        if (!currentUser) {
            return null;
        }

        try {
            return JSON.parse(currentUser) as User;
        } catch {
            localStorage.removeItem('currentUser');
            return null;
        }
    }
}
