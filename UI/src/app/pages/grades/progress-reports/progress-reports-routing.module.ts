import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ProgressReportDetailsComponent } from './progress-report-details/progress-report-details.component';
import { ProgressReportsComponent } from './progress-reports.component';

const routes: Routes = [
  {
    path: '',
    component: ProgressReportsComponent,
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
export class ProgressReportsRoutingModule { }
