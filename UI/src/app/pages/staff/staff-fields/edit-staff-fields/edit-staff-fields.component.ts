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

import { Component, ElementRef, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import icClose from '@iconify/icons-ic/twotone-close';
import { fadeInUp400ms } from '../../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../../@vex/animations/stagger.animation';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CustomFieldService } from '../../../../services/custom-field.service';
import {CustomFieldAddView} from '../../../../models/custom-field.model';
import { CustomFieldOptionsEnum } from '../../../../enums/custom-field-options.enum';
import { FieldCategoryModuleEnum } from '../../../../enums/field-category-module.enum'
import { ValidationService } from '../../../shared/validation.service';
import { CommonService } from 'src/app/services/common.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';

@Component({
  selector: 'vex-edit-staff-fields',
  templateUrl: './edit-staff-fields.component.html',
  styleUrls: ['./edit-staff-fields.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms
  ]
})
export class EditStaffFieldsComponent implements OnInit {
  icClose = icClose;
  form: FormGroup;
  customFieldTitle;
  buttonType;
  checkSearchRecord: number = 0;
  fieldCategoryModule=FieldCategoryModuleEnum
  customFieldOptionsEnum=Object.values(CustomFieldOptionsEnum).sort();
  customFieldAddView:CustomFieldAddView= new CustomFieldAddView()
  formfieldcheck=['Dropdown','Editable Dropdown','Multiple SelectBox']
  currentCategoryid;

  constructor(
    private dialogRef: MatDialogRef<EditStaffFieldsComponent>, 
    private fb: FormBuilder, 
    @Inject(MAT_DIALOG_DATA) public data:any,
    private snackbar:MatSnackBar,
    private customFieldService:CustomFieldService,
    private commonService: CommonService,
    private defaultValuesService: DefaultValuesService,
    private el: ElementRef
    ) {
      this.form=fb.group({
        fieldId:[0],
        title:['',[ValidationService.noWhitespaceValidator]],
        fieldType:['',[Validators.required]],
        selectOptions:[''],
        defaultSelection:[''],
        required:[false],
        hide:[false],
        systemField:[false],
        isSystemWideField: [false]
      })
      if(data.information==null){
        this.currentCategoryid=data.categoryID
        this.customFieldTitle="addCustomField";
        this.buttonType="submit";
      }
      else{
        this.buttonType="update";
        this.customFieldTitle="editCustomField";
        this.customFieldAddView.customFields=data.information;
        this.form.controls.fieldId.patchValue(data.information.fieldId);
        this.form.controls.title.patchValue(data.information.title);
        this.form.controls.selectOptions.patchValue(data.information.selectOptions.replaceAll("|","\n"));
        this.form.controls.defaultSelection.patchValue(data.information.defaultSelection);
        this.form.controls.required.patchValue(data.information.required);
        this.form.controls.fieldType.patchValue(data.information.type);
        this.form.controls.hide.patchValue(data.information.hide);
        this.form.controls.systemField.patchValue(data.information.systemField);
        this.form.controls.isSystemWideField.patchValue(data.information.isSystemWideField);
        if(data.information.type === "Checkbox") {
          this.form.controls.defaultSelection.setValidators([Validators.required]);
          this.form.controls.defaultSelection.updateValueAndValidity();
        }
        if(data.information.type !== "Checkbox") {
          this.form.controls.defaultSelection.clearValidators();
          this.form.controls.defaultSelection.updateValueAndValidity();
        }
        if(data.information.isSystemWideField){
          this.form.get('isSystemWideField').disable();
        }
      }
     }

  ngOnInit(): void {
  }

  scrollToInvalidControl() {
    if (this.form.controls.title.invalid) {
      const invalidTitleControl: HTMLElement = this.el.nativeElement.querySelector('.fieldName-scroll');
      invalidTitleControl.scrollIntoView({ behavior: 'smooth', block: 'center' });
    } else if (this.form.controls.fieldType.invalid) {
      const invalidFieldTypeControl: HTMLElement = this.el.nativeElement.querySelector('.fieldType-scroll');
      invalidFieldTypeControl.scrollIntoView({ behavior: 'smooth', block: 'center' });
    }
  }

