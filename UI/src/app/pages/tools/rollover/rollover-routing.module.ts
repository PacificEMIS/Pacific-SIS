import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { RolloverComponent } from './rollover.component';

const routes: Routes = [
  {
    path: '',
    component: RolloverComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RolloverRoutingModule { }
