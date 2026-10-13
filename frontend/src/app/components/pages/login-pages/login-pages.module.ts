import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonComponentsModule } from '../../common/common-components.module';
import { LoginPagesComponent } from './login-pages.component';
import { SocialLoginModule, GoogleSigninButtonModule } from '@abacritt/angularx-social-login';

@NgModule({
  declarations: [LoginPagesComponent],
  imports: [
    CommonComponentsModule,
    SocialLoginModule,
    GoogleSigninButtonModule,
    RouterModule.forChild([
      { path: '', component: LoginPagesComponent }
    ])
  ]
})
export class LoginPagesModule {}
