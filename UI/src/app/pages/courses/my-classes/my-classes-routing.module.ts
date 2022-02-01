import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MyClassesComponent } from './my-classes.component';

const routes: Routes = [
  {
    path: '',
    component: MyClassesComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MyClassesRoutingModule { }
