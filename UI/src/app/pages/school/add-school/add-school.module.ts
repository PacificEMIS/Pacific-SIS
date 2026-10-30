import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AddSchoolComponent } from './add-school.component';
import { MatIconModule } from '@angular/material/icon';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { MatLegacyFormFieldModule as MatFormFieldModule } from '@angular/material/legacy-form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatLegacyProgressSpinnerModule as MatProgressSpinnerModule } from '@angular/material/legacy-progress-spinner';
import { MatLegacyCardModule as MatCardModule } from '@angular/material/legacy-card';
import { MatLegacySnackBarModule as MatSnackBarModule } from '@angular/material/legacy-snack-bar';
import { SecondaryToolbarModule } from '@vex/components/secondary-toolbar/secondary-toolbar.module';
import { BreadcrumbsModule } from '@vex/components/breadcrumbs/breadcrumbs.module';
import { PageLayoutModule } from '@vex/components/page-layout/page-layout.module';
import { ContainerModule } from '@vex/directives/container/container.module';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatLegacyInputModule as MatInputModule } from '@angular/material/legacy-input';
import { MatLegacyCheckboxModule as MatCheckboxModule } from '@angular/material/legacy-checkbox';
import { MatLegacySelectModule as MatSelectModule } from '@angular/material/legacy-select';
import{GeneralInfoComponent} from '../add-school/general-info/general-info.component';
import{WashInfoComponent} from '../add-school/wash-info/wash-info.component';
import { TranslateModule } from '@ngx-translate/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModuleModule } from '../../shared-module/shared-module.module';
import { CustomFieldModule } from "../../../common/custom-field/custom-field.module";
import { CustomFieldWithoutFormModule } from "../../../common/custom-field-without-form/custom-field-without-form.module";
import { ViewGeneralInfoComponent } from './view-general-info/view-general-info.component';
import { ViewWashInfoComponent } from './view-wash-info/view-wash-info.component';
import { ScrollbarModule } from '@vex/components/scrollbar/scrollbar.module';
import { AddCopySchoolComponent } from './add-copy-school/add-copy-school.component';
import { MatDividerModule } from '@angular/material/divider';
import {MatLegacyDialogModule as MatDialogModule} from '@angular/material/legacy-dialog';
import {MatLegacySlideToggleModule as MatSlideToggleModule} from '@angular/material/legacy-slide-toggle';
import { SuccessCopySchoolComponent } from './success-copy-school/success-copy-school.component';
import { AddSchoolRoutingModule } from './add-school-routing-module';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { CaptureDateComponent } from './capture-date/capture-date.component';


@NgModule({
  declarations: [
    AddSchoolComponent,
    GeneralInfoComponent,
    WashInfoComponent,
    ViewGeneralInfoComponent,
    ViewWashInfoComponent,
    AddCopySchoolComponent,
    SuccessCopySchoolComponent,
    CaptureDateComponent],
  imports: [
    CommonModule,
    AddSchoolRoutingModule,
    MatIconModule,
    AddSchoolRoutingModule,
    MatInputModule,
    MatCheckboxModule,
    MatSelectModule,
    MatButtonModule,
    MatCardModule,
    MatSidenavModule,
    MatSnackBarModule,
    MatFormFieldModule,
    SecondaryToolbarModule,
    BreadcrumbsModule,
    PageLayoutModule,
    ContainerModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatProgressSpinnerModule,
    TranslateModule,
    ReactiveFormsModule,
    FormsModule,
    SharedModuleModule,
    CustomFieldModule,
    CustomFieldWithoutFormModule,
    ScrollbarModule,
    MatDividerModule,
    MatDialogModule,
    NgxMatSelectSearchModule,
    MatSlideToggleModule
  ]
})
export class AddSchoolModule { }
