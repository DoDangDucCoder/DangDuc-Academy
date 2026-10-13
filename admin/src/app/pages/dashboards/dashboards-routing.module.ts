import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

// Component
import { ELearningComponent } from './e-learning/e-learning.component';

const routes: Routes = [
  {
    path: "",
    component: ELearningComponent
  },
  {
    path: "e-learning",
    component: ELearningComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardsRoutingModule { }
