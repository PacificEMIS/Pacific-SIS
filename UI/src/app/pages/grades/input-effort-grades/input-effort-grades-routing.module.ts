import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { InputEffortGradesComponent } from './input-effort-grades.component';

const routes: Routes = [
  {
    path: "",
    component: InputEffortGradesComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class InputEffortGradesRoutingModule { }
