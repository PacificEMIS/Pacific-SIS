import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MyClassesRoutingModule } from './my-classes-routing.module';
import { MyClassesComponent } from './my-classes.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModuleModule } from '../../shared-module/shared-module.module';
import { TranslateModule } from '@ngx-translate/core';
import { MatLegacyCardModule as MatCardModule } from '@angular/material/legacy-card';
import { ContainerModule } from '@vex/directives/container/container.module';
import { PageLayoutModule } from '@vex/components/page-layout/page-layout.module';
import { BreadcrumbsModule } from '@vex/components/breadcrumbs/breadcrumbs.module';
import { SecondaryToolbarModule } from '@vex/components/secondary-toolbar/secondary-toolbar.module';
import { CourseManagerRoutingModule } from '../course-manager/course-manager-routing.module';
import { CourseSectionModule } from '../course-manager/course-section/course-section.module';
import { MatIconModule } from '@angular/material/icon';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { MatLegacySnackBarModule as MatSnackBarModule } from '@angular/material/legacy-snack-bar';
import { MatLegacySelectModule as MatSelectModule } from '@angular/material/legacy-select';
import { MatLegacyCheckboxModule as MatCheckboxModule } from '@angular/material/legacy-checkbox';
import { MatLegacyMenuModule as MatMenuModule } from '@angular/material/legacy-menu';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatLegacyTooltipModule as MatTooltipModule } from '@angular/material/legacy-tooltip';
import { MatLegacySlideToggleModule as MatSlideToggleModule } from '@angular/material/legacy-slide-toggle';
import { MatLegacyTableModule as MatTableModule } from '@angular/material/legacy-table';
import { MatLegacyPaginatorModule as MatPaginatorModule } from '@angular/material/legacy-paginator';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatDividerModule } from '@angular/material/divider';
@NgModule({
  declarations: [MyClassesComponent],
  imports: [
    CommonModule,
    MyClassesRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModuleModule,
    TranslateModule,
    ContainerModule,
    PageLayoutModule,
    BreadcrumbsModule,
    SecondaryToolbarModule,
    CourseManagerRoutingModule,
    CourseSectionModule,
    MatCardModule,
    MatSnackBarModule,
    MatSelectModule,
    MatCheckboxModule,
    MatIconModule,
    MatButtonModule,
    MatMenuModule,
    MatButtonToggleModule,
    MatTooltipModule,
    MatSlideToggleModule,
    MatTableModule,
    MatPaginatorModule,
    MatExpansionModule,
    MatDividerModule
  ]
})
export class MyClassesModule { }
