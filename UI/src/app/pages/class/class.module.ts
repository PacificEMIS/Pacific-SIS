import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ClassRoutingModule } from './class-routing.module';
import { ClassComponent } from './class.component';
import { MatIconModule } from '@angular/material/icon';
import { MatLegacyTooltipModule as MatTooltipModule } from '@angular/material/legacy-tooltip';
import { SecondaryToolbarModule } from '@vex/components/secondary-toolbar/secondary-toolbar.module';
import { BreadcrumbsModule } from '@vex/components/breadcrumbs/breadcrumbs.module';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { PageLayoutModule } from '@vex/components/page-layout/page-layout.module';
import { ContainerModule } from '@vex/directives/container/container.module';
import { TranslateModule } from '@ngx-translate/core';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatDividerModule } from '@angular/material/divider';
import { MatLegacyFormFieldModule as MatFormFieldModule } from '@angular/material/legacy-form-field';
import { MatLegacyInputModule as MatInputModule } from '@angular/material/legacy-input';
import { CourseOverviewComponent } from './course-overview/course-overview.component';
import { StudentsComponent } from './students/students.component';
import { AttendanceComponent } from './attendance/attendance.component';
import { MatLegacyTableModule as MatTableModule } from "@angular/material/legacy-table";
import { MatLegacySlideToggleModule as MatSlideToggleModule } from "@angular/material/legacy-slide-toggle";
import { MissingAttendanceListComponent } from './attendance/missing-attendance-list/missing-attendance-list.component';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatLegacyDialogModule as MatDialogModule } from '@angular/material/legacy-dialog';
import { AddTeacherCommentsComponent } from './attendance/add-teacher-comments/add-teacher-comments.component';
import { FormsModule } from "@angular/forms";
import { ReactiveFormsModule } from '@angular/forms';
import { MatMomentDateModule } from '@angular/material-moment-adapter';
import { MatLegacyPaginatorModule as MatPaginatorModule } from '@angular/material/legacy-paginator';
import { AssignmentsComponent } from './assignments/assignments.component';
import { AddAssignmentComponent } from './assignments/add-assignment/add-assignment.component';
import { CreateAssignmentComponent } from './assignments/create-assignment/create-assignment.component';
import { MatLegacySelectModule as MatSelectModule } from '@angular/material/legacy-select';
import { QuillModule } from 'ngx-quill';
import { DeleteAssignmentsComponent } from './assignments/delete-assignments/delete-assignments.component';
import { MatLegacyMenuModule as MatMenuModule } from '@angular/material/legacy-menu';
import { GradebookGradesComponent } from './grades/gradebook-grades/gradebook-grades.component';
import { AddGradeCommentsComponent } from './grades/gradebook-grades/add-grade-comments/add-grade-comments.component';
import { GradebookGradeDetailsComponent } from './grades/gradebook-grades/gradebook-grade-details/gradebook-grade-details.component';
import { GradesComponent } from './grades/grades.component';
import { InputFinalGradesComponent } from './grades/input-final-grades/input-final-grades.component';
import { SharedModuleModule } from '../shared-module/shared-module.module';
import { MatSortModule } from '@angular/material/sort';
import {ClipboardModule} from '@angular/cdk/clipboard';
import { MatLegacyCheckboxModule as MatCheckboxModule } from '@angular/material/legacy-checkbox';
import { MatLegacyChipsModule as MatChipsModule } from '@angular/material/legacy-chips';
import { GradebookConfigurationComponent } from './gradebook-configuration/gradebook-configuration.component';
import { MatLegacyRadioModule as MatRadioModule } from '@angular/material/legacy-radio';
import { ViewAssignmentDetailsComponent } from './assignments/view-assignment-details/view-assignment-details.component';
import { CopyAssignmentComponent } from './assignments/copy-assignment/copy-assignment.component';
import { MatLegacyAutocompleteModule as MatAutocompleteModule } from '@angular/material/legacy-autocomplete';
import { MatNativeDateModule } from '@angular/material/core';

@NgModule({
  declarations: [ClassComponent, CourseOverviewComponent, StudentsComponent, AttendanceComponent, MissingAttendanceListComponent, AddTeacherCommentsComponent, AssignmentsComponent, AddAssignmentComponent, CreateAssignmentComponent, DeleteAssignmentsComponent, GradebookGradesComponent, AddGradeCommentsComponent, GradebookGradeDetailsComponent, GradesComponent, InputFinalGradesComponent, GradebookConfigurationComponent, ViewAssignmentDetailsComponent, CopyAssignmentComponent],
  imports: [
    CommonModule,
    ClassRoutingModule,
    MatIconModule,
    ClipboardModule,
    MatTooltipModule,
    SecondaryToolbarModule,
    BreadcrumbsModule,
    MatButtonModule,
    PageLayoutModule,
    ContainerModule,
    TranslateModule,
    MatCheckboxModule,
    MatExpansionModule,
    MatDividerModule,
    MatFormFieldModule,
    MatInputModule,
    MatTableModule,
    MatSlideToggleModule,
    MatDatepickerModule,
    MatDialogModule,
    FormsModule,
    SharedModuleModule,
    ReactiveFormsModule,
    MatMomentDateModule,
    MatPaginatorModule,
    MatSortModule,
    MatSelectModule,
    MatMenuModule,
    MatChipsModule,
    MatNativeDateModule,
    MatRadioModule,
    MatAutocompleteModule,
    QuillModule.forRoot({
      modules: {
        toolbar: [
          ['bold', 'italic', 'underline', 'strike'],        // toggled buttons
          ['blockquote', 'code-block'],

          [{ header: 1 }, { header: 2 }],               // custom button values
          [{ list: 'ordered' }, { list: 'bullet' }],
          [{ script: 'sub' }, { script: 'super' }],      // superscript/subscript
          [{ indent: '-1' }, { indent: '+1' }],          // outdent/indent
          [{ direction: 'rtl' }],                         // text direction

          [{ size: ['small', false, 'large', 'huge'] }],  // custom dropdown
          [{ header: [1, 2, 3, 4, 5, 6, false] }],

          [{ color: [] }, { background: [] }],          // dropdown with defaults from theme
          [{ align: [] }],

          ['clean'],                                         // remove formatting button

          ['link', 'image', 'video']                         // link and image, video
        ]
      }
    })
  ]
})
export class ClassModule { }
