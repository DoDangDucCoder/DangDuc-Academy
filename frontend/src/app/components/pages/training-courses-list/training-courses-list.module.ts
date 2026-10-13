import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { TrainingCoursesComponent } from './training-courses-list.component';

const routes: Routes = [
    { path: '', component: TrainingCoursesComponent }
];

@NgModule({
    declarations: [TrainingCoursesComponent],
    imports: [CommonModule, RouterModule.forChild(routes)]
})
export class TrainingCoursesListModule { }
