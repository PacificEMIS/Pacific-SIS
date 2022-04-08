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

import { Component, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { NgForm, ControlContainer } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SchoolAddViewModel } from '../../models/school-master.model';
import { SchoolCreate } from '../../../../src/app/enums/school-create.enum';
import { CustomFieldListViewModel, CustomFieldModel } from '../../models/custom-field.model';
import { FieldsCategoryListView } from '../../models/fields-category.model';
import { SharedFunction } from '../../../../src/app/pages/shared/shared-function';
import { CustomFieldService } from '../../../../src/app/services/custom-field.service';
import { SchoolService } from '../../../../src/app/services/school.service';
import { CustomFieldsValueModel } from '../../models/custom-fields-value.model';
import { stagger60ms } from '../../../../src/@vex/animations/stagger.animation';
import { StudentAddModel } from '../../models/student.model';
import { StudentService } from '../../../../src/app/services/student.service';
import { StaffAddModel } from '../../models/staff.model';
import { StaffService } from '../../services/staff.service';
import icCheckboxChecked from '@iconify/icons-ic/check-box';
import icCheckboxUnchecked from '@iconify/icons-ic/check-box-outline-blank';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { DefaultValuesService } from '../default-values.service';


@Component({
  selector: 'vex-custom-field-without-form',
  templateUrl: './custom-field-without-form.component.html',
  styleUrls: ['./custom-field-without-form.component.scss'],
  animations: [stagger60ms],
  // viewProviders: [{ provide: ControlContainer, useExisting: NgForm }]
})
export class CustomFieldWithoutFormComponent implements OnInit, OnDestroy {
  schoolCreate = SchoolCreate;
  @Input() schoolDetailsForViewAndEdit;
  @Input() categoryId;
  editInfo: boolean = false;
  schoolAddViewModel: SchoolAddViewModel = new SchoolAddViewModel();
  studentAddModel: StudentAddModel = new StudentAddModel();
  staffAddModel: StaffAddModel = new StaffAddModel();
  @Input() schoolCreateMode;
  @Input() studentCreateMode;
  @Input() staffCreateMode;
  @Input() module;
  @Output() custom=new EventEmitter;
  staffMultiSelectValue;
  studentMultiSelectValue;
  icCheckboxChecked = icCheckboxChecked;
  icCheckboxUnchecked = icCheckboxUnchecked;
  staffCustomFields=[];
  schoolCustomFields=[];
  studentCustomFields=[];
  schoolMultiSelectValue
  customFieldListViewModel = new CustomFieldListViewModel();
  headerTitle: string = "Other Information";
  @ViewChild('staff') staffForm: NgForm;
  @ViewChild('school') schoolForm: NgForm;
  @ViewChild('student') studentForm: NgForm;
  fieldsCategoryListView = new FieldsCategoryListView();
  destroySubject$: Subject<void> = new Subject();

  constructor(
    private snackbar: MatSnackBar,
    private commonFunction: SharedFunction,
    private customFieldservice: CustomFieldService,
    private schoolService: SchoolService,
    private studentService: StudentService,
    private staffService : StaffService,
    private defaultValuesService: DefaultValuesService, 
    public translateService: TranslateService,
  ) {
    // translateService.use("en");
    this.schoolService.schoolDetailsForViewedAndEdited.pipe(takeUntil(this.destroySubject$)).subscribe((res: SchoolAddViewModel) => {
      if(res){
        this.schoolDetailsForViewAndEdit = res;
        this.checkCustomValue();
      }
    });
    this.studentService.getStudentDetailsForGeneral.pipe(takeUntil(this.destroySubject$)).subscribe((res: StudentAddModel) => {
      if(res){
        this.studentAddModel = res;
        if(res?.fieldsCategoryList?.length>0){
          this.checkStudentCustomValue();
        }
      }
    })
    this.staffService.getStaffDetailsForGeneral.pipe(takeUntil(this.destroySubject$)).subscribe((res: StaffAddModel) => {
      if(res){
        this.staffAddModel = res;
        if(res?.fieldsCategoryList?.length>0){
          this.checkStaffCustomValue();
        }
      }
    })
    this.defaultValuesService.customFieldsCheckParentCompObs.subscribe(x=>{ if(x) this.checkFormValidOrNot(); })
  }

