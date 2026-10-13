import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { MembershipLevelsComponent } from './membership-levels.component';

const routes: Routes = [
    { path: '', component: MembershipLevelsComponent }
];

@NgModule({
    declarations: [MembershipLevelsComponent],
    imports: [CommonModule, RouterModule.forChild(routes)]
})
export class MembershipLevelsModule { }
