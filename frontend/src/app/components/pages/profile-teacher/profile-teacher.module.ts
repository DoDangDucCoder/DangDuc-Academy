import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { ProfileTeacherComponent } from './profile-teacher.component';
import { FormsModule } from '@angular/forms';

const routes: Routes = [
    { path: '', component: ProfileTeacherComponent }
];

@NgModule({
    declarations: [ProfileTeacherComponent],
    imports: [CommonModule, RouterModule.forChild(routes), FormsModule]
})
export class ProfileTeacherModule { }
