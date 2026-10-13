import { Component } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { Subject, takeUntil } from 'rxjs';
import { AuthenticationService } from 'src/app/core/services/auth.service';
import { login } from 'src/app/store/Authentication/authentication.actions';
import { getError } from 'src/app/store/Authentication/authentication-selector';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})

// Login Component
export class LoginComponent {

  // Login Form
  loginForm!: UntypedFormGroup;
  submitted = false;
  fieldTextType!: boolean;
  error = '';
  returnUrl!: string;
  a: any = 10;
  b: any = 20;
  toast!: false;
  private readonly destroy$ = new Subject<void>();

  // set the current year
  year: number = new Date().getFullYear();

  // tslint:disable-next-line: max-line-length
  constructor(private formBuilder: UntypedFormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private store: Store,
    private authenticationService: AuthenticationService,
) { }

  ngOnInit(): void {
    const currentUser = this.authenticationService.currentUser();
    if (currentUser && this.isAdmin(currentUser)) {
      this.router.navigate(['/']);
      return;
    }

    if (currentUser) {
      this.authenticationService.clearLocalSession();
    }

    this.route.queryParamMap.pipe(takeUntil(this.destroy$)).subscribe(params => {
      if (params.get('forbidden') === 'true') {
        this.error = 'Tai khoan nay khong co quyen truy cap trang quan tri.';
      }
    });

    this.store.select(getError).pipe(takeUntil(this.destroy$)).subscribe(error => {
      if (error && this.submitted) {
        this.error = error;
      }
    });

    /**
     * Form Validatyion
     */
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]],
      rememberMe: [false],
    });
  }

  // convenience getter for easy access to form fields
  get f() { return this.loginForm.controls; }

  /**
   * Form submit
   */
  onSubmit() {
    this.submitted = true;
    this.error = '';
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    const email = this.f['email'].value; // Get the username from the form
    const password = this.f['password'].value; // Get the password from the form
    const rememberMe = Boolean(this.f['rememberMe'].value);

    // Login Api
    this.store.dispatch(login({ email: email, password: password, rememberMe: rememberMe }));
  }

  /**
   * Password Hide/Show
   */
  toggleFieldTextType() {
    this.fieldTextType = !this.fieldTextType;
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private isAdmin(user: any): boolean {
    const roleId = Number(user?.roleId);
    const roleName = `${user?.roleName ?? user?.role ?? ''}`.toLowerCase();
    return roleId === 0 || roleName === 'admin';
  }
}
