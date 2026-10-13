import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PayoutRequestsComponent } from './payout-requests.component';

const routes: Routes = [
  {
    path: '',
    component: PayoutRequestsComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PayoutRequestsRoutingModule { }
