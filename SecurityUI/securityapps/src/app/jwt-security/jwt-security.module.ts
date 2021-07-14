//leaves.module.ts
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { JwtSecurityRoutingModule } from './jwt-security-routing.module';

// import { Page404leavesComponent } from './page404leaves/page404leaves.component';


@NgModule({
  declarations: [

  ],
  imports: [
    CommonModule,
    FormsModule,
    JwtSecurityRoutingModule
  ]
})
export class JwtSecurityModule { }
