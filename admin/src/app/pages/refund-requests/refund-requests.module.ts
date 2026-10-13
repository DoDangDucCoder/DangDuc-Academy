import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { RefundRequestsRoutingModule } from './refund-requests-routing.module';
import { RefundRequestsComponent } from './refund-requests.component';
import { SharedModule } from 'src/app/shared/shared.module';

// Modules
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ModalModule } from 'ngx-bootstrap/modal';
import { TooltipModule } from 'ngx-bootstrap/tooltip';

@NgModule({
  declarations: [
    RefundRequestsComponent
  ],
  imports: [
    CommonModule,
    RefundRequestsRoutingModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    PaginationModule,
    ModalModule.forRoot(),
    TooltipModule
  ],
  providers: [
    DatePipe
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class RefundRequestsModule { }
