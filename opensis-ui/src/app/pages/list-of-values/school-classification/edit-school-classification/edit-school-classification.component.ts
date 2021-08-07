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

import { Component, OnInit,Inject } from '@angular/core';
import { MatDialogRef,MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder,FormGroup, Validators } from '@angular/forms';
import icClose from '@iconify/icons-ic/twotone-close';
import { fadeInUp400ms } from '../../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../../@vex/animations/stagger.animation';
import {LovAddView} from '../../../../models/lov.model';
import {CommonService} from '../../../../services/common.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { ValidationService } from 'src/app/pages/shared/validation.service';
@Component({
  selector: 'vex-edit-school-classification',
  templateUrl: './edit-school-classification.component.html',
  styleUrls: ['./edit-school-classification.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms
  ]
})
export class EditSchoolClassificationComponent implements OnInit {
  icClose = icClose;
  lovModel = new LovAddView();
  form: FormGroup;
  classificationModalTitle:string;
  classificationModalActionTitle:string;
  editMode = false;
  constructor(
    private dialogRef: MatDialogRef<EditSchoolClassificationComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any,
    public translateService: TranslateService,
    private fb: FormBuilder,
    private snackbar: MatSnackBar,
    private commonService: CommonService) {
    //translateService.use('en');
    this.form=fb.group({
      id:[0],
      lovName:["School Classification"],
      lovColumnValue:['',[ValidationService.noWhitespaceValidator]],
    })
    if (data == null) {
      this.classificationModalTitle = "addSchoolClassification";
      this.classificationModalActionTitle = "submit";
    }
    else {
      this.editMode = true;
      this.classificationModalTitle = "editSchoolClassification";
      this.classificationModalActionTitle="update";
    //  this.raceAddViewModel.dropdownValue = data;
      this.form.controls.id.patchValue(data.id)
      this.form.controls.lovColumnValue.patchValue(data.lovColumnValue);
      this.form.controls.lovName.patchValue(data.lovName)

    }

  }
  ngOnInit(): void {}

 
  
  submit() {    
    if (this.form.valid) {   
      if (this.form.controls.id.value == 0) {
        this.lovModel.dropdownValue.lovColumnValue = this.form.controls.lovColumnValue.value;
        this.lovModel.dropdownValue.lovName = "School Classification";
        this.commonService.addDropdownValue(this.lovModel).subscribe(data => {
          if (typeof (data) == 'undefined') {
            this.snackbar.open('Classification Submission failed. ' + sessionStorage.getItem("httpError"), '', {
              duration: 10000
            });
          }
          else {
           if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
              this.snackbar.open('' + data._message, '', {
                duration: 10000
              });
            } else {
              this.snackbar.open(''+ data._message, '', {
                duration: 10000
              });              
              this.dialogRef.close(true);
            }
          }
  
        })
      }else{
        this.lovModel.dropdownValue.id = this.form.controls.id.value
        this.lovModel.dropdownValue.lovColumnValue = this.form.controls.lovColumnValue.value;
        this.lovModel.dropdownValue.lovName = "School Classification";
        this.commonService.updateDropdownValue(this.lovModel).subscribe(data => {
          if (typeof (data) == 'undefined') {
            this.snackbar.open('Classification Updation failed. ' + sessionStorage.getItem("httpError"), '', {
              duration: 10000
            });
          }
          else {
           if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
              this.snackbar.open(''+ data._message, '', {
                duration: 10000
              });
            } else {
              this.snackbar.open(''+ data._message, '', {
                duration: 10000
              });              
              this.dialogRef.close(true);
            }
          }
  
        })
      }    
      
    }
      
  }

}