  ngOnInit(): void {
    if (this.module === 'Student') {
      this.studentAddModel = this.schoolDetailsForViewAndEdit;
      if(this.schoolDetailsForViewAndEdit?.fieldsCategoryList?.length>0){
        this.checkStudentCustomValue();
      }
    }
    else if (this.module === 'School') {
      this.checkNgOnInitCustomValue();
    }
    else if(this.module === 'Staff') {
      this.staffAddModel = this.schoolDetailsForViewAndEdit;
      if(this.schoolDetailsForViewAndEdit?.fieldsCategoryList?.length>0){
        this.checkStaffCustomValue();
      }
    }
  }

  modelChanged(selectValue) {
    if (this.module === 'Staff') {
      this.staffService.setStaffMultiselectValue(selectValue);
    }
    else if (this.module === 'Student') {
      this.studentService.setStudentMultiselectValue(selectValue);
    }
    else if (this.module === 'School') {
      this.schoolService.setSchoolMultiselectValue(selectValue);
    }
}

checkFormValidOrNot() {
  if (this.module === 'Staff') {
    this.defaultValuesService.customFieldsCheckParentComp.next(false);
    if (this.staffAddModel?.fieldsCategoryList?.length > 0) {
      this.staffForm.form.markAllAsTouched();
      if (this.staffForm.form.valid) 
        this.custom.emit(true)
      else 
        this.custom.emit(false)
    }
    else {
      this.custom.emit(false)
    }
  }
}

  checkStudentCustomValue() {
    if (this.studentAddModel?.fieldsCategoryList?.length>0) {      
        this.studentCustomFields = this.studentAddModel?.fieldsCategoryList[this.categoryId]?.customFields.filter(x=> !x.systemField && !x.hide);
        if(this.studentCustomFields?.length>0){

        for (let studentCustomField of this.studentCustomFields) {
          if (studentCustomField?.customFieldsValue.length !== 0) {
            if (studentCustomField?.type === 'Multiple SelectBox') {
              this.studentMultiSelectValue = studentCustomField?.customFieldsValue[0].customFieldValue.split('|');
            }
            else if(studentCustomField?.type === 'Checkbox'){
              if(studentCustomField.customFieldsValue[0].customFieldValue === "true"){
                studentCustomField.customFieldsValue[0].customFieldValue = true;
              }
              else if (studentCustomField.customFieldsValue[0].customFieldValue === "false"){
                studentCustomField.customFieldsValue[0].customFieldValue = false;
              }
            }
          }
          else {
            studentCustomField?.customFieldsValue.push(new CustomFieldsValueModel());
            if(studentCustomField.type === 'Checkbox'){
              studentCustomField.customFieldsValue[0].customFieldValue= studentCustomField.defaultSelection === "false" ? false : true;
            }
            else{
              studentCustomField.customFieldsValue[0].customFieldValue= studentCustomField.defaultSelection;
            }
          }
        }
      }
    }
  }

  checkStaffCustomValue(){    
      if (this.staffAddModel?.fieldsCategoryList?.length>0) {        
        this.staffCustomFields = this.staffAddModel?.fieldsCategoryList[this.categoryId]?.customFields.filter(x=> !x.systemField && !x.hide);
        
        if(this.staffCustomFields?.length>0 ){
        for (let staffCustomField of this.staffCustomFields) {
          if (staffCustomField?.customFieldsValue.length !== 0) {
            if (staffCustomField?.type === 'Multiple SelectBox') {
              this.staffMultiSelectValue = staffCustomField?.customFieldsValue[0].customFieldValue.split('|');
            }
            else if(staffCustomField?.type==='Checkbox'){
              if(staffCustomField.customFieldsValue[0].customFieldValue === "true"){
                staffCustomField.customFieldsValue[0].customFieldValue = true;
              }
            else if (staffCustomField.customFieldsValue[0].customFieldValue === "false"){
                staffCustomField.customFieldsValue[0].customFieldValue = false;
              }
            }
          }
          else {
            staffCustomField?.customFieldsValue.push(new CustomFieldsValueModel());
            if(staffCustomField.type === 'Checkbox'){
              staffCustomField.customFieldsValue[0].customFieldValue= staffCustomField.defaultSelection === "false" ? false : true;
            }
            else{
              staffCustomField.customFieldsValue[0].customFieldValue= staffCustomField.defaultSelection;
            }
          }
        }
      }
      }
  }

