import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { GroupAssignStudentInfoComponent } from './group-assign-student-info.component';

const routes: Routes = [
  {
    path: '',
    component: GroupAssignStudentInfoComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class GroupAssignStudentInfoRoutingModule { }
