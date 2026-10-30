import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatLegacyPaginatorModule as MatPaginatorModule } from '@angular/material/legacy-paginator';
import { MatLegacyTableModule as MatTableModule } from '@angular/material/legacy-table';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { CommonStaffListComponent } from './common-staff-list.component';
import { TranslateModule } from '@ngx-translate/core';
import { MatSortModule } from '@angular/material/sort';
import { MatLegacyCardModule as MatCardModule } from '@angular/material/legacy-card';
import { MatLegacyProgressSpinnerModule as MatProgressSpinnerModule } from '@angular/material/legacy-progress-spinner';
import { SecondaryToolbarModule } from '@vex/components/secondary-toolbar/secondary-toolbar.module';
import { MatLegacyTooltipModule as MatTooltipModule } from '@angular/material/legacy-tooltip';
import { MatLegacyMenuModule as MatMenuModule } from '@angular/material/legacy-menu';
import { MatLegacyCheckboxModule as MatCheckboxModule } from '@angular/material/legacy-checkbox';
import { SharedModuleModule } from 'src/app/pages/shared-module/shared-module.module';
import { StaffAdvancedSearchModule } from '../staff-advanced-search/staff-advanced-search.module';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatLegacySlideToggleModule as MatSlideToggleModule } from '@angular/material/legacy-slide-toggle';



@NgModule({
  declarations: [CommonStaffListComponent],
  imports: [
    CommonModule,
    TranslateModule,
    FormsModule,
    ReactiveFormsModule,
    MatPaginatorModule,
    MatTableModule,
    MatIconModule,
    MatSortModule,
    MatCardModule,
    MatProgressSpinnerModule,
    SecondaryToolbarModule,
    MatTooltipModule,
    MatMenuModule,
    MatCheckboxModule,
    SharedModuleModule,
    StaffAdvancedSearchModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatSlideToggleModule
  ],
  exports:[CommonStaffListComponent]
})
export class CommonStaffListModule { }
