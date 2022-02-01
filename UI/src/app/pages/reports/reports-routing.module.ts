import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ReportsComponent } from './reports.component';

const routes: Routes = [
  {
    path: '',
    component: ReportsComponent
  },
  {
    path: 'student-report',
    loadChildren: () => import('./student-report/student-report.module').then(m => m.StudentReportModule),
  },
  {
    path: 'schedule',
    loadChildren: () => import('./schedule/schedule.module').then(m => m.ScheduleModule),
  },
  {
    path: 'grades',
    loadChildren: () => import('./grades-report/grades-report.module').then(m => m.GradesReportModule),
  },
  {
    path: 'staff',
    loadChildren: () => import('./staff-report/staff-report.module').then(m => m.StaffReportModule),
  },
  {
    path: 'attendance',
    loadChildren: () => import('./attendance/attendance.module').then(m => m.AttendanceModule),
  },
  {
    path: 'school',
    loadChildren: () => import('./school-report/school-report.module').then(m => m.SchoolReportModule),
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReportsRoutingModule { }
