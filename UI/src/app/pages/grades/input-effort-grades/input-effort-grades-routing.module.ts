import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { EffortGradeDetailsComponent } from '../../staff/teacher-function/input-effort-grades/effort-grade-details/effort-grade-details.component';
import { InputEffortGradesComponent } from './input-effort-grades.component';

const routes: Routes = [
  {
    path: "",
    component: InputEffortGradesComponent,
    children: [
      {
        path: '',
        component: EffortGradeDetailsComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class InputEffortGradesRoutingModule { }
