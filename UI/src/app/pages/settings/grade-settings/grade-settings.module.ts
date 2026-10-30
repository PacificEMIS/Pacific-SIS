import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GradeSettingsComponent } from './grade-settings.component';
import { MatIconModule } from '@angular/material/icon';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { MatLegacyFormFieldModule as MatFormFieldModule } from '@angular/material/legacy-form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatLegacyCardModule as MatCardModule } from '@angular/material/legacy-card';
import { MatLegacySnackBarModule as MatSnackBarModule } from '@angular/material/legacy-snack-bar';
import { SecondaryToolbarModule } from '@vex/components/secondary-toolbar/secondary-toolbar.module';
import { BreadcrumbsModule } from '@vex/components/breadcrumbs/breadcrumbs.module';
import { PageLayoutModule } from '@vex/components/page-layout/page-layout.module';
import { ContainerModule } from '@vex/directives/container/container.module';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatLegacyInputModule as MatInputModule } from '@angular/material/legacy-input';
import { MatLegacySelectModule as MatSelectModule } from '@angular/material/legacy-select';
import { TranslateModule } from '@ngx-translate/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {  RouterModule } from '@angular/router';
import { MatLegacyTableModule as MatTableModule } from '@angular/material/legacy-table';
import { StandardGradeSetupComponent } from '../../grades/standard-grade-setup/standard-grade-setup.component';
import { UsCommonCoreStandardsComponent } from '../../grades/us-common-core-standards/us-common-core-standards.component';
import { SchoolSpecificStandardsComponent } from '../../grades/school-specific-standards/school-specific-standards.component';
import { EffortGradeSetupComponent } from '../../grades/effort-grade-setup/effort-grade-setup.component';
import { EffortGradeLibraryComponent } from '../../grades/effort-grade-library/effort-grade-library.component';
import { EffortGradeScaleComponent } from '../../grades/effort-grade-scale/effort-grade-scale.component';
import { ReportCardGradesComponent } from '../../grades/report-card-grades/report-card-grades.component';
import { HonorRollSetupComponent } from '../../grades/honor-roll-setup/honor-roll-setup.component';
import { MatLegacyDialogModule as MatDialogModule } from '@angular/material/legacy-dialog';
import { MatLegacyMenuModule as MatMenuModule } from '@angular/material/legacy-menu';
import { MatLegacyCheckboxModule as MatCheckboxModule } from '@angular/material/legacy-checkbox';
import { MatLegacyPaginatorModule as MatPaginatorModule } from '@angular/material/legacy-paginator';
import { MatSortModule } from '@angular/material/sort';
import { SharedModuleModule } from '../../../pages/shared-module/shared-module.module';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { ReportCardCommentsComponent } from '../../grades/report-card-comments/report-card-comments.component';
import { StandardGradesComponent } from '../../grades/standard-grades/standard-grades.component';
// import { ReportCardsComponent } from '../../grades/report-cards/report-cards.component';
import { MatLegacySlideToggleModule as MatSlideToggleModule } from '@angular/material/legacy-slide-toggle';
// import { TranscriptsComponent } from '../../grades/transcripts/transcripts.component';
import { MatDividerModule } from '@angular/material/divider';
import { NgxDocViewerModule } from 'ngx-doc-viewer';
import { MatExpansionModule } from '@angular/material/expansion';
// import { SearchStudentComponent } from '../../grades/search-student/search-student.component';
import {MatLegacyProgressSpinnerModule as MatProgressSpinnerModule} from '@angular/material/legacy-progress-spinner';
import { HistoricalMarkingPeriodsComponent } from '../../grades/historical-marking-periods/historical-marking-periods.component';
import { MatLegacyTooltipModule as MatTooltipModule } from '@angular/material/legacy-tooltip';




@NgModule({
  declarations: [
    GradeSettingsComponent,
    StandardGradeSetupComponent,
    UsCommonCoreStandardsComponent,
    SchoolSpecificStandardsComponent,
    EffortGradeSetupComponent,
    EffortGradeLibraryComponent,
    EffortGradeScaleComponent,
    ReportCardGradesComponent,
    HonorRollSetupComponent,
    ReportCardCommentsComponent,
    StandardGradesComponent,
    // ReportCardsComponent,
    // TranscriptsComponent,
    // SearchStudentComponent
    HistoricalMarkingPeriodsComponent
  ],
  imports: [
    CommonModule,
    MatIconModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,   
    MatCardModule,
    MatSidenavModule,
    MatSnackBarModule,
    MatFormFieldModule,
    SecondaryToolbarModule,
    BreadcrumbsModule,
    PageLayoutModule,
    ContainerModule,
    MatDatepickerModule,
    MatNativeDateModule,
    TranslateModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    SharedModuleModule,
    MatDialogModule,
    MatMenuModule,
    MatSortModule,
    MatCheckboxModule,
    MatButtonToggleModule,
    DragDropModule,
    MatSlideToggleModule,
    MatDividerModule,
    NgxDocViewerModule,
    MatExpansionModule,
    MatProgressSpinnerModule,
    MatTooltipModule
  ]
})
export class GradeSettingsModule { }
