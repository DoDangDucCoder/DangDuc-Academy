import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { AccountBookmarkCourseComponent } from './account-bookmark-course.component';
import { CommonComponentsModule } from '../../common/common-components.module';

const routes: Routes = [
    { path: '', component: AccountBookmarkCourseComponent }
];

@NgModule({
    declarations: [AccountBookmarkCourseComponent],
    imports: [CommonModule, CommonComponentsModule, RouterModule.forChild(routes)]
})
export class AccountBookmarkCourseModule { }
