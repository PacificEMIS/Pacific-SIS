import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AnomalousGradesComponent } from './anomalous-grades.component';

const routes: Routes = [
  {
    path: '',
    component: AnomalousGradesComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AnomalousGradesRoutingModule { }