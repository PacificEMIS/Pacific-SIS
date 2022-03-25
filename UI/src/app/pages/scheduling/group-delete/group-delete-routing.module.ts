import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { GroupDeleteComponent } from './group-delete.component';

const routes: Routes = [
  {
    path: '',
    component: GroupDeleteComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class GroupDeleteRoutingModule { }
