import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SessionExpireAlertComponent } from '../session-expire/session-expire-alert/session-expire-alert.component';
import { MatLegacyDialogModule as MatDialogModule} from '@angular/material/legacy-dialog';
import {MatLegacyButtonModule as MatButtonModule} from '@angular/material/legacy-button';
import { TranslateModule } from '@ngx-translate/core';
import { MatLegacyProgressSpinnerModule as MatProgressSpinnerModule } from '@angular/material/legacy-progress-spinner';
import { MatIconModule } from '@angular/material/icon';

@NgModule({
  declarations: [SessionExpireAlertComponent],
  imports: [
    CommonModule,
    MatDialogModule,
    MatButtonModule,
    TranslateModule,
    MatProgressSpinnerModule,
    MatIconModule
  ]
})
export class SessionExpireModule { }
