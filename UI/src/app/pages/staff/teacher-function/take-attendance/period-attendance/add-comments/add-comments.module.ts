import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AddCommentsComponent } from './add-comments.component';
import { MatLegacyDialogModule as MatDialogModule } from '@angular/material/legacy-dialog';
import { MatIconModule } from '@angular/material/icon';
import { TranslateModule } from '@ngx-translate/core';
import { MatDividerModule } from '@angular/material/divider';
import {MatLegacyButtonModule as MatButtonModule} from '@angular/material/legacy-button';
import { MatLegacyInputModule as MatInputModule } from '@angular/material/legacy-input';
import { FormsModule } from '@angular/forms';
import { MatLegacyTooltipModule as MatTooltipModule } from '@angular/material/legacy-tooltip';



@NgModule({
  declarations: [AddCommentsComponent],
  imports: [
    CommonModule,
    MatDialogModule,
    MatIconModule,
    TranslateModule,
    MatDividerModule,
    MatButtonModule,
    MatInputModule,
    FormsModule,
    MatTooltipModule
  ]
})
export class AddCommentsModule { }
