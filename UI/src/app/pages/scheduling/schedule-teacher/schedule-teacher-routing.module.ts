import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ScheduleTeacherComponent } from './schedule-teacher.component';



const routes: Routes = [
  {
    path: '',
    component: ScheduleTeacherComponent
  },
  
];



@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ScheduleTeacherRoutingModule {
}
