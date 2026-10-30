import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StudentReportComponent } from './student-report.component';
import { AddDropReportComponent } from './add-drop-report/add-drop-report.component';
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
import { PrintStudentInfoComponent } from './print-student-info/print-student-info.component';
import { MatDividerModule } from '@angular/material/divider';
import { MatLegacySlideToggleModule as MatSlideToggleModule } from '@angular/material/legacy-slide-toggle';
import { StudentReportRoutingModule } from './student-report-routing.module';
import { SharedModuleModule } from '../../shared-module/shared-module.module';
import { MatLegacyProgressSpinnerModule as MatProgressSpinnerModule } from '@angular/material/legacy-progress-spinner';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SearchStudentComponent } from './search-student/search-student.component';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { MatExpansionModule } from '@angular/material/expansion';
import { ContactInfoReportComponent } from './contact-info-report/contact-info-report.component';
import { EnrollmentReportComponent } from './enrollment-report/enrollment-report.component';
import { AdvanceReportComponent } from './advance-report/advance-report.component';
import { MatLegacyTooltipModule as MatTooltipModule } from '@angular/material/legacy-tooltip';
import { MatLegacyMenuModule as MatMenuModule } from '@angular/material/legacy-menu';
import { SearchStudentModule } from 'src/app/common/search-student/search-student.module';





@NgModule({
  declarations: [
    StudentReportComponent,
    AddDropReportComponent,
    PrintStudentInfoComponent,
    SearchStudentComponent,
    ContactInfoReportComponent,
    EnrollmentReportComponent,
    AdvanceReportComponent
  ],
  imports: [
    CommonModule,
    StudentReportRoutingModule,
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
    MatProgressSpinnerModule,
    SharedModuleModule,
    ReactiveFormsModule,
    MatTooltipModule,
    ReactiveFormsModule,
    FormsModule,
    NgxMatSelectSearchModule,
    MatExpansionModule,
    MatMenuModule,
    SearchStudentModule
  ]
})
export class StudentReportModule { }
