import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { ResetPasswordComponent } from './reset-password.component';

const routes: Routes = [
    { path: '', component: ResetPasswordComponent }
];

@NgModule({
    declarations: [ResetPasswordComponent],
    imports: [CommonModule, FormsModule, RouterModule.forChild(routes)]
})
export class ResetPasswordModule { }
