import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatLegacyInputModule as MatInputModule } from '@angular/material/legacy-input';
import { MatLegacySelectModule as MatSelectModule } from '@angular/material/legacy-select';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { MatLegacyCardModule as MatCardModule } from '@angular/material/legacy-card';
import { MatLegacyFormFieldModule as MatFormFieldModule } from '@angular/material/legacy-form-field';
import { SecondaryToolbarModule } from '@vex/components/secondary-toolbar/secondary-toolbar.module';
import { BreadcrumbsModule } from '@vex/components/breadcrumbs/breadcrumbs.module';
import { PageLayoutModule } from '@vex/components/page-layout/page-layout.module';
import { ContainerModule } from '@vex/directives/container/container.module';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { TranslateModule } from '@ngx-translate/core';
import { MatLegacyTableModule as MatTableModule } from '@angular/material/legacy-table';
import { MatLegacyPaginatorModule as MatPaginatorModule } from '@angular/material/legacy-paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatLegacyCheckboxModule as MatCheckboxModule } from '@angular/material/legacy-checkbox';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatDividerModule } from '@angular/material/divider';
import { MatLegacySlideToggleModule as MatSlideToggleModule } from '@angular/material/legacy-slide-toggle';
import { SharedModuleModule } from '../../shared-module/shared-module.module';
import { MatLegacyProgressSpinnerModule as MatProgressSpinnerModule } from '@angular/material/legacy-progress-spinner';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatLegacyMenuModule as MatMenuModule } from "@angular/material/legacy-menu";
import { AttendanceRoutingModule } from './attendance-routing.module';
import { AttendanceComponent } from './attendance.component';
import { AttendanceReportComponent } from './attendance-report/attendance-report.component';
import { MatLegacyTooltipModule as MatTooltipModule } from '@angular/material/legacy-tooltip';
import { AverageAttendanceByDayComponent } from './average-attendance-by-day/average-attendance-by-day.component';
import { AverageDailyAttendanceComponent } from './average-daily-attendance/average-daily-attendance.component';
import { AbsenceSummaryComponent } from './absence-summary/absence-summary.component';
import { AbsenceSummaryDetailsComponent } from './absence-summary/absence-summary-details/absence-summary-details.component';
import { AttendanceChartComponent } from './attendance-chart/attendance-chart.component';
import { AttendanceChartDetailsComponent } from './attendance-chart/attendance-chart-details/attendance-chart-details.component';
import { SearchStudentComponent } from './absence-summary/search-student/search-student.component';
import { SearchStudentModule } from 'src/app/common/search-student/search-student.module';


@NgModule({
  declarations: [AttendanceComponent, AttendanceReportComponent, AverageAttendanceByDayComponent, AverageDailyAttendanceComponent, AbsenceSummaryComponent, AbsenceSummaryDetailsComponent,AttendanceChartComponent,AttendanceChartDetailsComponent,SearchStudentComponent],
  imports: [
    CommonModule,
    AttendanceRoutingModule,
    MatIconModule,
    MatInputModule,
    MatSelectModule,
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
    FormsModule,
    NgxMatSelectSearchModule,
    MatExpansionModule,
    MatTooltipModule,
    MatMenuModule,
    SearchStudentModule
  ]
})
export class AttendanceModule { }
