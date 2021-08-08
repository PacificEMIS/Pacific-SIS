import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NoticesComponent } from './notices.component';


const routes: Routes = [
 {
     path:'',
     component: NoticesComponent
 }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class NoticeRoutingModule {
}
