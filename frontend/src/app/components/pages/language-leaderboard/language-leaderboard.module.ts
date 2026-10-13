import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonComponentsModule } from '../../common/common-components.module';
import { LanguageLeaderboardComponent } from './language-leaderboard.component';

@NgModule({
  declarations: [LanguageLeaderboardComponent],
  imports: [
    CommonComponentsModule,
    RouterModule.forChild([{ path: '', component: LanguageLeaderboardComponent }])
  ]
})
export class LanguageLeaderboardModule {}
