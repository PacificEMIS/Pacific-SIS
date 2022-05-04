import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StaffAdvancedSearchComponent } from './staff-advanced-search.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatNativeDateModule, MatRippleModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDividerModule } from '@angular/material/divider';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TranslateModule } from '@ngx-translate/core';
import { IconModule } from '@visurel/iconify-angular';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { BreadcrumbsModule } from 'src/@vex/components/breadcrumbs/breadcrumbs.module';
import { PageLayoutModule } from 'src/@vex/components/page-layout/page-layout.module';
import { SecondaryToolbarModule } from 'src/@vex/components/secondary-toolbar/secondary-toolbar.module';
import { ContainerModule } from 'src/@vex/directives/container/container.module';
import { SharedModuleModule } from 'src/app/pages/shared-module/shared-module.module';
import { StaffinfoRoutingModule } from 'src/app/pages/staff/staffinfo/staffinfo-routing-module';



@NgModule({
  declarations: [StaffAdvancedSearchComponent],
  imports: [
    CommonModule,
    StaffinfoRoutingModule,
    SecondaryToolbarModule,
    BreadcrumbsModule,
    MatIconModule,
    MatButtonModule,
    PageLayoutModule,
    ContainerModule,
    IconModule,
    MatCardModule,
    MatSnackBarModule,
    MatSelectModule,
    MatCheckboxModule,
    MatPaginatorModule,
    MatTableModule,
    MatMenuModule,
    MatButtonModule,
    MatButtonToggleModule,
    SharedModuleModule,
    TranslateModule,
    FormsModule,
    ReactiveFormsModule,
    MatSortModule,
    MatTooltipModule,
    MatProgressSpinnerModule,
    MatExpansionModule,
    MatInputModule,
    MatDividerModule,
    NgxMatSelectSearchModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatFormFieldModule,
    MatSlideToggleModule,
    MatRippleModule,
  ],exports:[StaffAdvancedSearchComponent]
})
export class StaffAdvancedSearchModule { }
