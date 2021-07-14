import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import {JwtLoginComponent} from './jwt-security/jwt-login.component';

import {AuthGuard} from './jwt-security/helper/auth.guard';
import { Page404Component } from './page404/page404.component';
import { JwtSecurityModule } from './jwt-security/jwt-security.module'


const routes: Routes = [
//  { path: '', component: HomeComponent, canActivate: [AuthGuard] },

  { path: '', component: Page404Component },
  { path: 'jwtlogin', loadChildren: () => import(`./jwt-security/jwt-security.module`).then(m => m.JwtSecurityModule) },

 // { path: '', redirectTo: 'jwtlogin' },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
