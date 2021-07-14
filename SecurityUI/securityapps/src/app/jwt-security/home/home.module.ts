//balance.module.ts
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import {MatCardModule} from '@angular/material/card';
import { HomeRoutingModule } from './home-routing.module';
import { PatientComponent } from './patient/patient.component';
import { MeasuresComponent } from './measures/measures.component';



@NgModule({
  declarations: [
    PatientComponent,
    MeasuresComponent
  ],
  imports: [
    CommonModule,
    HomeRoutingModule ,
    MatCardModule
  ]
})
export class HomeModule { }
