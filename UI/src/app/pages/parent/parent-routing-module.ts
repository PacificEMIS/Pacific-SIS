import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ParentinfoComponent } from './parentinfo/parentinfo.component';
//import { ParentGeneralinfoComponent } from './add-student/student-generalinfo/student-generalinfo.component';


const routes: Routes = [
  {
      path:'',
      component: ParentinfoComponent
  },  
  // {
  //   path:'parent-generalinfo',
  //   component:ParentGeneralinfoComponent
  // } 
];



@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ParentRoutingModule {
}
