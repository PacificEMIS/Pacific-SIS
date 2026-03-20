import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { FillAttendanceRoutingModule } from './fill-attendance-routing.module';
import { FillAttendanceComponent } from './fill-attendance.component';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { SecondaryToolbarModule } from '../../../../@vex/components/secondary-toolbar/secondary-toolbar.module';
import { BreadcrumbsModule } from '../../../../@vex/components/breadcrumbs/breadcrumbs.module';
import { PageLayoutModule } from '../../../../@vex/components/page-layout/page-layout.module';
import { TranslateModule } from '@ngx-translate/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { MatDialogModule } from '@angular/material/dialog';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [FillAttendanceComponent],
  imports: [
    CommonModule,
    FillAttendanceRoutingModule,
    MatIconModule,
    MatCardModule,
    SecondaryToolbarModule,
    BreadcrumbsModule,
    PageLayoutModule,
    TranslateModule,
    FlexLayoutModule,
    MatFormFieldModule,
    MatSelectModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatTableModule,
    MatDialogModule,
    FormsModule
  ]
})
export class FillAttendanceModule { }
