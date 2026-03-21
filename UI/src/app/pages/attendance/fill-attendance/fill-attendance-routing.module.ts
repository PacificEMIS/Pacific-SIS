import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { FillAttendanceComponent } from './fill-attendance.component';

const routes: Routes = [
  {
    path: '',
    component: FillAttendanceComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FillAttendanceRoutingModule { }
