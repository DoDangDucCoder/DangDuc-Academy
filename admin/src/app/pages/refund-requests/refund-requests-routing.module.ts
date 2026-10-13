import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RefundRequestsComponent } from './refund-requests.component';

const routes: Routes = [
  {
    path: '',
    component: RefundRequestsComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RefundRequestsRoutingModule { }
