import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AdministrationSettingsComponent } from './administration-settings.component';
import { MatIconModule } from '@angular/material/icon';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { MatLegacyFormFieldModule as MatFormFieldModule } from '@angular/material/legacy-form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatLegacyCardModule as MatCardModule } from '@angular/material/legacy-card';
import { MatLegacySnackBarModule as MatSnackBarModule } from '@angular/material/legacy-snack-bar';
import { SecondaryToolbarModule } from '@vex/components/secondary-toolbar/secondary-toolbar.module';
import { BreadcrumbsModule } from '@vex/components/breadcrumbs/breadcrumbs.module';
import { PageLayoutModule } from '@vex/components/page-layout/page-layout.module';
import { ContainerModule } from '@vex/directives/container/container.module';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatLegacyInputModule as MatInputModule } from '@angular/material/legacy-input';
import { MatLegacySelectModule as MatSelectModule } from '@angular/material/legacy-select';
import { TranslateModule } from '@ngx-translate/core';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {  RouterModule } from '@angular/router';

import { MatLegacyTableModule as MatTableModule } from '@angular/material/legacy-table';
import { AccessControlComponent } from '../../administration/access-control/access-control.component';
import { MatLegacyDialogModule as MatDialogModule } from '@angular/material/legacy-dialog';
import { MatLegacyMenuModule as MatMenuModule } from '@angular/material/legacy-menu';
import { MatLegacyCheckboxModule as MatCheckboxModule } from '@angular/material/legacy-checkbox';
import { ScrollbarModule } from '@vex/components/scrollbar/scrollbar.module';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatLegacySlideToggleModule as MatSlideToggleModule } from '@angular/material/legacy-slide-toggle';
import { SharedModuleModule } from '../../shared-module/shared-module.module';
import { MatLegacyTooltipModule as MatTooltipModule } from '@angular/material/legacy-tooltip';
import { SuperAdministratorsComponent } from '../../administration/super-administrators/super-administrators.component';
import { AddSuperAdministratorComponent } from '../../administration/super-administrators/add-super-administrator/add-super-administrator.component';
import { PromoteStaffComponent } from '../../administration/super-administrators/promote-staff/promote-staff.component';
import { DemoteSuperAdministratorComponent } from '../../administration/super-administrators/demote-super-administrator/demote-super-administrator.component';



@NgModule({
  declarations: [
    AdministrationSettingsComponent,
    AccessControlComponent,
    SuperAdministratorsComponent,
    AddSuperAdministratorComponent,
    PromoteStaffComponent,
    DemoteSuperAdministratorComponent
  ],
  imports: [
    CommonModule,
    MatIconModule,
    MatInputModule,
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
    TranslateModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule,
    MatTableModule,
    MatSlideToggleModule,
    MatDialogModule,
    MatMenuModule,
    MatCheckboxModule,
    ScrollbarModule,
    MatExpansionModule,
    MatSlideToggleModule,
    SharedModuleModule,
    MatTooltipModule
  ]
})
export class AdministrationSettingsModule { }
