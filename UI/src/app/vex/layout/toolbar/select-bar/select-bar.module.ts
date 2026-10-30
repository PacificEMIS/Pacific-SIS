import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SelectBarComponent } from './select-bar.component';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
import { MatIconModule } from '@angular/material/icon';
import { MatLegacyMenuModule as MatMenuModule } from '@angular/material/legacy-menu';
import { MatRippleModule } from '@angular/material/core';
import { MatLegacySelectModule as MatSelectModule } from '@angular/material/legacy-select';
import { MatLegacyFormFieldModule as MatFormFieldModule } from '@angular/material/legacy-form-field';
import { ReactiveFormsModule } from '@angular/forms';
import { MatLegacyAutocompleteModule as MatAutocompleteModule } from '@angular/material/legacy-autocomplete';
import { MatLegacyInputModule as MatInputModule } from '@angular/material/legacy-input';
@NgModule({
  declarations: [SelectBarComponent],
  imports: [
    CommonModule,
    NgxMatSelectSearchModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatRippleModule,
    MatSelectModule,
    MatFormFieldModule,
    ReactiveFormsModule,
    MatAutocompleteModule
  ],
 
  exports: [
    MatButtonModule,    
    SelectBarComponent
  ]
})
export class SelectBarModule { }
