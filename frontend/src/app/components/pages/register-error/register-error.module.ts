import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonComponentsModule } from '../../common/common-components.module';
import { RegisterErrorComponent } from './register-error.component';

@NgModule({
  declarations: [RegisterErrorComponent],
  imports: [
    CommonComponentsModule,
    RouterModule.forChild([{ path: '', component: RegisterErrorComponent }])
  ]
})
export class RegisterErrorModule {}
