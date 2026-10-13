import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonComponentsModule } from '../../common/common-components.module';
import { LanguageExamDoComponent } from './language-exam-do.component';
import { AuthGuard } from 'src/app/core/guards/auth.guard';

@NgModule({
  declarations: [LanguageExamDoComponent],
  imports: [
    CommonComponentsModule,
    RouterModule.forChild([{
      path: '', component: LanguageExamDoComponent, canActivate: [AuthGuard]
    }])
  ]
})
export class LanguageExamDoModule {}
