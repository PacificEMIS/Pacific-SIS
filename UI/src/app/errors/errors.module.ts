import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ErrorsRoutingModule } from './errors-routing.module';
import { StatusCode404Component } from './status-code404/status-code404.component';
import { ErrorsComponent } from './errors.component';


@NgModule({
  declarations: [StatusCode404Component, ErrorsComponent],
  imports: [
    CommonModule,
    ErrorsRoutingModule
  ]
})
export class ErrorsModule { }
