import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { PurchaseGuideComponent } from './purchase-guide.component';

const routes: Routes = [
    { path: '', component: PurchaseGuideComponent }
];

@NgModule({
    declarations: [PurchaseGuideComponent],
    imports: [CommonModule, RouterModule.forChild(routes)]
})
export class PurchaseGuideModule { }
