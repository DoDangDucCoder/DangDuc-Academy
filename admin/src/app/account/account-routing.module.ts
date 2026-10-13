import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

// Component
import { LoginComponent } from './login/login.component';

const routes: Routes = [
  {
    path: 'auth', loadChildren: () => import('./auth/auth.module').then(m => m.AuthModule)
  },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'register',
    redirectTo: 'login',
    pathMatch: 'full'
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccountRoutingModule { }
