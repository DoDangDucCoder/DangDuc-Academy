import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { ListArticlesComponent } from './list-articles/list-articles.component';
import { AddEditArticleComponent } from './add-edit-article/add-edit-article.component';

const routes: Routes = [
  {
    path: '',
    component: ListArticlesComponent
  },
  {
    path: 'add',
    component: AddEditArticleComponent
  },
  {
    path: 'edit/:id',
    component: AddEditArticleComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ArticlesRoutingModule { }
