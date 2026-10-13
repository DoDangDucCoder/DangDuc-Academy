import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { AccountProfileDetailsComponent } from './account-profile-details.component';
import { CommonComponentsModule } from '../../common/common-components.module';

const routes: Routes = [
    { path: '', component: AccountProfileDetailsComponent }
];

@NgModule({
    declarations: [AccountProfileDetailsComponent],
    imports: [CommonModule, CommonComponentsModule, RouterModule.forChild(routes)]
})
export class AccountProfileDetailsModule { }
