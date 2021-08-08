/***********************************************************************************
openSIS is a free student information system for public and non-public
schools from Open Solutions for Education, Inc.Website: www.os4ed.com.

Visit the openSIS product website at https://opensis.com to learn more.
If you have question regarding this software or the license, please contact
via the website.

The software is released under the terms of the GNU Affero General Public License as
published by the Free Software Foundation, version 3 of the License.
See https://www.gnu.org/licenses/agpl-3.0.en.html.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

Copyright (c) Open Solutions for Education, Inc.

All rights reserved.
***********************************************************************************/

import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import icClose from '@iconify/icons-ic/twotone-close';
import { fadeInUp400ms } from '../../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../../@vex/animations/stagger.animation';
import { TranslateService } from '@ngx-translate/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SchoolPeriodService } from '../../../../services/school-period.service';
import { ValidationService } from '../../../shared/validation.service';
import { BlockPeriodAddViewModel } from '../../../../models/school-period.model';
import { getTime } from 'date-fns';
import { SharedFunction } from '../../../shared/shared-function';
import { DefaultValuesService } from '../../../../common/default-values.service';
import { CommonService } from 'src/app/services/common.service';

export const dateTimeCustomFormats = {
};

@Component({
  selector: 'vex-edit-period',
  templateUrl: './edit-period.component.html',
  styleUrls: ['./edit-period.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms
  ]
})
export class EditPeriodComponent implements OnInit {
  icClose = icClose;
  form: FormGroup;
  buttonType: string;
  periodHeaderTitle: string;
  currentBlockId: number = null;
  blockPeriodAddViewModel: BlockPeriodAddViewModel = new BlockPeriodAddViewModel();

  constructor(private dialogRef: MatDialogRef<EditPeriodComponent>,
    public commonfunction: SharedFunction,
    public translateService: TranslateService,
    public defaultValuesService: DefaultValuesService,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private fb: FormBuilder,
    private snackbar: MatSnackBar,
    private schoolPeriodService: SchoolPeriodService,
    private commonService: CommonService,
    ) {
    //translateService.use('en');
    this.form = fb.group({
      periodId: [0],
      title: ['', [Validators.required, ValidationService.noWhitespaceValidator]],
      shortName: ['', [Validators.required, ValidationService.noWhitespaceValidator]],
      startTime: ['', [Validators.required]],
      endTime: [, [Validators.required]],
      calculateAttendance: [false]
    })
    if (data.periodData == null) {
      this.currentBlockId = data.blockId;
      this.periodHeaderTitle = "addNewPeriod";
      this.buttonType = "submit";
    }
    else {
      this.buttonType = "update";
      this.periodHeaderTitle = "editPeriod";
      this.blockPeriodAddViewModel.blockPeriod = data.periodData;
      this.currentBlockId = data.periodData.blockId;
      this.form.controls.periodId.patchValue(data.periodData.periodId);
      this.form.controls.title.patchValue(data.periodData.periodTitle);
      this.form.controls.shortName.patchValue(data.periodData.periodShortName);
      this.form.controls.startTime.patchValue(new Date(data.periodData.periodStartTime));
      this.form.controls.endTime.patchValue(new Date(data.periodData.periodEndTime));
      this.form.controls.calculateAttendance.patchValue(data.periodData.calculateAttendance);
    }

  }

  ngOnInit(): void {
  }

  submit() {
    this.form.markAllAsTouched();
    if (this.form.valid) {
      if (this.form.controls.startTime.value.toString().substr(16, 5) === this.form.controls.endTime.value.toString().substr(16, 5)) {
        this.snackbar.open(this.defaultValuesService.translateKey('startTimeAndEndTimeCanNotBeSame'), '', {
          duration: 10000
        });
      }
      else {
        if (this.form.controls.periodId.value == 0) {
          this.blockPeriodAddViewModel.blockPeriod.blockId = this.currentBlockId;
          this.blockPeriodAddViewModel.blockPeriod.periodTitle = this.form.controls.title.value;
          this.blockPeriodAddViewModel.blockPeriod.calculateAttendance = this.form.controls.calculateAttendance.value;
          this.blockPeriodAddViewModel.blockPeriod.periodShortName = this.form.controls.shortName.value;
          this.blockPeriodAddViewModel.blockPeriod.periodStartTime = this.form.controls.startTime.value.toString().substr(16, 5);
          this.blockPeriodAddViewModel.blockPeriod.periodEndTime = this.form.controls.endTime.value.toString().substr(16, 5);
          this.schoolPeriodService.addBlockPeriod(this.blockPeriodAddViewModel).subscribe(
            (res: BlockPeriodAddViewModel) => {
              if (typeof (res) == 'undefined') {
                this.snackbar.open('Period Creation failed. ' + sessionStorage.getItem("httpError"), '', {
                  duration: 10000
                });
              }
              else {
              if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
                  this.snackbar.open(res._message, '', {
                    duration: 10000
                  });
                }
                else {
                  this.snackbar.open(res._message, '', {
                    duration: 10000
                  });
                  this.dialogRef.close('submited');
                }
              }
            }
          )

        }
        else {
          this.blockPeriodAddViewModel.blockPeriod.periodId = this.form.controls.periodId.value;
          this.blockPeriodAddViewModel.blockPeriod.blockId = this.currentBlockId;
          this.blockPeriodAddViewModel.blockPeriod.periodTitle = this.form.controls.title.value;
          this.blockPeriodAddViewModel.blockPeriod.calculateAttendance = this.form.controls.calculateAttendance.value;
          this.blockPeriodAddViewModel.blockPeriod.periodShortName = this.form.controls.shortName.value;
          this.blockPeriodAddViewModel.blockPeriod.periodStartTime = this.form.controls.startTime.value.toString().substr(16, 5);
          this.blockPeriodAddViewModel.blockPeriod.periodEndTime = this.form.controls.endTime.value.toString().substr(16, 5);
          this.schoolPeriodService.updateBlockPeriod(this.blockPeriodAddViewModel).subscribe(
            (res: BlockPeriodAddViewModel) => {
              if (typeof (res) == 'undefined') {
                this.snackbar.open('Period Updation failed. ' + sessionStorage.getItem("httpError"), '', {
                  duration: 10000
                });
              }
              else {
              if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
                  this.snackbar.open(res._message, '', {
                    duration: 10000
                  });
                }
                else {
                  this.snackbar.open(res._message + '', '', {
                    duration: 10000
                  });
                  this.dialogRef.close('submited');
                }
              }
            }
          );
        }

      }
    }
  }

}
