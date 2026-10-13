import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonComponentsModule } from '../../common/common-components.module';
import { RegisterPagesComponent } from './register-pages.component';

@NgModule({
  declarations: [RegisterPagesComponent],
  imports: [
    CommonComponentsModule,
    RouterModule.forChild([
      { path: '', component: RegisterPagesComponent }
    ])
  ]
})
export class RegisterPagesModule {}
