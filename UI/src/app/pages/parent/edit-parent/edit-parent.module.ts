import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EditParentComponent } from './edit-parent.component';
import { MatIconModule } from '@angular/material/icon';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { MatLegacyFormFieldModule as MatFormFieldModule } from '@angular/material/legacy-form-field';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatLegacyRadioModule as MatRadioModule } from '@angular/material/legacy-radio';
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
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModuleModule } from '../../shared-module/shared-module.module';
import { MatLegacyCheckboxModule as MatCheckboxModule } from '@angular/material/legacy-checkbox';
import { MatExpansionModule } from '@angular/material/expansion';
import { ScrollbarModule } from '@vex/components/scrollbar/scrollbar.module';
import { MatLegacyMenuModule as MatMenuModule } from '@angular/material/legacy-menu';
import { MatLegacyPaginatorModule as MatPaginatorModule } from '@angular/material/legacy-paginator';
import { MatSortModule } from '@angular/material/sort';
//import { MatButtonToggle, MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatLegacyTableModule as MatTableModule } from '@angular/material/legacy-table';
import { MatLegacyTooltipModule as MatTooltipModule } from '@angular/material/legacy-tooltip';
import { MatLegacySlideToggleModule as MatSlideToggleModule } from '@angular/material/legacy-slide-toggle';
//import { ViewparentGeneralinfoComponent } from './viewparent-generalinfo/viewparent-generalinfo.component';
import { ViewparentAddressinfoComponent } from './viewparent-addressinfo/viewparent-addressinfo.component';
import { ViewparentSchoolaccessComponent } from './viewparent-schoolaccess/viewparent-schoolaccess.component';
import { EditparentGeneralinfoComponent } from './editparent-generalinfo/editparent-generalinfo.component';
import { EditparentAddressinfoComponent } from './editparent-addressinfo/editparent-addressinfo.component';
import { EditparentSchoolaccessComponent } from './editparent-schoolaccess/editparent-schoolaccess.component';
import { EditParentRoutingModule } from './edit-parent-routing-module';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';



@NgModule({
  declarations: [
    EditParentComponent,
   // ViewparentGeneralinfoComponent,
    ViewparentAddressinfoComponent,
    ViewparentSchoolaccessComponent,
    EditparentGeneralinfoComponent,
    EditparentAddressinfoComponent,
    EditparentSchoolaccessComponent
  ],
  imports: [
    CommonModule,
    MatIconModule,
    EditParentRoutingModule,
    MatButtonModule,
    MatFormFieldModule,
    MatDatepickerModule,
    MatRadioModule,
    MatNativeDateModule,
    MatCardModule,
    MatSnackBarModule,
    SecondaryToolbarModule,
    BreadcrumbsModule,
    PageLayoutModule,
    ContainerModule,
    MatSidenavModule,
    MatInputModule,
    MatSelectModule,
    TranslateModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModuleModule,
    MatCheckboxModule,
    MatExpansionModule,
    ScrollbarModule,
    MatMenuModule,
    MatPaginatorModule,
    MatSortModule,
    //MatButtonToggle,
    //MatButtonToggleModule,
    MatTableModule,
    MatTooltipModule,
    NgxMatSelectSearchModule,
    MatSlideToggleModule
  ]
})
export class EditParentModule { }
