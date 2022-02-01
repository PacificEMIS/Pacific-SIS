import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TakeAttendanceComponent } from './take-attendance/take-attendance.component';
import { TeacherMissingAttendanceComponent } from './teacher-missing-attendance.component';

const routes: Routes = [
  {
    path: "",
    component: TeacherMissingAttendanceComponent,
  },
  {
    path: "take-attendance",
    component: TakeAttendanceComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TeacherMissingAttendanceRoutingModule { }
