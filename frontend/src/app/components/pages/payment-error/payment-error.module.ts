import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonComponentsModule } from '../../common/common-components.module';
import { PaymentErrorComponent } from './payment-error.component';

@NgModule({
  declarations: [PaymentErrorComponent],
  imports: [
    CommonComponentsModule,
    RouterModule.forChild([{ path: '', component: PaymentErrorComponent }])
  ]
})
export class PaymentErrorModule {}
