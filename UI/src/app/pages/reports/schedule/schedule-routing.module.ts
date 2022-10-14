import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AddDropReportComponent } from './add-drop-report/add-drop-report.component';
import { ClassListComponent } from './class-list/class-list.component';
import { PrintSchedulesComponent } from './print-schedules/print-schedules.component';
import { ScheduleReportDetailsComponent } from './schedule-report/schedule-report-details/schedule-report-details.component';
import { ScheduleReportComponent } from './schedule-report/schedule-report.component';
import { ScheduleComponent } from './schedule.component';
import { SchoolwideScheduleReportComponent } from './schoolwide-schedule-report/schoolwide-schedule-report.component';

const routes: Routes = [
  {
    path:'',
    component: ScheduleComponent,
    children:[     
      {
        path:'add-drop-report',
        component: AddDropReportComponent
      },
      {
        path:'class-list',
        component: ClassListComponent
      },
      {
        path:'print-schedules',
        component: PrintSchedulesComponent
      },
      {
        path:'schedule-report',
        component: ScheduleReportComponent
      },
      {
        path:'schedule-report/schedule-report-details',
        component: ScheduleReportDetailsComponent
      },
      {
        path: 'schoolwide-schedule-report',
        component: SchoolwideScheduleReportComponent
      },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ScheduleRoutingModule { }
