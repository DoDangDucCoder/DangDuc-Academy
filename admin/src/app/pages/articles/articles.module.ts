import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ArticlesRoutingModule } from './articles-routing.module';
import { SharedModule } from 'src/app/shared/shared.module';

// Components
import { ListArticlesComponent } from './list-articles/list-articles.component';
import { AddEditArticleComponent } from './add-edit-article/add-edit-article.component';

// Modules
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ModalModule } from 'ngx-bootstrap/modal';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { DropzoneModule } from 'ngx-dropzone-wrapper';
import { NgSelectModule } from '@ng-select/ng-select';
import { CKEditorModule } from '@ckeditor/ckeditor5-angular';

@NgModule({
  declarations: [
    ListArticlesComponent,
    AddEditArticleComponent
  ],
  imports: [
    CommonModule,
    ArticlesRoutingModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    PaginationModule,
    ModalModule.forRoot(),
    TooltipModule,
    DropzoneModule,
    NgSelectModule,
    CKEditorModule
  ],
  providers: [
    DatePipe
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class ArticlesModule { }
