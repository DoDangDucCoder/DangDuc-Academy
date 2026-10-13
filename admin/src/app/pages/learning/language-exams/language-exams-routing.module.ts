import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LanguageExamsComponent } from './language-exams.component';

const routes: Routes = [
  {
    path: '',
    component: LanguageExamsComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LanguageExamsRoutingModule { }
