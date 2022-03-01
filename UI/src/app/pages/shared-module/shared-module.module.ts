import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSpinnerOverlayComponent } from './mat-spinner-overlay/mat-spinner-overlay.component';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import { ProfileImageComponent } from './profile-image/profile-image.component';
import { ImageCropperModule } from 'ngx-image-cropper';
import {MatSnackBarModule} from '@angular/material/snack-bar';
import { MatDialogModule} from '@angular/material/dialog';
import {MatButtonModule} from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { IconModule } from '@visurel/iconify-angular';
import { PhoneMaskDirective } from './directives/phone-mask.directive';
import { EmtyBooleanCheckPipe } from './user-define-pipe/emty-boolean-check-pipe';
import { EmtyValueCheckPipe } from './user-define-pipe/emty-value-check.pipe';
import {EmtyNumberCheckPipe} from './user-define-pipe/emty-number-check.pipe';
import { ConfirmDialogComponent } from './confirm-dialog/confirm-dialog.component';
import { MatCardModule } from '@angular/material/card';
import { TranslateModule } from '@ngx-translate/core';
import {InvalidControlScrollDirective} from './user-defined-directives/invalid-control-scroll.directive';
import {EmtyBooleanCheckReversePipe} from './user-define-pipe/emty-boolean-check.reverse.pipe';
import { SafePipe } from './user-define-pipe/safeHtml.pipe';
import { TransformDateTimePipe } from './user-define-pipe/transform-datetime-pipe';
import {EmailvalidatorDirective} from './user-defined-directives/emailvalidator.directive';
import {PhonevalidatorDirective} from './user-defined-directives/phonevalidator.directive';
import { AgePipe } from './user-define-pipe/age-calculator.pipe';
import { TransformTimePipe } from './user-define-pipe/transfrom-time.pipe';
import { EvenOddPipe } from './user-define-pipe/even-odd.pipe';
import { WeekDayPipe } from './user-define-pipe/number-to-week-day.pipe';
import { NgForFilterPipe } from './user-define-pipe/course-section-ngFor-div-filter.pipe';
import { Transform24to12Pipe } from './user-define-pipe/transform-24to12.pipe';
import { CourseNgForFilterPipe } from './user-define-pipe/course-ngFor-div-filter.pipe';
import { SystemCategoryCheckPipe } from './user-define-pipe/system-category-check.pipe';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatTooltipModule } from '@angular/material/tooltip';
import { FlexLayoutModule } from '@angular/flex-layout';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { NgxImageCompressService } from 'ngx-image-compress';
import { DataEditInfoComponent } from './data-edit-info/data-edit-info.component';
import { MatDividerModule } from '@angular/material/divider';
import { youtubeLinkValidatorDirective } from './user-defined-directives/youtube-link-validator.directive';
import { TwitterLinkValidatorDirective } from './user-defined-directives/twitter-link-validator.directive';
import { WebsiteLinkValidatorDirective } from './user-defined-directives/website-link-validator.directive';
import { LinkedinLinkValidatorDirective } from './user-defined-directives/linkedin-link-validator.directive';
import { InstagramLinkValidatorDirective } from './user-defined-directives/instagram-link-validator.directive';
import { FacebookLinkValidatorDirective } from './user-defined-directives/facebook-link-validator.directive';
import { SsnMaskPipe } from './user-define-pipe/ssn-mask.pipe';
import { InputEffortGradesNgForDivFilterPipe } from './user-define-pipe/input-effort-grades-ng-for-div-filter.pipe';
import { CustomMinDirective } from './user-defined-directives/custom-min.directive';
import { PasswordMaskPipe } from './user-define-pipe/password-mask.pipe';


@NgModule({
  declarations: [MatSpinnerOverlayComponent, ProfileImageComponent,PhoneMaskDirective,EmtyBooleanCheckPipe,EmtyBooleanCheckReversePipe,
    EmtyValueCheckPipe,EmtyNumberCheckPipe, ConfirmDialogComponent,InvalidControlScrollDirective,TransformDateTimePipe,TransformTimePipe,EmailvalidatorDirective,
    PhonevalidatorDirective,SafePipe,AgePipe,EvenOddPipe,WeekDayPipe,NgForFilterPipe,Transform24to12Pipe,CourseNgForFilterPipe,SystemCategoryCheckPipe, ResetPasswordComponent,
    DataEditInfoComponent,
    youtubeLinkValidatorDirective,
    TwitterLinkValidatorDirective,
    WebsiteLinkValidatorDirective,
    LinkedinLinkValidatorDirective,
    InstagramLinkValidatorDirective,
    FacebookLinkValidatorDirective,
    SsnMaskPipe,
    InputEffortGradesNgForDivFilterPipe,
    CustomMinDirective,
    PasswordMaskPipe],
  imports: [
    CommonModule,
    MatProgressSpinnerModule,
    ImageCropperModule,
    MatSnackBarModule,
    MatDialogModule,
    MatButtonModule,
    MatIconModule,
    IconModule,
    MatCardModule,
    TranslateModule,
    MatFormFieldModule,
    MatInputModule,
    FlexLayoutModule,
    ReactiveFormsModule,
    MatTooltipModule,
    MatDividerModule
  ],
  exports:[MatSpinnerOverlayComponent, ProfileImageComponent,PhoneMaskDirective,EmtyBooleanCheckPipe,EmtyValueCheckPipe,EmtyNumberCheckPipe,InvalidControlScrollDirective,
    EmtyBooleanCheckReversePipe,TransformDateTimePipe,TransformTimePipe,EmailvalidatorDirective,PhonevalidatorDirective,SafePipe,AgePipe,EvenOddPipe,WeekDayPipe,NgForFilterPipe,Transform24to12Pipe,CourseNgForFilterPipe,SystemCategoryCheckPipe,
    youtubeLinkValidatorDirective,TwitterLinkValidatorDirective,WebsiteLinkValidatorDirective,LinkedinLinkValidatorDirective,InstagramLinkValidatorDirective,FacebookLinkValidatorDirective,SsnMaskPipe,InputEffortGradesNgForDivFilterPipe,CustomMinDirective, PasswordMaskPipe],
    providers: [NgxImageCompressService]
})
export class SharedModuleModule { }
