import { NgModule } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { CommonModule } from '@angular/common';
import { MegaMenuComponent } from './mega-menu.component';
import { MatLegacyButtonModule as MatButtonModule } from '@angular/material/legacy-button';
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
    exports: [MegaMenuComponent]
})
export class MegaMenuModule { }
