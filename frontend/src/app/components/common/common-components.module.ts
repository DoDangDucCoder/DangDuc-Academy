import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { CarouselModule } from 'ngx-owl-carousel-o';
import { CountUpModule } from 'ngx-countup';
import { NgIconsModule } from '@ng-icons/core';
import { bootstrapBookmark, bootstrapBookmarkCheckFill } from '@ng-icons/bootstrap-icons';
import { LightgalleryModule } from 'lightgallery/angular';

import { FallbackSrcDirective } from '../../directives/fallback-src.directive';
import { AssetUrlPipe } from '../../library/custompipe/asset-url.pipe';
import { ApplyInstructorComponent } from './apply-instructor/apply-instructor.component';
import { CoursesComponent } from './courses/courses.component';
import { FeaturesComponent } from './features/features.component';
import { FunfactsFeedbackComponent } from './funfacts-feedback/funfacts-feedback.component';
import { HdnxLoadingComponent } from './hdnx-loading/hdnx-loading.component';
import { InstructorComponent } from './instructor/instructor.component';
import { PartnerComponent } from './partner/partner.component';
import { PremiumAccessComponent } from './premium-access/premium-access.component';
import { SubscribeComponent } from './subscribe/subscribe.component';
import { TeacherRegisterComponent } from './teacher-register/teacher-register.component';
import { TrainingComponent } from './training/training.component';
import { VideoStyleTwoComponent } from './video-style-two/video-style-two.component';
import { VideoComponent } from './video/video.component';
import { ElearningAboutComponent } from '../pages/elearning-school/elearning-about/elearning-about.component';
import { ElearningBannerComponent } from '../pages/elearning-school/elearning-banner/elearning-banner.component';
import { CourseCardComponent } from './course-card/course-card.component';

const SHARED_DECLARATIONS = [
  PartnerComponent,
  PremiumAccessComponent,
  SubscribeComponent,
  ElearningBannerComponent,
  ElearningAboutComponent,
  FeaturesComponent,
  FunfactsFeedbackComponent,
  VideoComponent,
  CoursesComponent,
  CourseCardComponent,
  InstructorComponent,
  TrainingComponent,
  ApplyInstructorComponent,
  TeacherRegisterComponent,
  VideoStyleTwoComponent,
  HdnxLoadingComponent,
  AssetUrlPipe,
  FallbackSrcDirective
];

@NgModule({
  declarations: SHARED_DECLARATIONS,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    CarouselModule,
    CountUpModule,
    LightgalleryModule,
    NgIconsModule.withIcons({ bootstrapBookmark, bootstrapBookmarkCheckFill })
  ],
  exports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    ...SHARED_DECLARATIONS
  ]
})
export class CommonComponentsModule {}
