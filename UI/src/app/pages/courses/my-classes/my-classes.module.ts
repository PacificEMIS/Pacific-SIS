import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MyClassesRoutingModule } from './my-classes-routing.module';
import { MyClassesComponent } from './my-classes.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModuleModule } from '../../shared-module/shared-module.module';
import { FlexLayoutModule } from '@angular/flex-layout';
import { TranslateModule } from '@ngx-translate/core';
import { MatCardModule } from '@angular/material/card';
import { ContainerModule } from 'src/@vex/directives/container/container.module';
import { PageLayoutModule } from 'src/@vex/components/page-layout/page-layout.module';
import { BreadcrumbsModule } from 'src/@vex/components/breadcrumbs/breadcrumbs.module';
import { SecondaryToolbarModule } from 'src/@vex/components/secondary-toolbar/secondary-toolbar.module';
import { CourseManagerRoutingModule } from '../course-manager/course-manager-routing.module';
import { CourseSectionModule } from '../course-manager/course-section/course-section.module';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { IconModule } from '@visurel/iconify-angular';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
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
    FlexLayoutModule,
    TranslateModule,
    ContainerModule,
    PageLayoutModule,
    BreadcrumbsModule,
    SecondaryToolbarModule,
    CourseManagerRoutingModule,
    CourseSectionModule,
    IconModule,
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
