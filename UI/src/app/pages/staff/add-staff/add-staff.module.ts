import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AddStaffComponent } from './add-staff.component';
import { MatIconModule } from '@angular/material/icon';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { MatLegacyFormFieldModule as MatFormFieldModule } from '@angular/material/legacy-form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatLegacyRadioModule as MatRadioModule } from '@angular/material/legacy-radio';
import { DateAdapter, MatNativeDateModule } from '@angular/material/core';
import { MatLegacyCardModule as MatCardModule } from '@angular/material/legacy-card';
import { MatLegacySnackBarModule as MatSnackBarModule } from '@angular/material/legacy-snack-bar';
import { SecondaryToolbarModule } from '@vex/components/secondary-toolbar/secondary-toolbar.module';
import { BreadcrumbsModule } from '@vex/components/breadcrumbs/breadcrumbs.module';
import { PageLayoutModule } from '@vex/components/page-layout/page-layout.module';
import { ContainerModule } from '@vex/directives/container/container.module';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatLegacyInputModule as MatInputModule } from '@angular/material/legacy-input';
import { MatLegacySelectModule as MatSelectModule } from '@angular/material/legacy-select';
import { MatLegacyProgressSpinnerModule as MatProgressSpinnerModule } from '@angular/material/legacy-progress-spinner';
import { TranslateModule } from '@ngx-translate/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModuleModule } from '../../shared-module/shared-module.module';
import { MatLegacyCheckboxModule as MatCheckboxModule } from '@angular/material/legacy-checkbox';
import { MatExpansionModule } from '@angular/material/expansion';
import { ScrollbarModule } from '@vex/components/scrollbar/scrollbar.module';
import { MatLegacyMenuModule as MatMenuModule } from '@angular/material/legacy-menu';
import { MatLegacyPaginatorModule as MatPaginatorModule } from '@angular/material/legacy-paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatLegacyTableModule as MatTableModule } from '@angular/material/legacy-table';
import { MatLegacyTooltipModule as MatTooltipModule } from '@angular/material/legacy-tooltip';
import { MatLegacySlideToggleModule as MatSlideToggleModule } from '@angular/material/legacy-slide-toggle';
import { StaffGeneralinfoComponent } from './staff-generalinfo/staff-generalinfo.component';
import { StaffSchoolinfoComponent } from './staff-schoolinfo/staff-schoolinfo.component';
import { StaffLogininfoComponent } from './staff-logininfo/staff-logininfo.component';
import { StaffAddressinfoComponent } from './staff-addressinfo/staff-addressinfo.component';
import { StaffCertificationinfoComponent } from './staff-certificationinfo/staff-certificationinfo.component';
import { ViewstaffCertificationinfoComponent } from './viewstaff-certificationinfo/viewstaff-certificationinfo.component';
import { CustomFieldModule } from '../../../../../src/app/common/custom-field/custom-field.module';
import { CustomFieldWithoutFormModule } from '../../../../../src/app/common/custom-field-without-form/custom-field-without-form.module';
import { ViewStaffGeneralinfoComponent } from './view-staff-generalinfo/view-staff-generalinfo.component';
import { ViewStaffAddressinfoComponent } from './view-staff-addressinfo/view-staff-addressinfo.component';
import { AddStaffRoutingModule } from './add-staff-routing-module';
import { MatLegacyDialogModule as MatDialogModule } from '@angular/material/legacy-dialog';
import { StaffCourseScheduleComponent } from './staff-course-schedule/staff-course-schedule.component';
import { CalendarModule as AngularCalendarModule } from 'angular-calendar';
import { MatDividerModule } from '@angular/material/divider';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { MatChipModule } from '../../shared-module/mat-chip/mat-chip.module';
import { AddCertificateComponent } from './staff-certificationinfo/add-certificate/add-certificate.component';



@NgModule({
  declarations: [
    AddStaffComponent,
    StaffGeneralinfoComponent,
    StaffSchoolinfoComponent,
    StaffLogininfoComponent,
    StaffAddressinfoComponent,
    StaffCertificationinfoComponent,
    ViewstaffCertificationinfoComponent,
    ViewStaffGeneralinfoComponent,
    ViewStaffAddressinfoComponent,
    StaffCourseScheduleComponent,
    AddCertificateComponent
  ],
  imports: [
    CommonModule,
    AngularCalendarModule,
    AddStaffRoutingModule,
    MatIconModule,
    MatButtonModule,
    MatFormFieldModule,
    MatDatepickerModule,
    MatRadioModule,
    MatNativeDateModule,
    MatCardModule,
    MatSnackBarModule,
    SecondaryToolbarModule,
    BreadcrumbsModule,
    PageLayoutModule,
    ContainerModule,
    MatSidenavModule,
    MatInputModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    TranslateModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModuleModule,
    MatCheckboxModule,
    MatExpansionModule,
    ScrollbarModule,
    MatMenuModule,
    MatPaginatorModule,
    MatSortModule,
    NgxMatSelectSearchModule,
    MatTableModule,
    MatTooltipModule,
    MatSlideToggleModule,
    CustomFieldModule,
    MatChipModule,
    CustomFieldWithoutFormModule,
    MatDialogModule,
    MatDividerModule
  ]
})
export class AddStaffModule { }
