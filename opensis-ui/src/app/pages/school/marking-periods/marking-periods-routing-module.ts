import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MarkingPeriodsComponent } from '../marking-periods/marking-periods.component';



const routes: Routes = [
 {
     path:'',
     component: MarkingPeriodsComponent
 } 
];


@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MarkingPeriodsRoutingModule {
}
