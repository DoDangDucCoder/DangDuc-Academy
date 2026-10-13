import { Component } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { Subject, takeUntil } from 'rxjs';

// Register Auth
import { Register } from 'src/app/store/Authentication/authentication.actions';
import { getError } from 'src/app/store/Authentication/authentication-selector';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})

// Register Component
export class RegisterComponent {
  // Login Form
  signupForm!: UntypedFormGroup;
  submitted = false;
  successmsg = false;
  error = '';
  private readonly destroy$ = new Subject<void>();
  // set the current year
  year: number = new Date().getFullYear();

  fieldTextType!: boolean;

  constructor(private formBuilder: UntypedFormBuilder,  public store: Store) { }

  ngOnInit(): void {
    this.store.select(getError).pipe(takeUntil(this.destroy$)).subscribe(error => {
      if (error && this.submitted) {
        this.error = error;
      }
    });

    /**
     * Form Validatyion
     */
    this.signupForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      name: ['', [Validators.required]],
      password: ['', [
        Validators.required,
        Validators.minLength(8),
        Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$/)
      ]],
    });
  }

  // convenience getter for easy access to form fields
  get f() { return this.signupForm.controls; }

  /**
   * Register submit form
   */
  onSubmit() {
    this.submitted = true;
    this.error = '';

    if (this.signupForm.invalid) {
      this.signupForm.markAllAsTouched();
      return;
    }

    const email = this.f['email'].value;
    const name = this.f['name'].value;
    const password = this.f['password'].value;

    //Dispatch Action
    this.store.dispatch(Register({ email: email, first_name: name, password: password }));
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

}
