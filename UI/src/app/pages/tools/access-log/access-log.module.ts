import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AccessLogRoutingModule } from './access-log-routing.module';
import { AccessLogComponent } from './access-log.component';
import { SecondaryToolbarModule } from 'src/@vex/components/secondary-toolbar/secondary-toolbar.module';
import { BreadcrumbsModule } from 'src/@vex/components/breadcrumbs/breadcrumbs.module';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { PageLayoutModule } from 'src/@vex/components/page-layout/page-layout.module';
import { ContainerModule } from 'src/@vex/directives/container/container.module';
import { IconModule } from '@visurel/iconify-angular';
import { MatCardModule } from '@angular/material/card';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { MatMenuModule } from '@angular/material/menu';
import { TranslateModule } from '@ngx-translate/core';
import { MatInputModule } from '@angular/material/input';
import { MatDividerModule } from '@angular/material/divider';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModuleModule } from '../../shared-module/shared-module.module';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatTooltipModule } from '@angular/material/tooltip';


@NgModule({
  declarations: [AccessLogComponent],
  imports: [
    CommonModule,
    AccessLogRoutingModule,
    SecondaryToolbarModule,
    BreadcrumbsModule,
    MatIconModule,
    MatButtonModule,
    PageLayoutModule,
    ContainerModule,
    IconModule,
    MatCardModule,
    MatPaginatorModule,
    MatTableModule,
    MatMenuModule,
    TranslateModule,
    MatInputModule,
    MatDividerModule,
    MatDatepickerModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModuleModule,
    MatCheckboxModule,
    MatTooltipModule
  ]
})
export class AccessLogModule { }
