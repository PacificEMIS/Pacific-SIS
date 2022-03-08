import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ProgressReportDetailsComponent } from 'src/app/pages/grades/progress-reports/progress-report-details/progress-report-details.component';
import { StudentProgressReportComponent } from './student-progress-report.component';

const routes: Routes = [
  {
    path: '',
    component: StudentProgressReportComponent,
    children: [
      {
        path: '',
        component: ProgressReportDetailsComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StudentProgressReportRoutingModule { }
