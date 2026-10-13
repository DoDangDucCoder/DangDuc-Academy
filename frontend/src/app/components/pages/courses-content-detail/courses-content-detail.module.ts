import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonComponentsModule } from '../../common/common-components.module';
import { CoursesContentDetailComponent } from './courses-content-detail.component';
import { NgbAccordionModule, NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';
import { NgxStarsModule } from 'ngx-stars';
import { MentionModule } from 'angular-mentions';
import { AuthGuard } from 'src/app/core/guards/auth.guard';

@NgModule({
  declarations: [CoursesContentDetailComponent],
  imports: [
    CommonComponentsModule,
    NgbAccordionModule,
    NgbTooltipModule,
    NgxStarsModule,
    MentionModule,
    RouterModule.forChild([
      {
        path: '',
        component: CoursesContentDetailComponent,
        canActivate: [AuthGuard]
      }
    ])
  ]
})
export class CoursesContentDetailModule {}
