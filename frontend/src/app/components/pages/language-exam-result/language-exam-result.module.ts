import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonComponentsModule } from '../../common/common-components.module';
import { CertificateComponent } from './certificate.component';
import { LanguageExamResultComponent } from './language-exam-result.component';
import { AuthGuard } from 'src/app/core/guards/auth.guard';

@NgModule({
  declarations: [LanguageExamResultComponent, CertificateComponent],
  imports: [
    CommonComponentsModule,
    RouterModule.forChild([{ path: '', component: LanguageExamResultComponent, canActivate: [AuthGuard] }])
  ]
})
export class LanguageExamResultModule {}
