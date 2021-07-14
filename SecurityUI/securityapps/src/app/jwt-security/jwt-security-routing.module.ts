//leaves-routing.module.ts
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import {JwtLoginComponent} from "./jwt-login.component";

const routes: Routes = [

  { path: '', component: JwtLoginComponent},
  { path: 'jwthome' ,  loadChildren: () => import(`./home/home.module`).then(m => m.HomeModule) },
  { path: '**', redirectTo: '' },


/*
 {
    path: 'home', component: HomeComponent, canActivate: [AuthGuard], children: [
      {
        path: 'population', component: PopulationComponent
      },
       {
        path: 'population', component: PopulationComponent
      },
      { path: '**', redirectTo: '' }
    ]
  } */
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class JwtSecurityRoutingModule { }
