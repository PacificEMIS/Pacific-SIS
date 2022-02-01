import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { CourseCatalogComponent } from './course-catalog.component';

const routes: Routes = [
  {
    path: "",
    component: CourseCatalogComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CourseCatalogRoutingModule { }
