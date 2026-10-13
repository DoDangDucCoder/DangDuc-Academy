import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonComponentsModule } from '../../common/common-components.module';
import { PaymentSuccessComponent } from './payment-success.component';

@NgModule({
  declarations: [PaymentSuccessComponent],
  imports: [
    CommonComponentsModule,
    RouterModule.forChild([
      { path: '', component: PaymentSuccessComponent }
    ])
  ]
})
export class PaymentSuccessModule {}
