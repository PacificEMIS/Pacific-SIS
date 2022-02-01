import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TakeAttendanceComponent } from './take-attendance/take-attendance.component';
import { TeacherFunctionComponent } from './teacher-function.component';
import { InputFinalGradeComponent } from './input-final-grade/input-final-grade.component';
import { InputEffortGradesComponent } from './input-effort-grades/input-effort-grades.component';
import { GradeDetailsComponent } from './input-final-grade/grade-details/grade-details.component';
import { EffortGradeDetailsComponent } from './input-effort-grades/effort-grade-details/effort-grade-details.component';

const routes: Routes = [
  {
    path: "",
    component: TeacherFunctionComponent,
    children: [
      {
        path: 'take-attendance',
        loadChildren: () => import('./take-attendance/take-attendance.module').then(m => m.TakeAttendanceModule),
      },
      {
        path: 'input-final-grade',
        loadChildren: () => import('./input-final-grade/input-final-grade.module').then(m => m.InputFinalGradeModule),
      },
      {
        path: 'gradebook-grades',
        loadChildren: () => import('./gradebook-grades/gradebook-grades.module').then(m => m.GradebookGradesModule),
      },
      {
        path: 'input-effort-grade',
        loadChildren: () => import('./input-effort-grades/input-effort-grades.module').then(m => m.InputEffortGradesModule),
      },
      {
        path: 'missing-attendance',
        loadChildren: () => import('./missing-attendance/missing-attendance.module').then(m => m.MissingAttendanceModule),
      }
    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TeacherFunctionRoutingModule { }
