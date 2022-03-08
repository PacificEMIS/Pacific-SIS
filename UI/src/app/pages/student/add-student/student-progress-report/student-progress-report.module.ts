import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { StudentProgressReportRoutingModule } from './student-progress-report-routing.module';
import { StudentProgressReportComponent } from './student-progress-report.component';


@NgModule({
  declarations: [StudentProgressReportComponent],
  imports: [
    CommonModule,
    StudentProgressReportRoutingModule
  ]
})
export class StudentProgressReportModule { }
