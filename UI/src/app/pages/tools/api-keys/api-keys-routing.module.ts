import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ApiModuleDatailsComponent } from './api-module-datails/api-module-datails.component';
import { ApiKeysComponent } from './api-keys.component';

const routes: Routes = [
  {
    path: '',
    component: ApiKeysComponent
  },
  {
    path: 'api-module-details',
    component: ApiModuleDatailsComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ApiKeysRoutingModule { }
