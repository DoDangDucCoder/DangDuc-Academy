import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: '',
    loadChildren: () =>
      import('./dashboards/dashboards.module').then((m) => m.DashboardsModule),
  },
  {
    path: 'manager-system',
    loadChildren: () =>
      import('./manager-system/manager-system.module').then((m) => m.ManagerSystemModule),
  },
  {
    path: 'learning',
    loadChildren: () =>
      import('./learning/learning.module').then((m) => m.LearningModule),
  },
  {
    path: 'purchase-order',
    loadChildren: () =>
      import('./purchase-order/purchase-order.module').then((m) => m.PurchaseOrderModule),
  },
  {
    path: 'subscriptions',
    loadChildren: () =>
      import('./subscriptions/subscriptions.module').then(
        (m) => m.SubscriptionsModule
      ),
  },
  {
    path: 'user-manager',
    loadChildren: () =>
      import('./user-manager/user-manager.module').then(
        (m) => m.UserManagerModule
      ),
  },
  {
    path: 'articles',
    loadChildren: () =>
      import('./articles/articles.module').then((m) => m.ArticlesModule),
  },
  {
    path: 'refund-requests',
    loadChildren: () =>
      import('./refund-requests/refund-requests.module').then(m => m.RefundRequestsModule),
  },
  {
    path: 'payout-requests',
    loadChildren: () =>
      import('./payout-requests/payout-requests.module').then(m => m.PayoutRequestsModule),
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PagesRoutingModule {}
