import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonComponentsModule } from '../../common/common-components.module';
import { SendEmailAfterRegisterComponent } from './send-email-after-register.component';

@NgModule({
  declarations: [SendEmailAfterRegisterComponent],
  imports: [
    CommonComponentsModule,
    RouterModule.forChild([{ path: '', component: SendEmailAfterRegisterComponent }])
  ]
})
export class SendEmailAfterRegisterModule {}
