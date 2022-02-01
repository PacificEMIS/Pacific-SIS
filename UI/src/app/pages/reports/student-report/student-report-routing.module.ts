import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AddDropReportComponent } from './add-drop-report/add-drop-report.component';
import { AdvanceReportComponent } from './advance-report/advance-report.component';
import { ContactInfoReportComponent } from './contact-info-report/contact-info-report.component';
import { EnrollmentReportComponent } from './enrollment-report/enrollment-report.component';
import { PrintStudentInfoComponent } from './print-student-info/print-student-info.component';
import { StudentReportComponent } from './student-report.component';

const routes: Routes = [
  {
    path:'',
    component: StudentReportComponent,
    children:[     
      {
        path:'add-drop-report',
        component: AddDropReportComponent
      },
      {
        path:'student-info',
        component: PrintStudentInfoComponent
      },
      {
        path:'contact-info',
        component: ContactInfoReportComponent
      },
      {
        path:'enrollment-report',
        component: EnrollmentReportComponent
      },
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
export class StudentReportRoutingModule { }
