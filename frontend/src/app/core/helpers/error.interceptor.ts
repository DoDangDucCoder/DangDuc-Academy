import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

    constructor() { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(catchError(err => {
            // 401 is handled by AuthInterceptor (token refresh).
            // Only handle non-auth errors here.
            if (err.status === 403) {
                console.error('Access forbidden:', err.url);
            }
            const error = err.error?.systemMessage || err.error?.message || err.statusText;
            return throwError(() => error);
        }));
    }
}
