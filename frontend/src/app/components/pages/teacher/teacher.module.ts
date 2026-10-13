import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { TeacherComponent } from './teacher.component';
import { CommonComponentsModule } from '../../common/common-components.module';

const routes: Routes = [
    { path: '', component: TeacherComponent }
];

@NgModule({
    declarations: [TeacherComponent],
    imports: [CommonModule, CommonComponentsModule, RouterModule.forChild(routes)]
})
export class TeacherModule { }
