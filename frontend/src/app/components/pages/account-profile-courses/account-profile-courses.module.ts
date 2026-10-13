import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { AccountProfileCoursesComponent } from './account-profile-courses.component';

const routes: Routes = [
    { path: '', component: AccountProfileCoursesComponent }
];

@NgModule({
    declarations: [AccountProfileCoursesComponent],
    imports: [CommonModule, RouterModule.forChild(routes)]
})
export class AccountProfileCoursesModule { }
