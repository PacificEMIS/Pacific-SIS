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
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import icClose from '@iconify/icons-ic/twotone-close';
import { LovAddView } from '../../../../models/lov.model';
import { ValidationService } from '../../../shared/validation.service';
import { CommonService } from '../../../../services/common.service';
import { fadeInUp400ms } from '../../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../../@vex/animations/stagger.animation';
import { DefaultValuesService } from 'src/app/common/default-values.service';

@Component({
  selector: 'vex-edit-male-toilet-accessibility',
  templateUrl: './edit-male-toilet-accessibility.component.html',
  styleUrls: ['./edit-male-toilet-accessibility.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms
  ]
})
export class EditMaleToiletAccessibilityComponent implements OnInit {
  icClose = icClose;
  form:FormGroup
  editmod;
  maleToiletAccessibilityTitle: string;
  buttonType: string;
  lovAddView:LovAddView=new LovAddView();
  constructor(
    private dialogRef: MatDialogRef<EditMaleToiletAccessibilityComponent>,
    @Inject(MAT_DIALOG_DATA) public data:any,
    private snackbar:MatSnackBar,
    private commonService:CommonService,
    fb:FormBuilder,
    private defaultValuesService: DefaultValuesService
    ) {
      this.editmod=data.mod
      this.form=fb.group({
        id:[0],
        lovName:["Male Toilet Accessibility"],
        lovColumnValue:['',[ValidationService.noWhitespaceValidator]],
      })
     }

  ngOnInit(): void {
    if(this.editmod===1){
      this.maleToiletAccessibilityTitle="editMaleToiletAccessibility";
      this.buttonType="update";
      this.patchDataToForm();
    }
    else{
      this.maleToiletAccessibilityTitle="addNewMaleToiletAccessibility";
      this.buttonType="submit";
    }
  }
  patchDataToForm() {
    this.form.controls.id.patchValue(this.data.element.id)
    this.form.controls.lovName.patchValue(this.data.element.lovName)
    this.form.controls.lovColumnValue.patchValue(this.data.element.lovColumnValue)
  }
  submit(){
    this.form.markAllAsTouched();
    if(this.form.valid){
      if(this.editmod==0){
        this.addMaleToiletAccessibility()
      }
      else{
        this.editMaleToiletAccessibility()
      }
    }
  }
  editMaleToiletAccessibility() {
    this.lovAddView.dropdownValue.id=this.form.controls.id.value;
    this.lovAddView.dropdownValue.lovName=this.form.controls.lovName.value;
    this.lovAddView.dropdownValue.lovColumnValue=this.form.controls.lovColumnValue.value;
    this.commonService.updateDropdownValue(this.lovAddView).subscribe(
      (res:LovAddView)=>{
        if(typeof(res)=='undefined'){
          this.snackbar.open( this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
        else{
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open( res._message, '', {
              duration: 10000
            });
          } 
          else{
            this.snackbar.open( res._message, '', {
              duration: 10000
            });
            this.dialogRef.close('submited');
          }
        }
      }
    );
  }
  addMaleToiletAccessibility() {
    this.lovAddView.dropdownValue.lovColumnValue=this.form.controls.lovColumnValue.value;
    this.lovAddView.dropdownValue.lovName=this.form.controls.lovName.value;
    this.commonService.addDropdownValue(this.lovAddView).subscribe(
      (res:LovAddView)=>{
        if(typeof(res)=='undefined'){
          this.snackbar.open( this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
        else{
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open( res._message, '', {
              duration: 10000
            });
          } 
          else{
            this.snackbar.open( res._message, '', {
              duration: 10000
            });
            this.dialogRef.close('submited');
          }
        }
      }
    );
  }

}
