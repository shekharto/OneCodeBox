
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import {HomeComponent} from "./home.component";
import {PatientComponent} from "./patient/patient.component";
import {MeasuresComponent} from "./measures/measures.component";
import {AuthGuard} from "../helper";

const routes: Routes = [
    {
      path: '', component: HomeComponent, canActivate: [AuthGuard], children: [
        {
          path: 'patient', component: PatientComponent
        },
        {
          path: 'measure', component: MeasuresComponent
        },
      ],
    }
  ]

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HomeRoutingModule { }
