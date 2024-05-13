import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { CommonStaffListComponent } from './common-staff-list.component';
import { TranslateModule } from '@ngx-translate/core';
import { MatSortModule } from '@angular/material/sort';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { SecondaryToolbarModule } from 'src/@vex/components/secondary-toolbar/secondary-toolbar.module';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatMenuModule } from '@angular/material/menu';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { IconModule } from '@visurel/iconify-angular';
import { SharedModuleModule } from 'src/app/pages/shared-module/shared-module.module';
import { StaffAdvancedSearchModule } from '../staff-advanced-search/staff-advanced-search.module';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';



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
    IconModule,
    SharedModuleModule,
    StaffAdvancedSearchModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatSlideToggleModule
  ],
  exports:[CommonStaffListComponent]
})
export class CommonStaffListModule { }
