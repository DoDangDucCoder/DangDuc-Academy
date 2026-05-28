import { FacebookLoginProvider, SocialAuthService, SocialUser } from '@abacritt/angularx-social-login';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { AuthenticationServices } from 'src/app/core/services/authentication.service';
import { MessengerServices } from 'src/app/core/services/messenger.service';
import { LocalStorageConfig } from 'src/app/library/clientconfig/localstorageconfig';
import { RepositoryModel } from 'src/app/models/models/repository_base';
import { LoginRegister } from 'src/app/models/respone_model/login-register-respone';

@Component({
  selector: 'app-register-pages',
  templateUrl: './register-pages.component.html',
  styleUrls: ['./register-pages.component.scss']
})
export class RegisterPagesComponent implements OnInit {

  registerForm!: UntypedFormGroup;
  isLoading = false;
  constructor(
    private formBuilder: FormBuilder,
    private readonly authenticationService: AuthenticationServices,
    private readonly messengerServices: MessengerServices,
    private readonly externalAuthService: SocialAuthService,
    private readonly router: Router,
  ) { }

  ngOnInit() {
    this.registerForm = this.formBuilder.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
      phone: ['', Validators.required],
    });

    this.externalAuthService.authState.subscribe((user: SocialUser) => {
      if (user.id && user.idToken) {
        this.isLoading = true;
        this.loginWithGoogle(user);
      }
    });
  }

  registerUser() {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      this.messengerServices.warringWithIssue();
      return;
    }

    const { email, password, name, phone } = this.registerForm.getRawValue();
    this.isLoading = true;
    this.authenticationService.registerUser(name, phone, email, password)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (res) => {
          if (res.retCode === 0 && res.systemMessage === '') {
            this.registerForm.reset();
            this.messengerServices.successes('Đăng ký tài khoản thành công');
            return;
          }

          this.messengerServices.errorWithIssue();
        },
        error: () => this.messengerServices.errorWithIssue(),
      });
  }

  loginWithGoogle(model: SocialUser) {
    this.authenticationService.loginWithGoogle(model)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (res) => this.handleLoginResponse(res),
        error: () => this.messengerServices.errorWithIssue(),
      });
  }

  loginWithFaceBook() {
    this.isLoading = true;
    this.externalAuthService.signIn(FacebookLoginProvider.PROVIDER_ID).then((res: SocialUser) => {
      if (res.authToken && res.id) {
        this.authenticationService.loginWithFaceBook(res)
          .pipe(finalize(() => this.isLoading = false))
          .subscribe({
            next: (response) => this.handleLoginResponse(response),
            error: () => this.messengerServices.errorWithIssue(),
          });
      } else {
        this.isLoading = false;
      }
    }).catch(() => {
      this.isLoading = false;
      this.messengerServices.errorWithIssue();
    });
  }

  private handleLoginResponse(res: RepositoryModel<LoginRegister>) {
    if (res.retCode === 0 && res.systemMessage === '' && res.data) {
      LocalStorageConfig.SetUser(res.data);
      this.authenticationService.updateAfterLogin(res.data);
      this.router.navigate([`/`]);
      return;
    }

    this.messengerServices.errorWithIssue();
  }

}