  checkNgOnInitCustomValue() {

    if (this.schoolDetailsForViewAndEdit !== undefined) {      
      if (this.schoolDetailsForViewAndEdit.schoolMaster.fieldsCategory !== undefined) {
        if (this.schoolDetailsForViewAndEdit.schoolMaster.fieldsCategory[this.categoryId]?.customFields !== undefined) {
          this.schoolCustomFields= this.schoolDetailsForViewAndEdit.schoolMaster.fieldsCategory[this.categoryId].customFields.filter(x => !x.systemField && !x.hide);
          for (let schoolCustomField of this.schoolCustomFields) {
            if (schoolCustomField.customFieldsValue.length !== 0) {
              if (schoolCustomField?.type === 'Multiple SelectBox') {
                this.schoolMultiSelectValue = schoolCustomField?.customFieldsValue[0].customFieldValue.split('|');
              }
              else if(schoolCustomField?.type === 'Checkbox'){
                if(schoolCustomField.customFieldsValue[0].customFieldValue === "true"){
                  schoolCustomField.customFieldsValue[0].customFieldValue = true;
                }
                else if (schoolCustomField.customFieldsValue[0].customFieldValue === "false"){
                  schoolCustomField.customFieldsValue[0].customFieldValue = false;
                }
              } 
            }
            else {
              schoolCustomField.customFieldsValue.push(new CustomFieldsValueModel());
              if(schoolCustomField.type === 'Checkbox'){
                schoolCustomField.customFieldsValue[0].customFieldValue= schoolCustomField.defaultSelection === "false" ? false : true;
              }
              else{
                schoolCustomField.customFieldsValue[0].customFieldValue= schoolCustomField.defaultSelection;
              }
            }
          }
        }
      }
    }
  }

  checkCustomValue() {
    if (this.schoolDetailsForViewAndEdit !== undefined) {
      if (this.schoolDetailsForViewAndEdit.schoolMaster.fieldsCategory !== undefined && this.schoolDetailsForViewAndEdit.schoolMaster.fieldsCategory !== null) {

        if (this.schoolDetailsForViewAndEdit.schoolMaster.fieldsCategory[this.categoryId]?.customFields !== undefined) {

          this.schoolCustomFields= this.schoolDetailsForViewAndEdit.schoolMaster.fieldsCategory[this.categoryId].customFields.filter(x => !x.systemField && !x.hide);
          
          for (let schoolCustomField of this.schoolCustomFields) {
            if (schoolCustomField.customFieldsValue.length === 0) {
              schoolCustomField.customFieldsValue.push(new CustomFieldsValueModel());
              if(schoolCustomField.type==='Checkbox'){
                schoolCustomField.customFieldsValue[0].customFieldValue= schoolCustomField.defaultSelection === "false" ? false : true;
              }
              else{
                schoolCustomField.customFieldsValue[0].customFieldValue= schoolCustomField.defaultSelection;
              }
            }
            else {
              if(schoolCustomField.type==='Checkbox'){
                schoolCustomField.customFieldsValue[0].customFieldValue= schoolCustomField.customFieldsValue[0].customFieldValue === "false" ? false : true;
              }
              if (schoolCustomField?.type === 'Multiple SelectBox') {
                this.schoolMultiSelectValue = schoolCustomField?.customFieldsValue[0].customFieldValue.split('|');
  
              }
            }
          }
        }
      }
    }
  }
  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.unsubscribe();
  }

}