  submit(){
    this.scrollToInvalidControl();
    if(this.form.valid){
      this.checkSearchRecord = 1;
      if(this.form.controls.fieldId.value==0){
        this.customFieldAddView.customFields.categoryId=this.currentCategoryid;
        this.customFieldAddView.customFields.fieldId=this.form.controls.fieldId.value;
        this.customFieldAddView.customFields.title=this.form.controls.title.value;
        this.customFieldAddView.customFields.selectOptions=this.form.controls.selectOptions.value.replaceAll("\n","|");
        this.customFieldAddView.customFields.defaultSelection=this.form.controls.defaultSelection.value;
        this.customFieldAddView.customFields.required=this.form.controls.required.value;
        this.customFieldAddView.customFields.hide=this.form.controls.hide.value;
        this.customFieldAddView.customFields.systemField = false;
        this.customFieldAddView.customFields.isSystemWideField = this.form.controls.isSystemWideField.value;
        this.customFieldAddView.customFields.type=this.form.controls.fieldType.value;
        this.customFieldAddView.customFields.module=this.fieldCategoryModule.Staff;
         this.customFieldService.addCustomField(this.customFieldAddView).subscribe(
          (res:CustomFieldAddView)=>{
            if(typeof(res)=='undefined'){
              this.snackbar.open('Staff field failed. ' + this.defaultValuesService.getHttpError(), '', {
                duration: 10000
              });
              this.checkSearchRecord = 0;
            }
            else{
            if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
                this.snackbar.open( res._message, '', {
                  duration: 10000
                });
                this.checkSearchRecord = 0;
              } 
              else { 
                this.snackbar.open(res._message, '', {
                  duration: 10000
                }); 
                this.checkSearchRecord = 0;
                this.dialogRef.close('submited');
              }
            }
          }
        ); 
      }
      else{
        this.customFieldAddView.customFields.fieldId=this.form.controls.fieldId.value;
        this.customFieldAddView.customFields.title=this.form.controls.title.value;
        this.customFieldAddView.customFields.type=this.form.controls.fieldType.value;
        if((this.form.controls.fieldType.value===this.formfieldcheck[0])||(this.form.controls.fieldType.value===this.formfieldcheck[1])||(this.form.controls.fieldType.value===this.formfieldcheck[2])){
            this.customFieldAddView.customFields.selectOptions=this.form.controls.selectOptions.value.replaceAll("\n","|");
          }
          else{
            this.customFieldAddView.customFields.selectOptions="";
          }
        this.customFieldAddView.customFields.defaultSelection=this.form.controls.defaultSelection.value;
        this.customFieldAddView.customFields.required=this.form.controls.required.value;
        this.customFieldAddView.customFields.hide=this.form.controls.hide.value;
        this.customFieldAddView.customFields.systemField = false;
        this.customFieldAddView.customFields.isSystemWideField = this.form.controls.isSystemWideField.value;
        this.customFieldService.updateCustomField(this.customFieldAddView).subscribe(
          (res:CustomFieldAddView)=>{
            if(typeof(res)=='undefined'){
              this.snackbar.open('Staff field failed. ' + this.defaultValuesService.getHttpError(), '', {
                duration: 10000
              });
              this.checkSearchRecord = 0;
            }
            else{
            if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
                this.snackbar.open( res._message, '', {
                  duration: 10000
                });
                this.checkSearchRecord = 0;
              } 
              else {
                this.snackbar.open( res._message, '', {
                  duration: 10000
                }); 
                this.checkSearchRecord = 0;
                this.dialogRef.close('submited');
              }
            }
          }
        );
      }
    } else {
      this.form.markAllAsTouched();
    }
  }
  
  checkFieldType(event) {
    let selectedValue;
    if (selectedValue != event.value) {
      this.form.controls.defaultSelection.setValue('');
      if (event.value === "Checkbox") {
        this.form.controls.defaultSelection.setValidators([Validators.required]);
        this.form.controls.defaultSelection.updateValueAndValidity();
      } else {
        this.form.controls.defaultSelection.clearValidators();
        this.form.controls.defaultSelection.updateValueAndValidity();
      }
    }
    selectedValue = event.value;
  }
  
}
