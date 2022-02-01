import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { InstituteReportComponent } from './institute-report/institute-report.component';
import { SchoolReportComponent } from './school-report.component';

const routes: Routes = [
  {
    path:'',
    component: SchoolReportComponent,
    children:[     
      {
        path:'institute-report',
        component: InstituteReportComponent
      },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SchoolReportRoutingModule { }
