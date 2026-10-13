import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { BecomeATeacherComponent } from './become-a-teacher.component';
import { CommonComponentsModule } from '../../common/common-components.module';

const routes: Routes = [
    { path: '', component: BecomeATeacherComponent }
];

@NgModule({
    declarations: [BecomeATeacherComponent],
    imports: [CommonModule, CommonComponentsModule, RouterModule.forChild(routes)]
})
export class BecomeATeacherModule { }
