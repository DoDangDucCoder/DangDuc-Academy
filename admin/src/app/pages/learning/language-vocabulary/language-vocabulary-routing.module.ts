import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LanguageVocabularyComponent } from './language-vocabulary.component';

const routes: Routes = [
  {
    path: '',
    component: LanguageVocabularyComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class LanguageVocabularyRoutingModule { }
