import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { DownLoadRecoursesComponent } from './down-load-resourses.component';
import { CommonComponentsModule } from '../../common/common-components.module';

const routes: Routes = [
    { path: '', component: DownLoadRecoursesComponent }
];

@NgModule({
    declarations: [DownLoadRecoursesComponent],
    imports: [CommonModule, CommonComponentsModule, RouterModule.forChild(routes)]
})
export class DownLoadRecoursesModule { }
