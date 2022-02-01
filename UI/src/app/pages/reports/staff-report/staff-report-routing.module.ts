import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AdvanceReportComponent } from './advance-report/advance-report.component';
import { StaffReportComponent } from './staff-report.component';

const routes: Routes = [
  {
    path:'',
    component: StaffReportComponent,
    children:[     
      {
        path:'advance-report',
        component: AdvanceReportComponent
      },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StaffReportRoutingModule { }
