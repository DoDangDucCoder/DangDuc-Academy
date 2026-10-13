import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonComponentsModule } from '../../common/common-components.module';
import { CoursesDetailsComponent } from './courses-details.component';
import { NgxStarsModule } from 'ngx-stars';
import { LightgalleryModule } from 'lightgallery/angular';
import { NgbAccordionModule, NgbModalModule } from '@ng-bootstrap/ng-bootstrap';
import { MentionModule } from 'angular-mentions';

@NgModule({
  declarations: [CoursesDetailsComponent],
  imports: [
    CommonComponentsModule,
    NgxStarsModule,
    LightgalleryModule,
    NgbAccordionModule,
    NgbModalModule,
    MentionModule,
    RouterModule.forChild([
      { path: '', component: CoursesDetailsComponent }
    ])
  ]
})
export class CoursesDetailsModule {}
