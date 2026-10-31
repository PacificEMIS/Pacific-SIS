import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AddCommentsComponent } from './add-comments.component';
import { MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { TranslateModule } from '@ngx-translate/core';
import { MatDividerModule } from '@angular/material/divider';
import {MatButtonModule} from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { MatTooltipModule } from '@angular/material/tooltip';



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
