import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MicroserviceCurdProductComponent } from './../microservice-curd-product/microservice-curd-product.component';
import { AddUpdateProductComponent } from './../microservice-curd-product/add-update-product/add-update-product.component';

const routes: Routes = [
  { path: '', redirectTo: '/', pathMatch: 'full' },
  { path: 'product', component: MicroserviceCurdProductComponent },
  { path: 'productdetails', component: AddUpdateProductComponent },
  { path: 'productdetails/:selectedProductId', component: AddUpdateProductComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})



export class AppRoutingModule { }
