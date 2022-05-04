import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { GradebookGradesRoutingModule } from './gradebook-grades-routing.module';
import { GradebookGradesComponent } from './gradebook-grades.component'
import { RouterModule } from '@angular/router';
import { SecondaryToolbarModule } from 'src/@vex/components/secondary-toolbar/secondary-toolbar.module';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { PageLayoutModule } from 'src/@vex/components/page-layout/page-layout.module';
import { ContainerModule } from 'src/@vex/directives/container/container.module';
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
import { SharedModuleModule } from 'src/app/pages/shared-module/shared-module.module';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatNativeDateModule, MatRippleModule } from '@angular/material/core';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatChipsModule } from '@angular/material/chips';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { AddGradeCommentsComponent } from './add-grade-comments/add-grade-comments.component';
import { GradebookGradeListComponent } from './gradebook-grade-list/gradebook-grade-list.component';
import { GradebookGradeDetailsComponent } from './gradebook-grade-details/gradebook-grade-details.component';
import { CommonStaffListModule } from 'src/app/common/common-staff-list/common-staff-list.module';


@NgModule({
  declarations: [GradebookGradesComponent, AddGradeCommentsComponent, GradebookGradeListComponent, GradebookGradeDetailsComponent],
  imports: [
    CommonModule,
    GradebookGradesRoutingModule,
    RouterModule,
    SecondaryToolbarModule,
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
    CommonStaffListModule

  ]
})
export class GradebookGradesModule { }
