import { Injectable, Inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { map, catchError, exhaustMap, tap } from 'rxjs/operators';
import { of } from 'rxjs';
import { AuthenticationService } from '../../core/services/auth.service';
import { login, loginSuccess, loginFailure, logout, logoutSuccess, Register, RegisterFailure, RegisterSuccess } from './authentication.actions';
import { Router } from '@angular/router';

@Injectable()
export class AuthenticationEffects {

  Register$ = createEffect(() =>
    this.actions$.pipe(
      ofType(Register),
      exhaustMap(({ email, first_name, password }) =>
        this.AuthenticationService.register(email, first_name, password).pipe(
          map((response) => {
            if (this.isSuccessResponse(response)) {
              this.router.navigate(['/auth/login']);
              return RegisterSuccess({ user: { email, name: first_name } });
            }

            return RegisterFailure({ error: response.systemMessage || 'Register failed' });
          }),
          catchError((error) => of(RegisterFailure({ error: this.getErrorMessage(error, 'Register failed') })))
        )
      )
    )
  );

  login$ = createEffect(() =>
    this.actions$.pipe(
      ofType(login),
      exhaustMap(({ email, password, rememberMe }) =>
        this.AuthenticationService.login(email, password, rememberMe).pipe(
          map((response) => {
            if (this.isSuccessResponse(response) && response.data) {
              if (!this.isAdmin(response.data)) {
                this.AuthenticationService.clearLocalSession();
                return loginFailure({ error: 'Tai khoan nay khong co quyen truy cap trang quan tri.' });
              }

              this.router.navigate(['/']);
              return loginSuccess({ user: response.data });
            }

            return loginFailure({ error: response.systemMessage || 'Login failed' });
          }),
          catchError((error) => of(loginFailure({ error: this.getErrorMessage(error, 'Login failed') })))
        )
      )
    )
  );

  logout$ = createEffect(() =>
    this.actions$.pipe(
      ofType(logout),
      tap(() => {
        // Perform any necessary cleanup or side effects before logging out
      }),
      exhaustMap(() => of(logoutSuccess()))
    )
  );

  constructor(
    @Inject(Actions) private actions$: Actions,
    private AuthenticationService: AuthenticationService,
    private router: Router) { }

  private getErrorMessage(error: unknown, fallbackMessage: string): string {
    if (typeof error === 'string') {
      return error;
    }

    const value = error as { error?: { systemMessage?: string; message?: string }; message?: string };
    return value?.error?.systemMessage || value?.error?.message || value?.message || fallbackMessage;
  }

  private isAdmin(user: any): boolean {
    const roleId = Number(user?.roleId);
    const roleName = `${user?.roleName ?? user?.role ?? ''}`.toLowerCase();
    return roleId === 0 || roleName === 'admin';
  }

  private isSuccessResponse(response: any): boolean {
    const retCode = response?.retCode;
    return retCode === 0 || `${retCode}`.toLowerCase() === 'successfull';
  }
}
