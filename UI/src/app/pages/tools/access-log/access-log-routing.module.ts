import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AccessLogComponent } from './access-log.component';

const routes: Routes = [
  {
    path:'',
    component: AccessLogComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AccessLogRoutingModule { }
