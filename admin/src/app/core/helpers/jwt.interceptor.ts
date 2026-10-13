import { Injectable } from '@angular/core';
import {
    HttpRequest,
    HttpHandler,
    HttpEvent,
    HttpInterceptor,
    HttpErrorResponse,
} from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, filter, switchMap, take } from 'rxjs/operators';

import { AuthenticationService } from '../services/auth.service';
import { AuthfakeauthenticationService } from '../services/authfake.service';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { LinkSettings } from 'src/app/library/linksetting/LinkSetting';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
    private isRefreshing = false;
    private refreshTokenSubject = new BehaviorSubject<string | null>(null);

    constructor(
        private authenticationService: AuthenticationService,
        private authfackservice: AuthfakeauthenticationService,
        private http: HttpClient
    ) { }

    intercept(
        request: HttpRequest<any>,
        next: HttpHandler
    ): Observable<HttpEvent<any>> {
        const currentUser = environment.defaultauth === 'fakebackend'
            ? this.authfackservice.currentUserValue
            : this.authenticationService.currentUser();

        if (currentUser && currentUser.token) {
            request = this.addTokenHeader(request, currentUser.token);
        }

        return next.handle(request).pipe(
            catchError((error) => {
                if (
                    error instanceof HttpErrorResponse &&
                    error.status === 401 &&
                    !request.url.includes('refresh-token') &&
                    !request.url.includes('revoke-token')
                ) {
                    return this.handle401Error(request, next);
                }
                return throwError(() => error);
            })
        );
    }

    private handle401Error(
        request: HttpRequest<any>,
        next: HttpHandler
    ): Observable<HttpEvent<any>> {
        if (!this.isRefreshing) {
            this.isRefreshing = true;
            this.refreshTokenSubject.next(null);

            const currentUser = this.authenticationService.currentUser();
            const refreshToken = currentUser?.refreshToken;

            if (refreshToken) {
                const apiUrl =
                    environment.baseUrl +
                    LinkSettings.GetResLinkSetting('Authentication', 'RefreshToken');

                return this.http
                    .post<any>(apiUrl, { refreshToken }, {
                        headers: new HttpHeaders({ 'Content-Type': 'application/json' }),
                    })
                    .pipe(
                        switchMap((response) => {
                            this.isRefreshing = false;
                            if (this.isSuccessResponse(response) && response?.data) {
                                const rememberMe = localStorage.getItem('currentUser') !== null;
                                const targetStorage = rememberMe ? localStorage : sessionStorage;
                                const otherStorage = rememberMe ? sessionStorage : localStorage;

                                targetStorage.setItem('currentUser', JSON.stringify(response.data));
                                otherStorage.removeItem('currentUser');

                                if (response.data.token) {
                                    targetStorage.setItem('auth-token', response.data.token);
                                    otherStorage.removeItem('auth-token');
                                    if (rememberMe) {
                                        localStorage.setItem('token', response.data.token);
                                    } else {
                                        localStorage.removeItem('token');
                                    }
                                }
                                this.refreshTokenSubject.next(response.data.token);
                                return next.handle(
                                    this.addTokenHeader(request, response.data.token)
                                );
                            }
                            this.authenticationService.logout();
                            return throwError(() => new Error('Token refresh failed'));
                        }),
                        catchError((err) => {
                            this.isRefreshing = false;
                            this.authenticationService.logout();
                            return throwError(() => err);
                        })
                    );
            }
        }

        return this.refreshTokenSubject.pipe(
            filter((token) => token !== null),
            take(1),
            switchMap((token) =>
                next.handle(this.addTokenHeader(request, token!))
            )
        );
    }

    private addTokenHeader(
        request: HttpRequest<any>,
        token: string
    ): HttpRequest<any> {
        return request.clone({
            setHeaders: { Authorization: `Bearer ${token}` },
        });
    }

    private isSuccessResponse(response: any): boolean {
        const retCode = response?.retCode;
        return retCode === 0 || `${retCode}`.toLowerCase() === 'successfull';
    }
}
