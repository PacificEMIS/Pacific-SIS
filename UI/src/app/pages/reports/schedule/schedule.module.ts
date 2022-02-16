import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ScheduleRoutingModule } from './schedule-routing.module';
import { ScheduleComponent } from './schedule.component';
import { AddDropReportComponent } from './add-drop-report/add-drop-report.component';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { IconModule } from '@visurel/iconify-angular';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { SecondaryToolbarModule } from 'src/@vex/components/secondary-toolbar/secondary-toolbar.module';
import { BreadcrumbsModule } from 'src/@vex/components/breadcrumbs/breadcrumbs.module';
import { PageLayoutModule } from 'src/@vex/components/page-layout/page-layout.module';
import { ContainerModule } from 'src/@vex/directives/container/container.module';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { TranslateModule } from '@ngx-translate/core';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatDividerModule } from '@angular/material/divider';
import { ClassListComponent } from './class-list/class-list.component';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { SharedModuleModule } from '../../shared-module/shared-module.module';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatTooltipModule } from '@angular/material/tooltip';
import { PrintSchedulesComponent } from './print-schedules/print-schedules.component';
import { ScheduleReportComponent } from './schedule-report/schedule-report.component';
import { ScheduleReportDetailsComponent } from './schedule-report/schedule-report-details/schedule-report-details.component';


@NgModule({
  declarations: [ScheduleComponent, AddDropReportComponent, ClassListComponent, PrintSchedulesComponent, ScheduleReportComponent, ScheduleReportDetailsComponent],
  imports: [
    CommonModule,
    ScheduleRoutingModule,
    MatIconModule,
    MatInputModule,
    MatSelectModule,
    IconModule,
    MatButtonModule,   
    MatCardModule,
    MatFormFieldModule,
    SecondaryToolbarModule,
    BreadcrumbsModule,
    PageLayoutModule,
    ContainerModule,
    MatDatepickerModule,
    MatNativeDateModule,
    TranslateModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatCheckboxModule,
    MatButtonToggleModule,
    MatDividerModule,
    MatSlideToggleModule,
    SharedModuleModule,
    MatProgressSpinnerModule,
    ReactiveFormsModule,
    MatTooltipModule,
    FormsModule
  ]
})
export class ScheduleModule { }
