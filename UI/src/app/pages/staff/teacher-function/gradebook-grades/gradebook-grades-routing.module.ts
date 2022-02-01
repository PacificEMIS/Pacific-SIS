import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { GradebookGradeDetailsComponent } from './gradebook-grade-details/gradebook-grade-details.component';
import { GradebookGradeListComponent } from './gradebook-grade-list/gradebook-grade-list.component';
import { GradebookGradesComponent } from './gradebook-grades.component';

const routes: Routes = [
  {
    path: "",
    component: GradebookGradesComponent,
  },
  // {
  //   path: "gradebook-grade-details",
  //   component: GradebookGradeDetailsComponent,
  // },
  {
    path: "gradebook-grade-list",
    component: GradebookGradeListComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class GradebookGradesRoutingModule { }
