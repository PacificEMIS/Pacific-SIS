import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TeacherViewScheduleComponent } from './teacher-view-schedule.component';

const routes: Routes = [
  {
    path: '',
    component: TeacherViewScheduleComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TeacherViewScheduleRoutingModule { }
