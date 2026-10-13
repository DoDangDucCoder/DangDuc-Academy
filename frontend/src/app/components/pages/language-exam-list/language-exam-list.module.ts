import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonComponentsModule } from '../../common/common-components.module';
import { LanguageExamListComponent } from './language-exam-list.component';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [LanguageExamListComponent],
  imports: [
    CommonComponentsModule,
    FormsModule,
    RouterModule.forChild([{ path: '', component: LanguageExamListComponent }])
  ]
})
export class LanguageExamListModule {}
