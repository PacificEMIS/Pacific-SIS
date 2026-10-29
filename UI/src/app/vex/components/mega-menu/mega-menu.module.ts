import { NgModule } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { MegaMenuComponent } from './mega-menu.component';
import { MatButtonModule } from '@angular/material/button';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';



@NgModule({
  declarations: [MegaMenuComponent],
  imports: [
    MatIconModule,
    CommonModule,
    MatButtonModule,
    RouterModule,
    TranslateModule
  ],
  exports: [MegaMenuComponent],
  entryComponents: [MegaMenuComponent]
})
export class MegaMenuModule { }
