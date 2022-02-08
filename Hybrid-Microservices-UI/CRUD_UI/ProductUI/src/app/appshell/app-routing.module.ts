import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MicroserviceCurdProductComponent } from './../microservice-curd-product/microservice-curd-product.component';
import { AddUpdateProductComponent } from './../microservice-curd-product/add-update-product/add-update-product.component';
import {DialogProductComponent} from "../microservice-curd-product/dialog-product/dialog-product.component";
import {GenerateZipFileComponent} from "../ZipFile/generate-zip-file/generate-zip-file.component";

const routes: Routes = [
  { path: '', redirectTo: '/', pathMatch: 'full' },
  { path: 'product', component: MicroserviceCurdProductComponent },
  { path: 'productdetails', component: AddUpdateProductComponent },
  { path: 'productdetails/:selectedProductId', component: AddUpdateProductComponent },
  { path: 'productdetail', component: DialogProductComponent },
  { path: 'zipfile', component: GenerateZipFileComponent },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})

export class AppRoutingModule { }
