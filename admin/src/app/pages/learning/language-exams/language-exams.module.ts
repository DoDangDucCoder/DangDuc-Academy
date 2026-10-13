import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

// Page Route
import { LanguageExamsRoutingModule } from './language-exams-routing.module';
import { SharedModule } from 'src/app/shared/shared.module';

// ngx-bootstrap
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ModalModule } from 'ngx-bootstrap/modal';

// TransferHttp
import { TransferHttp } from 'src/app/core/transfer-http/transfer-http';

// Component
import { LanguageExamsComponent } from './language-exams.component';

@NgModule({
  declarations: [
    LanguageExamsComponent
  ],
  imports: [
    CommonModule,
    LanguageExamsRoutingModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    BsDropdownModule.forRoot(),
    PaginationModule.forRoot(),
    ModalModule.forRoot(),
  ],
  providers: [TransferHttp],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class LanguageExamsModule { }
