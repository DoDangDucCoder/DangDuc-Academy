import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonComponentsModule } from '../../common/common-components.module';
import { LanguageHubComponent } from './language-hub.component';

@NgModule({
  declarations: [LanguageHubComponent],
  imports: [
    CommonComponentsModule,
    RouterModule.forChild([{ path: '', component: LanguageHubComponent }])
  ]
})
export class LanguageHubModule {}
