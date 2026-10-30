import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatIconModule } from '@angular/material/icon';
import { MatLegacyCardModule as MatCardModule } from '@angular/material/legacy-card';
import { MatSidenavModule } from '@angular/material/sidenav';
import { SecondaryToolbarModule } from '@vex/components/secondary-toolbar/secondary-toolbar.module';
import { TranslateModule } from '@ngx-translate/core';
import { MatLegacyTableModule as MatTableModule } from '@angular/material/legacy-table';
import { MatLegacyPaginatorModule as MatPaginatorModule } from '@angular/material/legacy-paginator';
import { MatDividerModule } from '@angular/material/divider';
import { MatSortModule } from '@angular/material/sort';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatLegacyProgressSpinnerModule as MatProgressSpinnerModule } from '@angular/material/legacy-progress-spinner';
import { StaffRoutingModule } from './staff-routing-module';
import { BreadcrumbsModule } from '@vex/components/breadcrumbs/breadcrumbs.module';
import { PageLayoutModule } from '@vex/components/page-layout/page-layout.module';
import { ContainerModule } from '@vex/directives/container/container.module';
import { ScrollbarModule } from '@vex/components/scrollbar/scrollbar.module';
import { SharedModuleModule } from '../shared-module/shared-module.module';
import { GradeDetailsModule } from './teacher-function/input-final-grade/grade-details/grade-details.module';
import { EffortGradeDetailsModule } from './teacher-function/input-effort-grades/effort-grade-details/effort-grade-details.module';


@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    StaffRoutingModule,
    MatIconModule,
    MatCardModule,
    MatSidenavModule,
    SecondaryToolbarModule,
    BreadcrumbsModule,
    PageLayoutModule,
    ContainerModule,
    TranslateModule,
    ScrollbarModule,
    MatTableModule,
    MatPaginatorModule,
    MatDividerModule,
    GradeDetailsModule,
    EffortGradeDetailsModule,
    MatSortModule,
    FormsModule,
    ReactiveFormsModule,
    MatProgressSpinnerModule,
    SharedModuleModule
  ],
})
export class StaffModule { }
