import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { TrainingCoursesDetailsComponent } from './training-courses-details.component';
import { LightgalleryModule } from 'lightgallery/angular/13';

const routes: Routes = [
    { path: '', component: TrainingCoursesDetailsComponent }
];

@NgModule({
    declarations: [TrainingCoursesDetailsComponent],
    imports: [CommonModule, RouterModule.forChild(routes), LightgalleryModule]
})
export class TrainingCoursesDetailsModule { }
