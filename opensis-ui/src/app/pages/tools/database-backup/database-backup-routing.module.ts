import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { DatabaseBackupComponent } from './database-backup.component';

const routes: Routes = [
  {
    path:'',
    component: DatabaseBackupComponent
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DatabaseBackupRoutingModule { }
