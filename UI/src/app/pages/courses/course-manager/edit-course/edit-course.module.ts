import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EditCourseComponent } from './edit-course.component';
import { MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatInputModule } from '@angular/material/input';
import { MatDividerModule } from '@angular/material/divider';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { IconModule } from '@visurel/iconify-angular';
import { FormsModule,ReactiveFormsModule } from '@angular/forms';
import { MatMenuModule } from '@angular/material/menu';
import { MatButtonModule } from '@angular/material/button';
import { MatNativeDateModule } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { TranslateModule } from '@ngx-translate/core';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import {MatSlideToggleModule} from '@angular/material/slide-toggle';
import { MatTooltipModule } from '@angular/material/tooltip';
import { NgxMaskModule } from 'ngx-mask';
import { SharedModuleModule } from '../../../shared-module/shared-module.module';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@NgModule({
  declarations: [EditCourseComponent],
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
    MatDialogModule,
    MatDatepickerModule,
    MatNativeDateModule,
    IconModule,
    FlexLayoutModule,
    MatInputModule,
    MatDividerModule,
    MatMenuModule,
    MatFormFieldModule,
    MatCheckboxModule,
    MatPaginatorModule,
    MatTableModule,
    ReactiveFormsModule,
    TranslateModule,
    MatSelectModule,
    FormsModule,
    MatSnackBarModule,
    MatSlideToggleModule,
    SharedModuleModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    NgxMaskModule.forRoot()
  ]
})
export class EditCourseModule { }
