import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { IconModule } from '@visurel/iconify-angular';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { SecondaryToolbarModule } from '../../../../@vex/components/secondary-toolbar/secondary-toolbar.module';
import { BreadcrumbsModule } from '../../../../@vex/components/breadcrumbs/breadcrumbs.module';
import { PageLayoutModule } from '../../../../@vex/components/page-layout/page-layout.module';
import { ContainerModule } from '../../../../@vex/directives/container/container.module';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { TranslateModule } from '@ngx-translate/core';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule } from '@angular/material/sort';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatDividerModule } from '@angular/material/divider';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { SharedModuleModule } from '../../shared-module/shared-module.module';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { MatExpansionModule } from '@angular/material/expansion';

import { GradesReportRoutingModule } from './grades-report-routing.module';
import { GradesReportComponent } from './grades-report.component';
import { GradeBreakdownComponent } from './grade-breakdown/grade-breakdown.component';
import { MatRadioModule } from '@angular/material/radio';
import { ClassRankListComponent } from './class-rank-list/class-rank-list.component';
import { MatPaginatorModule } from '@angular/material/paginator';
import { StudentFinalGradesComponent } from './student-final-grades/student-final-grades.component';
import { ProgressReportComponent } from './progress-report/progress-report.component';
import { HonorRollComponent } from './honor-roll/honor-roll.component';


@NgModule({
  declarations: [GradesReportComponent, GradeBreakdownComponent, ProgressReportComponent, ClassRankListComponent, StudentFinalGradesComponent, HonorRollComponent],
  imports: [
    CommonModule,
    GradesReportRoutingModule,
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
    MatPaginatorModule
  ]
})
export class GradesReportModule { }
