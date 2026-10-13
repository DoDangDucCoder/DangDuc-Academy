import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonComponentsModule } from '../../common/common-components.module';
import { LanguageFlashcardComponent } from './language-flashcard.component';
import { AuthGuard } from 'src/app/core/guards/auth.guard';

@NgModule({
  declarations: [LanguageFlashcardComponent],
  imports: [
    CommonComponentsModule,
    RouterModule.forChild([{ path: '', component: LanguageFlashcardComponent, canActivate: [AuthGuard] }])
  ]
})
export class LanguageFlashcardModule {}
