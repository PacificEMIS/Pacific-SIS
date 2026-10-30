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

import { GradesReportRoutingModule } from './grades-report-routing.module';
import { GradesReportComponent } from './grades-report.component';
import { GradeBreakdownComponent } from './grade-breakdown/grade-breakdown.component';
import { MatLegacyRadioModule as MatRadioModule } from '@angular/material/legacy-radio';
import { ClassRankListComponent } from './class-rank-list/class-rank-list.component';
import { MatLegacyPaginatorModule as MatPaginatorModule } from '@angular/material/legacy-paginator';
import { StudentFinalGradesComponent } from './student-final-grades/student-final-grades.component';
import { ProgressReportComponent } from './progress-report/progress-report.component';
import { HonorRollComponent } from './honor-roll/honor-roll.component';
import { SearchStudentComponent } from './search-student/search-student.component';
import { MatLegacyTooltipModule as MatTooltipModule } from '@angular/material/legacy-tooltip';
import { SearchStudentModule } from 'src/app/common/search-student/search-student.module';


@NgModule({
  declarations: [
    GradesReportComponent,
    GradeBreakdownComponent,
    ProgressReportComponent,
    ClassRankListComponent,
    StudentFinalGradesComponent,
    HonorRollComponent,
    SearchStudentComponent
  ],
  imports: [
    CommonModule,
    GradesReportRoutingModule,
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
    MatRadioModule,
    MatPaginatorModule,
    MatTooltipModule,
    SearchStudentModule
  ]
})
export class GradesReportModule { }
