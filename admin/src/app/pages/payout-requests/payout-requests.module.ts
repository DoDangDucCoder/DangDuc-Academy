import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { PayoutRequestsRoutingModule } from './payout-requests-routing.module';
import { PayoutRequestsComponent } from './payout-requests.component';
import { SharedModule } from 'src/app/shared/shared.module';

@NgModule({
  declarations: [
    PayoutRequestsComponent
  ],
  imports: [
    CommonModule,
    PayoutRequestsRoutingModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [
    DatePipe
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]
})
export class PayoutRequestsModule { }
