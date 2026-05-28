import { FacebookLoginProvider, SocialAuthService, SocialUser } from '@abacritt/angularx-social-login';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { finalize } from 'rxjs/operators';
import { AuthenticationServices } from 'src/app/core/services/authentication.service';
import { MessengerServices } from 'src/app/core/services/messenger.service';
import { LocalStorageConfig } from 'src/app/library/clientconfig/localstorageconfig';
import { RepositoryModel } from 'src/app/models/models/repository_base';
import { LoginRegister } from 'src/app/models/respone_model/login-register-respone';

@Component({
  selector: 'app-login-pages',
  templateUrl: './login-pages.component.html',
  styleUrls: ['./login-pages.component.scss']
})
export class LoginPagesComponent implements OnInit {

  loginForm!: UntypedFormGroup;
  isLoading = false;
  private returnUrl = '/';
  constructor(
    private readonly authenticationService: AuthenticationServices,
    private readonly externalAuthService: SocialAuthService,
    private readonly router: Router,
    private readonly route: ActivatedRoute,
    private readonly messengerServices: MessengerServices,
    private formBuilder: FormBuilder,
  ) {

    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
    });
  }

  ngOnInit() {
    this.returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/';
    this.externalAuthService.authState.subscribe((user: SocialUser) => {
      if (user.id && user.idToken) {
        this.isLoading = true;
        this.loginWithGoogle(user);
      }
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

  loginWithEmail() {
    this.isLoading = true;
    this.authenticationService.loginNormalAccount(this.loginForm.value.email, this.loginForm.value.password)
      .pipe(finalize(() => this.isLoading = false))
      .subscribe({
        next: (res) => this.handleLoginResponse(res),
        error: () => this.messengerServices.errorWithIssue(),
      });
  }

  private handleLoginResponse(res: RepositoryModel<LoginRegister>) {
    if (res.retCode === 0 && res.systemMessage === '' && res.data) {
      LocalStorageConfig.SetUser(res.data);
      this.authenticationService.updateAfterLogin(res.data);
      this.router.navigateByUrl(this.returnUrl);
      return;
    }

    this.messengerServices.errorWithIssue();
  }
}
