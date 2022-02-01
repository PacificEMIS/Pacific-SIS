import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ClassRankListComponent } from './class-rank-list/class-rank-list.component';
import { GradeBreakdownComponent } from './grade-breakdown/grade-breakdown.component';
import { GradesReportComponent } from './grades-report.component';
import { HonorRollComponent } from './honor-roll/honor-roll.component';
import { StudentFinalGradesComponent } from './student-final-grades/student-final-grades.component';
import { ProgressReportComponent } from './progress-report/progress-report.component';

const routes: Routes = [
  {
    path:'',
    component: GradesReportComponent,
    children:[     
      {
        path:'grade-breakdown',
        component: GradeBreakdownComponent
      },
      {
        path:'progress-reports',
        component: ProgressReportComponent
      },
      {
        path:'class-rank-list',
        component: ClassRankListComponent
      },
      {
        path:'honor-roll',
        component: HonorRollComponent
      },
      {
        path:'student-final-grades',
        component: StudentFinalGradesComponent
      },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class GradesReportRoutingModule { }
