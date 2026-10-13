import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonComponentsModule } from '../../common/common-components.module';
import { LanguageGrammarComponent } from './language-grammar.component';

@NgModule({
  declarations: [LanguageGrammarComponent],
  imports: [
    CommonComponentsModule,
    RouterModule.forChild([{ path: '', component: LanguageGrammarComponent }])
  ]
})
export class LanguageGrammarModule {}
