import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { CartComponent } from './cart.component';
import { CommonComponentsModule } from '../../common/common-components.module';

const routes: Routes = [
    { path: '', component: CartComponent }
];

@NgModule({
    declarations: [CartComponent],
    imports: [CommonModule, CommonComponentsModule, RouterModule.forChild(routes)]
})
export class CartModule { }
