import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { ProductsDetailsComponent } from './products-details.component';

const routes: Routes = [
    { path: '', component: ProductsDetailsComponent }
];

@NgModule({
    declarations: [ProductsDetailsComponent],
    imports: [CommonModule, RouterModule.forChild(routes)]
})
export class ProductsDetailsModule { }
