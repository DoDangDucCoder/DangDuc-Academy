import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

// Component
import { ListNewsComponent } from './list-news/list-news.component';
import { ListFileUploadComponent } from './list-file-upload/list-file-upload.component';
import { ListBannerComponent } from './list-banner/list-banner.component';
import { ListConfigSystemComponent } from './list-system-config/list-system-config.component';
import { CategoryComponent } from './category/category.component';
import { AuditLogComponent } from './audit-log/audit-log.component';

const routes: Routes = [
  {
    path: "list-news",
    component: ListNewsComponent
  },
  {
    path: "list-file-upload",
    component: ListFileUploadComponent
  },
  {
    path: "list-banners",
    component: ListBannerComponent
  },
  {
    path: "list-config-system",
    component: ListConfigSystemComponent
  },
  {
    path: "category",
    component: CategoryComponent
  },
  {
    path: "audit-log",
    component: AuditLogComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ManagerSystemRoutingModule { }
