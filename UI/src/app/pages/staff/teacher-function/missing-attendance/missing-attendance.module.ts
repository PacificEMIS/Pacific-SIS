import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MissingAttendanceRoutingModule } from './missing-attendance-routing.module';
import { MissingAttendanceComponent } from './missing-attendance.component';
import { MissingAttendanceDetailsComponent } from './missing-attendance-details/missing-attendance-details.component';
import { TakeAttendanceComponent } from './take-attendance/take-attendance.component';
import { AddTeacherCommentsComponent } from './take-attendance/add-teacher-comments/add-teacher-comments.component';
import { SecondaryToolbarModule } from "@vex/components/secondary-toolbar/secondary-toolbar.module";
import { BreadcrumbsModule } from "@vex/components/breadcrumbs/breadcrumbs.module";
import { MatIconModule } from "@angular/material/icon";
import { MatLegacyButtonModule as MatButtonModule } from "@angular/material/legacy-button";
import { PageLayoutModule } from "@vex/components/page-layout/page-layout.module";
import { ContainerModule } from "@vex/directives/container/container.module";
import { TranslateModule } from "@ngx-translate/core";
import { MatDividerModule } from "@angular/material/divider";
import { MatLegacyInputModule as MatInputModule } from "@angular/material/legacy-input";
import { MatLegacyDialogModule as MatDialogModule } from "@angular/material/legacy-dialog";
import { MatLegacySelectModule as MatSelectModule } from "@angular/material/legacy-select";
import { MatLegacyCheckboxModule as MatCheckboxModule } from "@angular/material/legacy-checkbox";
import { MatLegacyMenuModule as MatMenuModule } from "@angular/material/legacy-menu";
import { MatLegacyCardModule as MatCardModule } from "@angular/material/legacy-card";
import { MatLegacyPaginatorModule as MatPaginatorModule } from "@angular/material/legacy-paginator";
import { MatLegacyTableModule as MatTableModule } from "@angular/material/legacy-table";
import { MatButtonToggleModule } from "@angular/material/button-toggle";
import { MatSortModule } from "@angular/material/sort";
import { MatLegacyTooltipModule as MatTooltipModule } from "@angular/material/legacy-tooltip";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { MatLegacyFormFieldModule as MatFormFieldModule } from "@angular/material/legacy-form-field";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { MatLegacySlideToggleModule as MatSlideToggleModule } from "@angular/material/legacy-slide-toggle";
import { SharedModuleModule } from "../../../shared-module/shared-module.module";


@NgModule({
  declarations: [MissingAttendanceComponent, MissingAttendanceDetailsComponent, TakeAttendanceComponent, AddTeacherCommentsComponent],
  imports: [
    MissingAttendanceRoutingModule,
    CommonModule,
    SecondaryToolbarModule,
    BreadcrumbsModule,
    MatIconModule,
    MatButtonModule,
    PageLayoutModule,
    ContainerModule,
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
    RouterModule,
    MatSlideToggleModule,
    SharedModuleModule
  ]
})
export class MissingAttendanceModule { }
