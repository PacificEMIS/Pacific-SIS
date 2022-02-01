import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AttendanceChartDetailsComponent } from './attendance-chart/attendance-chart-details/attendance-chart-details.component';
import { AttendanceChartComponent } from './attendance-chart/attendance-chart.component';
import { AbsenceSummaryDetailsComponent } from './absence-summary/absence-summary-details/absence-summary-details.component';
import { AbsenceSummaryComponent } from './absence-summary/absence-summary.component';
import { AttendanceReportComponent } from './attendance-report/attendance-report.component';
import { AttendanceComponent } from './attendance.component';
import { AverageAttendanceByDayComponent } from './average-attendance-by-day/average-attendance-by-day.component';
import { AverageDailyAttendanceComponent } from './average-daily-attendance/average-daily-attendance.component';

const routes: Routes = [
  {
    path:'',
    component: AttendanceComponent,
    children:[     
      {
        path:'attendance-report',
        component: AttendanceReportComponent
      },
      {
        path:'average-attendance-by-day',
        component: AverageAttendanceByDayComponent
      },
      {
        path:'average-daily-attendance',
        component: AverageDailyAttendanceComponent
      },
      {
        path:'attendance-chart',
        component: AttendanceChartComponent
      },
      {
        path:'attendance-chart/attendance-chart-details',
        component: AttendanceChartDetailsComponent
      },
      {
        path:'absence-summary',
        component: AbsenceSummaryComponent
      },
      {
        path:'absence-summary/absence-summary-details',
        component: AbsenceSummaryDetailsComponent
      },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AttendanceRoutingModule { }
