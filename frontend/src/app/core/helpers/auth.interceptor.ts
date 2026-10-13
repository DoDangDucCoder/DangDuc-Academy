import { HTTP_INTERCEPTORS, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';

import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, filter, switchMap, take } from 'rxjs/operators';
import { LocalStorageConfig } from 'src/app/library/clientconfig/localstorageconfig';
import { AuthenticationServices } from '../services/authentication.service';

const TOKEN_HEADER_KEY = 'Authorization';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<string | null> = new BehaviorSubject<string | null>(null);

  constructor(private authService: AuthenticationServices) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    let authReq = req;
    const token = LocalStorageConfig.GetUser()?.token;
    if (token != null) {
      authReq = this.addTokenHeader(req, token);
    }
    return next.handle(authReq).pipe(
      catchError((error) => {
        if (error instanceof HttpErrorResponse && error.status === 401 && !req.url.includes('refresh-token') && !req.url.includes('revoke-token')) {
          return this.handle401Error(authReq, next);
        }
        return throwError(() => error);
      })
    );
  }

  private handle401Error(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      const currentUser = LocalStorageConfig.GetUser();
      const refreshToken = currentUser?.refreshToken;

      if (refreshToken) {
        return this.authService.refreshToken(refreshToken).pipe(
          switchMap((response) => {
            this.isRefreshing = false;
            if (response?.data) {
              LocalStorageConfig.SetUser(response.data);
              this.authService.updateAfterLogin(response.data);
              this.refreshTokenSubject.next(response.data.token);
              return next.handle(this.addTokenHeader(request, response.data.token));
            }
            this.authService.logoutWithAccount();
            return throwError(() => new Error('Token refresh failed'));
          }),
          catchError((err) => {
            this.isRefreshing = false;
            this.authService.logoutWithAccount();
            return throwError(() => err);
          })
        );
      }
    }

    return this.refreshTokenSubject.pipe(
      filter((token) => token !== null),
      take(1),
      switchMap((token) => next.handle(this.addTokenHeader(request, token!)))
    );
  }

  private addTokenHeader(request: HttpRequest<any>, token: string): HttpRequest<any> {
    return request.clone({ headers: request.headers.set(TOKEN_HEADER_KEY, `Bearer ${token}`) });
  }
}

export const authInterceptorProviders = [
  { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
];
