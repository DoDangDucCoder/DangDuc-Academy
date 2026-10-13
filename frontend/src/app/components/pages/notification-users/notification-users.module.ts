import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { NotificationUsersComponent } from './notification-users.component';

const routes: Routes = [
    { path: '', component: NotificationUsersComponent }
];

@NgModule({
    declarations: [NotificationUsersComponent],
    imports: [CommonModule, RouterModule.forChild(routes)]
})
export class NotificationUsersModule { }
