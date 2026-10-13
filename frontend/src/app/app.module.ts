import { BrowserModule } from '@angular/platform-browser';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA, DEFAULT_CURRENCY_CODE } from '@angular/core';
import { CarouselModule } from 'ngx-owl-carousel-o';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CountUpModule } from 'ngx-countup';
import { NgxScrollTopModule } from 'ngx-scrolltop';
import { LightboxModule } from 'ngx-lightbox';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavbarComponent } from './components/common/navbar/navbar.component';
import { FooterComponent } from './components/common/footer/footer.component';
import { NotFoundComponent } from './components/pages/not-found/not-found.component';
import { ElearningSchoolComponent } from './components/pages/elearning-school/elearning-school.component';
import { UserCommentCoursesComponent } from './components/dialog/user-comment-courses/user-comment-courses.component';
import { UserReviewCoursesComponent } from './components/dialog/user-review-courses/user-review-courses.component';

import { NgxSimpleCountdownModule } from 'ngx-simple-countdown';
import { NgbAccordionModule, NgbModalModule, NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import { NgxStarsModule } from 'ngx-stars';
import { TransferHttp } from './core/transfer-http/transfer-http';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { NgIconsModule } from '@ng-icons/core';
import { bootstrapBookmark, bootstrapBookmarkCheckFill } from '@ng-icons/bootstrap-icons';
import { MentionModule } from 'angular-mentions';
import { FacebookLoginProvider, GoogleLoginProvider, GoogleSigninButtonModule, SocialAuthServiceConfig, SocialLoginModule } from '@abacritt/angularx-social-login';
import { environment } from 'src/environments/environment';
import { CommonComponentsModule } from './components/common/common-components.module';
import { AuthInterceptor } from './core/helpers/auth.interceptor';

@NgModule({
  declarations: [
    // ── Core shell (always loaded) ───────────────────────────────────────────
    AppComponent,
    NavbarComponent,
    FooterComponent,
    NotFoundComponent,

    // ── Eager page: home (highest traffic) ──────────────────────────────────
    ElearningSchoolComponent,

    // ── Dialog components (used across many pages) ───────────────────────────
    UserCommentCoursesComponent,
    UserReviewCoursesComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,

    // UI libraries (needed at shell level)
    CarouselModule,
    CountUpModule,
    NgxScrollTopModule,
    LightboxModule,
    NgxSimpleCountdownModule,
    NgbModalModule,
    NgbAccordionModule,
    NgbTooltipModule,
    NgxStarsModule,
    BsDropdownModule.forRoot(),
    MentionModule,

    // Auth
    SocialLoginModule,
    GoogleSigninButtonModule,

    // Icons
    NgIconsModule.withIcons({ bootstrapBookmark, bootstrapBookmarkCheckFill }),

    // Shared module (re-exports CommonModule, RouterModule, AssetUrlPipe, etc.)
    CommonComponentsModule,
  ],
  providers: [
    TransferHttp,
    // AuthInterceptor: handles 401 → token refresh → retry automatically
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    {
      provide: 'SocialAuthServiceConfig',
      useValue: {
        autoLogin: false,
        providers: [
          {
            id: GoogleLoginProvider.PROVIDER_ID,
            provider: new GoogleLoginProvider(environment.clientIdGoogle, { oneTapEnabled: false }),
          },
          {
            id: FacebookLoginProvider.PROVIDER_ID,
            provider: new FacebookLoginProvider(environment.facebookId),
          },
        ],
        onError: () => {},
      } as SocialAuthServiceConfig,
    },
    { provide: DEFAULT_CURRENCY_CODE, useValue: 'USD' },
  ],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class AppModule {}
