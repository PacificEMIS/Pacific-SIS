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
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import icClose from '@iconify/icons-ic/twotone-close';
import { fadeInUp400ms } from '../../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../../@vex/animations/stagger.animation';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CustomFieldService } from '../../../../services/custom-field.service';
import {FieldsCategoryAddView} from '../../../../models/fields-category.model';
import {FieldCategoryModuleEnum} from '../../../../enums/field-category-module.enum';
import { ValidationService } from 'src/app/pages/shared/validation.service';
import { DefaultValuesService } from '../../../../common/default-values.service';
import { CommonService } from 'src/app/services/common.service';

@Component({
  selector: 'vex-student-fields-category',
  templateUrl: './student-fields-category.component.html',
  styleUrls: ['./student-fields-category.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms
  ]
})
export class StudentFieldsCategoryComponent implements OnInit {
  icClose = icClose;
  form: FormGroup;
  FieldCategoryTitle: string;
  buttonType: string;
  fieldsCategoryAddView: FieldsCategoryAddView = new FieldsCategoryAddView();
  fieldCategoryModuleEnum = FieldCategoryModuleEnum;

  constructor(
    private dialogRef: MatDialogRef<StudentFieldsCategoryComponent>,
    private fb: FormBuilder,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private snackbar: MatSnackBar,
    private defaultValuesService: DefaultValuesService,
    private customFieldService: CustomFieldService,
    private commonService: CommonService,
     ) {
      this.form = fb.group({
        categoryId: [0],
        title: ['', [Validators.required, ValidationService.noWhitespaceValidator]],
        sortOrder: ['', [Validators.required, Validators.min(1)]]
      });
      if (data === null){
        this.FieldCategoryTitle = 'addFieldCategory';
        this.buttonType = 'submit';
      }
      else{
        this.FieldCategoryTitle = 'editFieldCategory';
        this.buttonType = 'update';
        this.fieldsCategoryAddView.fieldsCategory = data;
        this.form.controls.categoryId.patchValue(data.categoryId);
        this.form.controls.title.patchValue(data.title);
        this.form.controls.sortOrder.patchValue(data.sortOrder);
      }
     }

  ngOnInit(): void {
  }
  submit(){
    if (this.form.valid){
      if (this.form.controls.categoryId.value === 0){
        this.fieldsCategoryAddView.fieldsCategory.title = this.form.controls.title.value;
        this.fieldsCategoryAddView.fieldsCategory.sortOrder = this.form.controls.sortOrder.value;
        this.fieldsCategoryAddView.fieldsCategory.module = this.fieldCategoryModuleEnum.Student;
        this.customFieldService.addFieldsCategory(this.fieldsCategoryAddView).subscribe(
          (res: FieldsCategoryAddView) => {
            if (res){
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
            }else{
              this.snackbar.open(this.defaultValuesService.translateKey('fieldCategoryFailed') + sessionStorage.getItem('httpError'), '', {
                duration: 10000
              });
            }
          }
        );

      }
      else{
        this.fieldsCategoryAddView.fieldsCategory.categoryId = this.form.controls.categoryId.value;
        this.fieldsCategoryAddView.fieldsCategory.title = this.form.controls.title.value;
        this.fieldsCategoryAddView.fieldsCategory.sortOrder = this.form.controls.sortOrder.value;
        this.customFieldService.updateFieldsCategory(this.fieldsCategoryAddView).subscribe(
          (res: FieldsCategoryAddView) => {
            if (res){
            if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
                this.snackbar.open( res._message, '', {
                  duration: 10000
                });
              }
              else {
                this.snackbar.open(res._message, '', {
                  duration: 10000
                });
                this.dialogRef.close('submited');
              }
            }else{
              this.snackbar.open(this.defaultValuesService.translateKey('fieldCategoryFailed') + sessionStorage.getItem('httpError'), '', {
                duration: 10000
              });
            }
          }
        );
      }
    }
  }

}
