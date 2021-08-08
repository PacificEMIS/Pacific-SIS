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
import { CommonService } from '../../../../services/common.service';
import { LovAddView } from '../../../../models/lov.model';

@Component({
  selector: 'vex-edit-race',
  templateUrl: './edit-race.component.html',
  styleUrls: ['./edit-race.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms
  ]
})
export class EditRaceComponent implements OnInit {
  icClose = icClose;
  raceAddViewModel: LovAddView = new LovAddView();
  form: FormGroup;
  editMode = false;
  raceTitle: string;

  constructor(
    private dialogRef: MatDialogRef<EditRaceComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public translateService: TranslateService,
    private fb: FormBuilder,
    private snackbar: MatSnackBar,
    private commonService: CommonService) {
    //translateService.use('en');
    this.form = this.fb.group({
      id: [0],
      lovColumnValue: ['', [Validators.required]]
    })

    if (data == null) {
      this.raceTitle = "addRace";
    }
    else {
      this.editMode = true;
      this.raceTitle = "editRace";
    //  this.raceAddViewModel.dropdownValue = data;
      this.form.controls.id.patchValue(data.id)
      this.form.controls.lovColumnValue.patchValue(data.lovColumnValue)

    }

  }

  ngOnInit(): void {
  }
  submit() {
    if (this.form.valid) {
      if (this.form.controls.id.value == 0) {
        this.raceAddViewModel.dropdownValue.lovColumnValue = this.form.controls.lovColumnValue.value;
        this.raceAddViewModel.dropdownValue.lovName = "Race";
        this.commonService.addDropdownValue(this.raceAddViewModel).subscribe(
          (res) => {
            if (typeof (res) == 'undefined') {
              this.snackbar.open('Race Addition failed. ' + sessionStorage.getItem("httpError"), '', {
                duration: 10000
              });
            }
            else {
            if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
                this.snackbar.open('' + res._message, '', {
                  duration: 10000
                });
              }
              else {
                this.snackbar.open('' + res._message, '', {
                  duration: 10000
                });
                this.dialogRef.close('submited');
              }
            }
          }
        );

      }
      else {
        this.raceAddViewModel.dropdownValue.id = this.form.controls.id.value
        this.raceAddViewModel.dropdownValue.lovColumnValue = this.form.controls.lovColumnValue.value;
        this.raceAddViewModel.dropdownValue.lovName = "Race";
        this.commonService.updateDropdownValue(this.raceAddViewModel).subscribe(
          (res) => {
            if (typeof (res) == 'undefined') {
              this.snackbar.open('Race Updation failed. ' + sessionStorage.getItem("httpError"), '', {
                duration: 10000
              });
            }
            else {
            if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
                this.snackbar.open('' + res._message, '', {
                  duration: 10000
                });
              }
              else {
                this.snackbar.open('' + res._message, '', {
                  duration: 10000
                });
                this.dialogRef.close('submited');
              }
            }
          }
        )
      }
    }
  }
  cancel() {
    this.dialogRef.close();
  }
}
