import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AdministrationRoutingModule } from './administration-routing.module';
import { AdministrationComponent } from './administration.component';
import { SecondaryToolbarModule } from '../../../../@vex/components/secondary-toolbar/secondary-toolbar.module';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { PageLayoutModule } from '../../../../@vex/components/page-layout/page-layout.module';
import { ContainerModule } from '../../../../@vex/directives/container/container.module';
import { BreadcrumbsModule } from '../../../../@vex/components/breadcrumbs/breadcrumbs.module';
import { TranslateModule } from '@ngx-translate/core';
import { MatDividerModule } from '@angular/material/divider';
import { MatInputModule } from '@angular/material/input';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatMenuModule } from '@angular/material/menu';
import { MatCardModule } from '@angular/material/card';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatSortModule } from '@angular/material/sort';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { IconModule } from '@visurel/iconify-angular';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { SharedModuleModule } from '../../../../app/pages/shared-module/shared-module.module';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatNativeDateModule, MatRippleModule } from '@angular/material/core';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatChipsModule } from '@angular/material/chips';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { EditReportCardGradesDetailsComponent } from './edit-report-card-grades-details/edit-report-card-grades-details.component';
import { HistoricalGradesDetailsComponent } from './historical-grades-details/historical-grades-details.component';
import { SearchStudentComponentForEditReportCardGrades } from './search-student-edit-report-card-grades/search-student.component';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { CourseSectionComponent } from './course-section/course-section.component';
import { HistSearchStudentComponent } from './search-student/search-student.component';
import { NgxMaskModule } from 'ngx-mask';


@NgModule({
    declarations: [AdministrationComponent, EditReportCardGradesDetailsComponent, HistoricalGradesDetailsComponent, SearchStudentComponentForEditReportCardGrades, CourseSectionComponent, HistSearchStudentComponent],
  imports: [
    CommonModule,
    AdministrationRoutingModule,
    SecondaryToolbarModule,
    MatIconModule,
    MatButtonModule,
    PageLayoutModule,
    ContainerModule,
    BreadcrumbsModule,
    TranslateModule,
    MatDividerModule,
    MatInputModule,
    MatDialogModule,
    MatSelectModule,
    MatCheckboxModule,
    MatMenuModule,
    MatCardModule,
    MatPaginatorModule,
    MatTableModule,
    MatButtonToggleModule,
    MatSortModule,
    MatTooltipModule,
    MatDatepickerModule,
    MatFormFieldModule,
    FormsModule,
    ReactiveFormsModule,
    IconModule,
    MatSnackBarModule,
    SharedModuleModule,
    MatProgressSpinnerModule,
    MatExpansionModule,
    MatNativeDateModule,
    MatSlideToggleModule,
    MatRippleModule,
    MatChipsModule,
    MatAutocompleteModule,
    NgxMatSelectSearchModule,
    NgxMaskModule
  ]
})
export class AdministrationModule { }
