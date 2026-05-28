import { Injectable } from '@angular/core';
import { Store } from '@ngrx/store';

import { User } from '../../store/Authentication/auth.models';
import { getFirebaseBackend } from 'src/app/authUtils';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { catchError, map, tap } from 'rxjs/operators';
import { BehaviorSubject, Observable, of, throwError } from 'rxjs';
import { environment } from 'src/environments/environment';
import { LinkSettings } from 'src/app/library/linksetting/LinkSetting';
import { RepositoryModel } from 'src/app/models/models/repository_base';
import { LoginRegister } from 'src/app/models/respone_model/login-register-respone';

// Action
import { logout, logoutSuccess } from '../../store/Authentication/authentication.actions';

// Firebase
import { AngularFireAuth } from '@angular/fire/compat/auth';
import firebase from 'firebase/compat/app';

const httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
};

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
    private readonly defaultPhone = '0000000000';
    private currentUserSubject: BehaviorSubject<LoginRegister | User | null>;

    constructor(private http: HttpClient, private store: Store, private afAuth: AngularFireAuth) {
        this.currentUserSubject = new BehaviorSubject<LoginRegister | User | null>(this.getStoredCurrentUser());
    }

    // Sign in with Google provider
    signInWithGoogle(): Promise<User> {
        const provider = new firebase.auth.GoogleAuthProvider();
        return this.signInWithPopup(provider);
    }

    // Sign in with Facebook provider
    signInWithFacebook(): Promise<User> {
        const provider = new firebase.auth.FacebookAuthProvider();
        return this.signInWithPopup(provider);
    }

    // Sign in with a popup for the specified provider
    private async signInWithPopup(provider: firebase.auth.AuthProvider): Promise<User> {
        try {
            const result = await this.afAuth.signInWithPopup(provider);
            const user = result.user;
            return {
                //     uid: user.uid,
                //     displayName: user.displayName,
                //     email: user.email,
                //     // Add other user properties as needed
            };
        } catch (error) {
            throw new Error('Failed to sign in with the specified provider.');
        }
    }

    // Sign out the current user
    signOut(): Promise<void> {
        return this.afAuth.signOut();
    }


    register(email: string, first_name: string, password: string) {
        const apiUrl = environment.baseUrl + LinkSettings.GetResLinkSetting(
            'Authentication',
            'RegisterUser',
        );

        return this.http.post<RepositoryModel<boolean>>(apiUrl, {
            name: first_name,
            phone: this.defaultPhone,
            email,
            password,
        }, httpOptions).pipe(
            catchError((error: any) => throwError(() => this.getErrorMessage(error, 'Register failed')))
        );
    }

    login(email: string, password: string) {
        const apiUrl = environment.baseUrl + LinkSettings.GetResLinkSetting(
            'Authentication',
            'LoginNormalAccount',
        );

        return this.http.post<RepositoryModel<LoginRegister>>(apiUrl, { email, password }, httpOptions).pipe(
            map((response) => {
                if (response.retCode === 0 && response.systemMessage === '' && response.data) {
                    sessionStorage.setItem('currentUser', JSON.stringify(response.data));
                    localStorage.removeItem('currentUser');
                    if (response.data.token) {
                        sessionStorage.setItem('auth-token', response.data.token);
                        localStorage.removeItem('token');
                    } else {
                        sessionStorage.removeItem('auth-token');
                        localStorage.removeItem('token');
                    }
                    this.currentUserSubject.next(response.data);
                }

                return response;
            }),
            catchError((error: any) => throwError(() => this.getErrorMessage(error, 'Login failed')))
        );
    }

    logout(): Observable<void> {
        this.store.dispatch(logout());
        // Perform any additional logout logic, e.g., calling an API to invalidate the token

        localStorage.removeItem('currentUser');
        localStorage.removeItem('token');
        sessionStorage.removeItem('currentUser');
        sessionStorage.removeItem('auth-token');
        this.currentUserSubject.next(null);
        this.store.dispatch(logoutSuccess());

        // Return an Observable<void> indicating the successful logout
        return of(undefined).pipe(
            tap(() => {
                // Perform any additional logic after the logout is successful
            })
        );
    }

    resetPassword(email: string) {
        return this.http.post(`${environment.baseUrl}auth/reset-password`, { email }, httpOptions);
    }

    /**
 * Returns the current user
 */
    public currentUser(): any {
        if (environment.defaultauth === 'firebase') {
            return getFirebaseBackend()?.getAuthenticatedUser();
        }

        return this.currentUserSubject.value ?? this.getStoredCurrentUser();
    }

    private getStoredCurrentUser(): LoginRegister | User | null {
        const currentUser = sessionStorage.getItem('currentUser') ?? localStorage.getItem('currentUser');

        if (!currentUser) {
            return null;
        }

        try {
            return JSON.parse(currentUser) as LoginRegister;
        } catch {
            localStorage.removeItem('currentUser');
            sessionStorage.removeItem('currentUser');
            return null;
        }
    }

    private getErrorMessage(error: any, fallbackMessage: string): string {
        return error?.error?.systemMessage || error?.error?.message || error?.message || fallbackMessage;
    }
}
