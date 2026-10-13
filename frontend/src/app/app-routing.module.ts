import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ElearningSchoolComponent } from './components/pages/elearning-school/elearning-school.component';
import { NotFoundComponent } from './components/pages/not-found/not-found.component';
import { AuthGuard } from './core/guards/auth.guard';

const routes: Routes = [
    // === EAGER: home page (highest traffic, must load instantly) ===
    { path: '', component: ElearningSchoolComponent },

    // === LAZY: language learning platform ===
    {
        path: 'language',
        loadChildren: () => import('./components/pages/language-hub/language-hub.module').then(m => m.LanguageHubModule)
    },
    {
        path: 'language/:lang',
        loadChildren: () => import('./components/pages/language-exam-list/language-exam-list.module').then(m => m.LanguageExamListModule)
    },
    {
        path: 'language/:lang/exam/:examId',
        loadChildren: () => import('./components/pages/language-exam-do/language-exam-do.module').then(m => m.LanguageExamDoModule)
    },
    {
        path: 'language/:lang/result/:attemptId',
        loadChildren: () => import('./components/pages/language-exam-result/language-exam-result.module').then(m => m.LanguageExamResultModule)
    },
    {
        path: 'language/:lang/flashcard',
        loadChildren: () => import('./components/pages/language-flashcard/language-flashcard.module').then(m => m.LanguageFlashcardModule)
    },
    {
        path: 'language/:lang/leaderboard',
        loadChildren: () => import('./components/pages/language-leaderboard/language-leaderboard.module').then(m => m.LanguageLeaderboardModule)
    },
    {
        path: 'language/:lang/grammar',
        loadChildren: () => import('./components/pages/language-grammar/language-grammar.module').then(m => m.LanguageGrammarModule)
    },

    // === LAZY: course browsing ===
    {
        path: 'courses',
        redirectTo: 'courses/1',
        pathMatch: 'full'
    },
    {
        path: 'courses/:id',
        loadChildren: () => import('./components/pages/courses-list/courses-list.module').then(m => m.CoursesListModule)
    },
    {
        path: 'detail-courses',
        redirectTo: 'detail-courses/1',
        pathMatch: 'full'
    },
    {
        path: 'detail-courses/:id',
        loadChildren: () => import('./components/pages/courses-details/courses-details.module').then(m => m.CoursesDetailsModule)
    },
    {
        path: 'categories',
        loadChildren: () => import('./components/pages/categories/categories.module').then(m => m.CategoriesModule)
    },

    // === LAZY: learning page (auth required) ===
    {
        path: 'detail-content-course/:id',
        canActivate: [AuthGuard],
        loadChildren: () => import('./components/pages/courses-content-detail/courses-content-detail.module').then(m => m.CoursesContentDetailModule)
    },

    // === LAZY: auth pages ===
    {
        path: 'login',
        loadChildren: () => import('./components/pages/login-pages/login-pages.module').then(m => m.LoginPagesModule)
    },
    {
        path: 'register',
        loadChildren: () => import('./components/pages/register-pages/register-pages.module').then(m => m.RegisterPagesModule)
    },
    {
        path: 'register-error',
        loadChildren: () => import('./components/pages/register-error/register-error.module').then(m => m.RegisterErrorModule)
    },
    {
        path: 'send-email-register-success',
        loadChildren: () => import('./components/pages/send-email-after-register/send-email-after-register.module').then(m => m.SendEmailAfterRegisterModule)
    },

    // === LAZY: payment pages ===
    {
        path: 'payment-success',
        loadChildren: () => import('./components/pages/payment-success/payment-success.module').then(m => m.PaymentSuccessModule)
    },
    {
        path: 'payment-error',
        loadChildren: () => import('./components/pages/payment-error/payment-error.module').then(m => m.PaymentErrorModule)
    },

    // === LAZY: password recovery ===
    {
        path: 'forgot-password',
        loadChildren: () => import('./components/pages/forgot-password/forgot-password.module').then(m => m.ForgotPasswordModule)
    },
    {
        path: 'reset-password',
        loadChildren: () => import('./components/pages/reset-password/reset-password.module').then(m => m.ResetPasswordModule)
    },

    // === LAZY: authenticated user pages ===
    {
        path: 'my-course',
        canActivate: [AuthGuard],
        loadChildren: () => import('./components/pages/account-profile-courses/account-profile-courses.module').then(m => m.AccountProfileCoursesModule)
    },
    {
        path: 'my-information',
        canActivate: [AuthGuard],
        loadChildren: () => import('./components/pages/account-profile-details/account-profile-details.module').then(m => m.AccountProfileDetailsModule)
    },
    {
        path: 'notification',
        canActivate: [AuthGuard],
        loadChildren: () => import('./components/pages/notification-users/notification-users.module').then(m => m.NotificationUsersModule)
    },
    {
        path: 'bookmark-course',
        canActivate: [AuthGuard],
        loadChildren: () => import('./components/pages/account-bookmark-course/account-bookmark-course.module').then(m => m.AccountBookmarkCourseModule)
    },
    {
        path: 'payment/:purchaseCode',
        canActivate: [AuthGuard],
        loadChildren: () => import('./components/pages/checkout/checkout.module').then(m => m.CheckoutModule)
    },

    // === LAZY: info pages ===
    { path: 'about',              loadChildren: () => import('./components/pages/about/about.module').then(m => m.AboutModule) },
    { path: 'teacher',            loadChildren: () => import('./components/pages/teacher/teacher.module').then(m => m.TeacherModule) },
    { path: 'infomation-teacher', loadChildren: () => import('./components/pages/profile-teacher/profile-teacher.module').then(m => m.ProfileTeacherModule) },
    { path: 'faq',                loadChildren: () => import('./components/pages/faq/faq.module').then(m => m.FaqModule) },
    { path: 'coming-soon',        loadChildren: () => import('./components/pages/coming-soon/coming-soon.module').then(m => m.ComingSoonModule) },
    { path: 'purchase-guide',     loadChildren: () => import('./components/pages/purchase-guide/purchase-guide.module').then(m => m.PurchaseGuideModule) },
    { path: 'privacy-policy',     loadChildren: () => import('./components/pages/privacy-policy/privacy-policy.module').then(m => m.PrivacyPolicyModule) },
    { path: 'terms-of-service',   loadChildren: () => import('./components/pages/terms-of-service/terms-of-service.module').then(m => m.TermsOfServiceModule) },
    { path: 'contact',            loadChildren: () => import('./components/pages/contact-us/contact-us.module').then(m => m.ContactUsModule) },
    { path: 'become-teacher',     loadChildren: () => import('./components/pages/become-a-teacher/become-a-teacher.module').then(m => m.BecomeATeacherModule) },
    { path: 'membership-levels',  loadChildren: () => import('./components/pages/membership-levels/membership-levels.module').then(m => m.MembershipLevelsModule) },

    // === LAZY: secondary pages ===
    { path: 'download-document',    loadChildren: () => import('./components/pages/down-load-recourses/down-load-recourses.module').then(m => m.DownLoadRecoursesModule) },
    { path: 'products',             loadChildren: () => import('./components/pages/products-list/products-list.module').then(m => m.ProductsListModule) },
    { path: 'cart',                 loadChildren: () => import('./components/pages/cart/cart.module').then(m => m.CartModule) },
    { path: 'details-products',     loadChildren: () => import('./components/pages/products-details/products-details.module').then(m => m.ProductsDetailsModule) },
    { path: 'gallery',              loadChildren: () => import('./components/pages/gallery/gallery.module').then(m => m.GalleryModule) },
    { path: 'news',                 loadChildren: () => import('./components/pages/courses-news/courses-news.module').then(m => m.CoursesNewsModule) },
    { path: 'detail-news/:slug',    loadChildren: () => import('./components/pages/courses-details-news/courses-details-news.module').then(m => m.CoursesDetailsNewsModule) },
    { path: 'training-online',      loadChildren: () => import('./components/pages/training-courses-list/training-courses-list.module').then(m => m.TrainingCoursesListModule) },
    { path: 'detail-training-online', loadChildren: () => import('./components/pages/training-courses-details/training-courses-details.module').then(m => m.TrainingCoursesDetailsModule) },

    // === 404 fallback ===
    { path: '**', component: NotFoundComponent },
];

@NgModule({
    imports: [RouterModule.forRoot(routes, { scrollPositionRestoration: 'top' })],
    exports: [RouterModule]
})
export class AppRoutingModule {}
