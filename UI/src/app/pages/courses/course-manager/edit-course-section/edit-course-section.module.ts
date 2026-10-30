import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CalendarModule as AngularCalendarModule, DateAdapter } from 'angular-calendar';
import { adapterFactory } from 'angular-calendar/date-adapters/date-fns';
import { MatLegacyDialogModule as MatDialogModule } from '@angular/material/legacy-dialog';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { MatIconModule } from '@angular/material/icon';
import { MatLegacySelectModule as MatSelectModule } from '@angular/material/legacy-select';
import { MatLegacySnackBarModule as MatSnackBarModule } from '@angular/material/legacy-snack-bar';
import { MatLegacyInputModule as MatInputModule } from '@angular/material/legacy-input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { FormsModule,ReactiveFormsModule } from '@angular/forms';
import { MatNativeDateModule } from '@angular/material/core';
import { MatLegacyMenuModule as MatMenuModule } from '@angular/material/legacy-menu';
import { TranslateModule } from '@ngx-translate/core';
import { EditCourseSectionComponent } from './edit-course-section.component';
import { MatDividerModule } from '@angular/material/divider';
import { MatLegacyFormFieldModule as MatFormFieldModule } from '@angular/material/legacy-form-field';
import { MatLegacyCheckboxModule as MatCheckboxModule } from '@angular/material/legacy-checkbox';
import {MatLegacySlideToggleModule as MatSlideToggleModule} from '@angular/material/legacy-slide-toggle';
import { MatLegacyRadioModule as MatRadioModule } from '@angular/material/legacy-radio';
import { FixedSchedulingComponent } from './fixed-scheduling/fixed-scheduling.component';
import { VariableSchedulingComponent } from './variable-scheduling/variable-scheduling.component';
import { CalendarDaysComponent } from './calendar-days/calendar-days.component';
import { RotatingSchedulingComponent } from './rotating-scheduling/rotating-scheduling.component';
import { SharedModuleModule } from '../../../../pages/shared-module/shared-module.module';
import { NgxMaskDirective, NgxMaskPipe } from 'ngx-mask';
@NgModule({
  declarations: [EditCourseSectionComponent, FixedSchedulingComponent, VariableSchedulingComponent, CalendarDaysComponent, RotatingSchedulingComponent],
  imports: [
    CommonModule,
    NgxMaskDirective, NgxMaskPipe,
    AngularCalendarModule.forRoot({
      provide: DateAdapter,
      useFactory: adapterFactory
    }),
    MatIconModule,
    MatButtonModule,
    MatDialogModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatInputModule,
    MatDividerModule,
    MatMenuModule,
    MatFormFieldModule,
    MatCheckboxModule,
    ReactiveFormsModule,
    TranslateModule,
    MatSelectModule,
    FormsModule,
    MatSnackBarModule,
    MatSlideToggleModule,
    MatRadioModule,
    SharedModuleModule
  ]
})
export class EditCourseSectionModule { }
