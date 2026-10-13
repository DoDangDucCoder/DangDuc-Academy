import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonComponentsModule } from '../../common/common-components.module';
import { CoursesListComponent } from './courses-list.component';
import { PaginationModule } from 'ngx-bootstrap/pagination';

@NgModule({
  declarations: [CoursesListComponent],
  imports: [
    CommonComponentsModule,
    PaginationModule.forRoot(),
    RouterModule.forChild([
      { path: '', component: CoursesListComponent }
    ])
  ]
})
export class CoursesListModule {}
