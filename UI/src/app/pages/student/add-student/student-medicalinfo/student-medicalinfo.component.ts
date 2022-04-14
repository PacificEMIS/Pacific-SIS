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

import {
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { FormBuilder, NgForm } from '@angular/forms';
import { fadeInUp400ms } from '../../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../../@vex/animations/stagger.animation';
import { fadeInRight400ms } from '../../../../../@vex/animations/fade-in-right.animation';
import { TranslateService } from '@ngx-translate/core';
import icEdit from '@iconify/icons-ic/twotone-edit';
import icDelete from '@iconify/icons-ic/twotone-delete';
import icAdd from '@iconify/icons-ic/baseline-add';
import icComment from '@iconify/icons-ic/twotone-comment';
import icDeleteForever from '@iconify/icons-ic/twotone-delete-forever';
import { SchoolCreate } from '../../../../enums/school-create.enum';
import {
  AddEditStudentMedicalAlertModel,
  AddEditStudentMedicalImmunizationModel,
  AddEditStudentMedicalNoteModel,
  AddEditStudentMedicalNurseVisitModel,
  AddEditStudentMedicalProviderModel,
  StudentAddModel,
  StudentMedicalInfoListModel } from '../../../../models/student.model';
import { StudentService } from '../../../../services/student.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { GetAllParentInfoModel, ViewParentInfoModel } from '../../../../models/parent-info.model';
import { ParentInfoService } from '../../../../services/parent-info.service';
import { ImageCropperService } from '../../../../services/image-cropper.service';
import { ModuleIdentifier } from '../../../../enums/module-identifier.enum';
import {
  Permissions,
  RolePermissionListViewModel,
  RolePermissionViewModel,
} from '../../../../models/roll-based-access.model';
import { CryptoService } from '../../../../services/Crypto.service';
import { DefaultValuesService } from '../../../../common/default-values.service';
import { MatDialog } from '@angular/material/dialog';
import { AddAlertComponent } from './add-alert/add-alert.component';
import { AddMedicalComponent } from './add-medical/add-medical.component';
import { AddImmunizationComponent } from './add-immunization/add-immunization.component';
import { AddNurseVisitComponent } from './add-nurse-visit/add-nurse-visit.component';
import { MatTabChangeEvent } from '@angular/material/tabs';
import { ConfirmDialogComponent } from 'src/app/pages/shared-module/confirm-dialog/confirm-dialog.component';
import { PageRolesPermission } from '../../../../common/page-roles-permissions.service';
import { CommonService } from '../../../../services/common.service';
import { FieldsCategoryListView } from '../../../../models/fields-category.model';
import { Module } from '../../../../enums/module.enum';
import { CustomFieldService } from '../../../../services/custom-field.service';
@Component({
  selector: 'vex-student-medicalinfo',
  templateUrl: './student-medicalinfo.component.html',
  styleUrls: ['./student-medicalinfo.component.scss'],
  animations: [stagger60ms, fadeInUp400ms, fadeInRight400ms],
})
export class StudentMedicalinfoComponent implements OnInit, OnDestroy {
  moduleIdentifier = ModuleIdentifier;
  studentCreate = SchoolCreate;
  @Input() studentCreateMode: SchoolCreate;
  @Input() categoryId;
  @Input() studentDetailsForViewAndEdit;
  @ViewChild('f') currentForm: NgForm;
  @Output() studentDetailsForParent = new EventEmitter<StudentAddModel>();
  studentAddModel: StudentAddModel = new StudentAddModel();
  parentInfoModel: ViewParentInfoModel = new ViewParentInfoModel();
  fieldsCategoryListView = new FieldsCategoryListView();
  icEdit = icEdit;
  icDelete = icDelete;
  icDeleteForever = icDeleteForever;
  module = 'Student';
  actionButtonTitle = 'submit';
  icAdd = icAdd;
  icComment = icComment;
  parentsFullName = [];
  cloneStudentAddModel;
  editPermission = false;
  deletePermission = false;
  addPermission = false;
  permissionListViewModel: RolePermissionListViewModel = new RolePermissionListViewModel();
  permissionGroup: RolePermissionViewModel = new RolePermissionViewModel();
  studentMedicalInfoListModel: StudentMedicalInfoListModel = new StudentMedicalInfoListModel();
  addEditStudentMedicalNurseVisitModel: AddEditStudentMedicalNurseVisitModel = new AddEditStudentMedicalNurseVisitModel();
  addEditStudentMedicalAlertModel: AddEditStudentMedicalAlertModel = new AddEditStudentMedicalAlertModel();
  addEditStudentMedicalImmunizationModel: AddEditStudentMedicalImmunizationModel = new AddEditStudentMedicalImmunizationModel();
  addEditStudentMedicalNoteModel: AddEditStudentMedicalNoteModel = new AddEditStudentMedicalNoteModel();
  getAllParentInfoModel: GetAllParentInfoModel = new GetAllParentInfoModel();
  addEditStudentMedicalProviderModel: AddEditStudentMedicalProviderModel = new AddEditStudentMedicalProviderModel();
  currentTab: string;
  permissions: Permissions;
  customValid=false;
  constructor(
    private fb: FormBuilder,
    public translateService: TranslateService,
    private studentService: StudentService,
    private snackbar: MatSnackBar,
    private customFieldservice: CustomFieldService,
    private defaultValuesService: DefaultValuesService,
    private parentInfoService: ParentInfoService,
    private imageCropperService: ImageCropperService,
    private cryptoService: CryptoService,
    private pageRolePermissions: PageRolesPermission,
    private dialog: MatDialog,
    private commonService: CommonService,
  ) {
    //translateService.use('en');
    this.defaultValuesService.checkAcademicYear() && !this.studentService.getStudentId() ? this.studentService.redirectToGeneralInfo() : !this.defaultValuesService.checkAcademicYear() && !this.studentService.getStudentId() ? this.studentService.redirectToStudentList() : '';
  }

  ngOnInit(): void {
    this.studentService.studentCreatedMode.subscribe((res) => {
      this.studentCreateMode = res;
    });
    this.studentService.studentDetailsForViewedAndEdited.subscribe((res) => {
      this.studentDetailsForViewAndEdit = res;
    });
    this.studentService.categoryIdSelected.subscribe((res) => {
      this.categoryId = res;
    });
    this.currentTab = 'activities';
    if (this.studentCreateMode === this.studentCreate.VIEW) {
      this.permissions = this.pageRolePermissions.checkPageRolePermission();
      this.studentService.changePageMode(this.studentCreateMode);
      this.studentAddModel = this.studentDetailsForViewAndEdit;
      this.cloneStudentAddModel = JSON.stringify(this.studentAddModel);
    } else {
      
      this.getAllParents();
      this.permissions = this.pageRolePermissions.checkPageRolePermission();
      this.studentService.changePageMode(this.studentCreateMode);
      this.studentAddModel = this.studentService.getStudentDetails();
    }
    this.getAllStudentMedicalInfo();
  }

  editMedicalInfo() {
    this.addEditStudentMedicalProviderModel.fieldsCategoryList= this.checkViewPermission(
      this.studentMedicalInfoListModel.fieldsCategoryList
    );
    this.actionButtonTitle='update';
    this.getAllParents();
    this.studentCreateMode = this.studentCreate.EDIT;
    this.studentService.changePageMode(this.studentCreateMode);
  }

  cancelEdit() {
    if (JSON.stringify(this.studentAddModel) !== this.cloneStudentAddModel) {
      this.studentAddModel = JSON.parse(this.cloneStudentAddModel);
      this.studentService.sendDetails(JSON.parse(this.cloneStudentAddModel));
    }
    this.studentCreateMode = this.studentCreate.VIEW;
    this.studentService.changePageMode(this.studentCreateMode);
  }

  getAllParents() {
    this.parentInfoModel.studentId = this.studentAddModel.studentMaster.studentId;
    this.parentInfoService
      .ViewParentListForStudent(this.parentInfoModel)
      .subscribe((res) => {
        if(res._failure){
          this.commonService.checkTokenValidOrNot(res._message);
        }
        this.concatenateParentsName(res.parentInfoListForView);
      });
  }

  concatenateParentsName(parentDetails) {
    this.parentsFullName = parentDetails?.map((item) => {
      return item.firstname + ' ' + item.lastname;
    });
  }

  addAlert() {
    this.dialog.open(AddAlertComponent, {
      data: null,
      width: '500px',
    }).afterClosed().subscribe(data => {
      if (data === 'submited'){
        this.getAllStudentMedicalInfo();
      }
    });
  }
  editAlert(info) {
    this.dialog.open(AddAlertComponent, {
      data: info,
      width: '500px',
    }).afterClosed().subscribe(data => {
      if (data === 'submited'){
        this.getAllStudentMedicalInfo();
      }
    });
  }

  addMedicalNotes() {
    this.dialog.open(AddMedicalComponent, {
      data: null,
      width: '500px',
    }).afterClosed().subscribe(data => {
      if (data === 'submited'){
        this.getAllStudentMedicalInfo();
      }
    });
  }
  editMedicalNotes(info) {
    this.dialog.open(AddMedicalComponent, {
      data: info,
      width: '500px',
    }).afterClosed().subscribe(data => {
      if (data === 'submited'){
        this.getAllStudentMedicalInfo();
      }
    });
  }

  addImmunization() {
    this.dialog.open(AddImmunizationComponent, {
      data: null,
      width: '500px',
    }).afterClosed().subscribe(data => {
      if (data === 'submited'){
        this.getAllStudentMedicalInfo();
      }
    });
  }
  editImmunization(info) {
    this.dialog.open(AddImmunizationComponent, {
      data: info,
      width: '500px',
    }).afterClosed().subscribe(data => {
      if (data === 'submited'){
        this.getAllStudentMedicalInfo();
      }
    });
  }

  addNurseVisit() {
    this.dialog.open(AddNurseVisitComponent, {
      data: null,
      width: '500px',
    }).afterClosed().subscribe(data => {
      if (data === 'submited'){
        this.getAllStudentMedicalInfo();
      }
    });
  }
  editNurseVisit(info) {
    this.dialog.open(AddNurseVisitComponent, {
      data: info,
      width: '500px',
    }).afterClosed().subscribe(data => {
      if (data === 'submited'){
        this.getAllStudentMedicalInfo();
      }
    });
  }

  changeTab(status) {
    this.currentTab = status;
    if(this.currentTab === 'providerInformation'){
      this.addEditStudentMedicalProviderModel.fieldsCategoryList= this.checkViewPermission(
        this.studentMedicalInfoListModel.fieldsCategoryList
      );
      // if(this.studentCreateMode === this.studentCreate.ADD){
        this.getAllFieldsCategory();
      //}
     
    }
    if(this.studentCreateMode !== this.studentCreate.EDIT && this.studentCreateMode !== this.studentCreate.ADD ){
     
      this.studentCreateMode = this.studentCreate.VIEW;
    }
    else{
      this.studentCreateMode = this.studentCreate.EDIT;
    }
  }

  getAllFieldsCategory() {
    this.fieldsCategoryListView.module = Module.STUDENT;
    this.customFieldservice
      .getAllFieldsCategory(this.fieldsCategoryListView)
      .subscribe((res) => {
        if (res) {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            if (!res.fieldsCategoryList) {
              this.snackbar.open(res._message, "", {
                duration: 10000,
              });
            }
          } else {
            this.addEditStudentMedicalProviderModel.fieldsCategoryList = this.checkViewPermission(
              res.fieldsCategoryList
            );
          }
        } else {
          this.snackbar.open(
            this.defaultValuesService.translateKey("categoryListFailed") +
            this.defaultValuesService.getHttpError(),
            "",
            {
              duration: 10000,
            }
          );
        }
      });
  }
 

  deleteStudentMedicalAlert(element){
    this.addEditStudentMedicalAlertModel.studentMedicalAlert = element;
    this.studentService.deleteStudentMedicalAlert(this.addEditStudentMedicalAlertModel).subscribe(
      (res: AddEditStudentMedicalAlertModel) => {
        if (res){
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open( res._message, '', {
              duration: 10000
            });
          }
          else {
            this.snackbar.open( res._message, '', {
              duration: 10000
            });
            this.getAllStudentMedicalInfo();
          }
        }
        else{
          this.snackbar.open( this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      }
    );
  }
  confirmDeleteStudentMedicalAlert(element){
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: {
          title: 'Are you sure?',
          message: 'You are about to delete this Alert Information ' + '.'}
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult){
        this.deleteStudentMedicalAlert(element);
      }
   });
  }
  deleteStudentMedicalNote(element){
    this.addEditStudentMedicalNoteModel.studentMedicalNote = element;
    this.studentService.deleteStudentMedicalNote(this.addEditStudentMedicalNoteModel).subscribe(
      (res: AddEditStudentMedicalNoteModel) => {
        if (res){
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open( res._message, '', {
              duration: 10000
            });
          }
          else {
            this.snackbar.open( res._message, '', {
              duration: 10000
            });
            this.getAllStudentMedicalInfo();
          }
        }
        else{
          this.snackbar.open( this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      }
    );
  }
  confirmDeleteStudentMedicalNote(element){
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: {
          title: 'Are you sure?',
          message: 'You are about to delete this Medical Notes' + '.'}
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult){
        this.deleteStudentMedicalNote(element);
      }
   });
  }

  deleteStudentMedicalImmunization(element){
    this.addEditStudentMedicalImmunizationModel.studentMedicalImmunization = element;
    this.studentService.deleteStudentMedicalImmunization(this.addEditStudentMedicalImmunizationModel).subscribe(
      (res: AddEditStudentMedicalImmunizationModel) => {
        if (res){
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open( res._message, '', {
              duration: 10000
            });
          }
          else {
            this.snackbar.open( res._message, '', {
              duration: 10000
            });
            this.getAllStudentMedicalInfo();
          }
        }
        else{
          this.snackbar.open( this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      }
    );
  }

  confirmDeleteStudentMedicalImmunization(element){
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: {
          title: 'Are you sure?',
          message: 'You are about to delete this physical record ' + '.'}
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult){
        this.deleteStudentMedicalImmunization(element);
      }
   });
  }

  deleteStudentMedicalNurseVisit(element){
    this.addEditStudentMedicalNurseVisitModel.studentMedicalNurseVisit = element;
    this.studentService.deleteStudentMedicalNurseVisit(this.addEditStudentMedicalNurseVisitModel).subscribe(
      (res: AddEditStudentMedicalNurseVisitModel) => {
        if (res){
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open( res._message, '', {
              duration: 10000
            });
          }
          else {
            this.snackbar.open( res._message, '', {
              duration: 10000
            });
            this.getAllStudentMedicalInfo();
          }
        }
        else{
          this.snackbar.open( this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      }
    );
  }
  confirmDeleteStudentMedicalNurseVisit(element){
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: {
          title: 'Are you sure?',
          message: 'You are about to delete this nurse visit record ' + '.'}
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult){
        this.deleteStudentMedicalNurseVisit(element);
      }
   });
  }

  submit() {
    this.customValid=false;
    if (this.addEditStudentMedicalProviderModel.fieldsCategoryList){
      this.addEditStudentMedicalProviderModel.selectedCategoryId =
      this.addEditStudentMedicalProviderModel.fieldsCategoryList[this.categoryId].categoryId;
      for (const studentCustomField of this.addEditStudentMedicalProviderModel
        .fieldsCategoryList[this.categoryId].customFields){
          if (
            studentCustomField.type === 'Multiple SelectBox' &&
            this.studentService.getStudentMultiselectValue() !== undefined
          ) {
            studentCustomField.customFieldsValue[0].customFieldValue = this.studentService
              .getStudentMultiselectValue()
              .toString()
              .replaceAll(',', '|');
          }
      }
    }
    this.currentForm.form.markAllAsTouched();
    this.defaultValuesService.customFieldsCheckParentComp.next(true);
    if (this.currentForm.form.valid) {
      if(this.customValid){
      this.addEditStudentMedicalProviderModel.studentMedicalProvider.studentId = this.studentService.getStudentId();
      if (this.addEditStudentMedicalProviderModel.studentMedicalProvider.id === 0){
        this.studentService.addStudentMedicalProvider(this.addEditStudentMedicalProviderModel).subscribe(
          (res) => {
            if (res){
               if (res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
                this.studentCreateMode = this.studentCreate.VIEW;
              }else{
                this.studentCreateMode = this.studentCreate.VIEW;
                this.addEditStudentMedicalProviderModel = res;
                this.snackbar.open( res._message, '', {
                  duration: 10000
                });
              }
            }
            else{
              this.snackbar.open( this.defaultValuesService.getHttpError(), '', {
                duration: 10000
              });
            }
          }
        );
      }else{
        this.studentService.updateStudentMedicalProvider(this.addEditStudentMedicalProviderModel).subscribe(
          (res) => {
            if (res){
               if (res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
                this.studentCreateMode = this.studentCreate.EDIT;
              }else{
                this.studentCreateMode = this.studentCreate.VIEW;
                this.addEditStudentMedicalProviderModel = res;
                this.snackbar.open( res._message, '', {
                  duration: 10000
                });
              }
            }
            else{
              this.snackbar.open( this.defaultValuesService.getHttpError(), '', {
                duration: 10000
              });
            }
          }
        );
      }
      /* if (this.studentAddModel.fieldsCategoryList !== null) {
        this.studentAddModel.selectedCategoryId = this.studentAddModel.fieldsCategoryList[
          this.categoryId
        ].categoryId;

        for (const studentCustomField of this.studentAddModel
          .fieldsCategoryList[this.categoryId].customFields) {
          if (
            studentCustomField.type === 'Multiple SelectBox' &&
            this.studentService.getStudentMultiselectValue() !== undefined
          ) {
            studentCustomField.customFieldsValue[0].customFieldValue = this.studentService
              .getStudentMultiselectValue()
              .toString()
              .replaceAll(',', '|');
          }
        }
      } */
      /* this.studentService
        .UpdateStudent(this.studentAddModel)
        .subscribe((data) => {
          if (data) {
           if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
              this.snackbar.open(data._message, '', {
                duration: 10000,
              });
            } else {
              this.snackbar.open(data._message, '', {
                duration: 10000,
              });
              this.studentService.setStudentCloneImage(
                data.studentMaster.studentPhoto
              );
              data.studentMaster.studentPhoto = null;
              this.cloneStudentAddModel = JSON.stringify(data);
              this.studentCreateMode = this.studentCreate.VIEW;
              this.studentService.changePageMode(this.studentCreateMode);
              this.studentDetailsForParent.emit(data);
            }
          } else {
            this.snackbar.open(
              this.defaultValuesService.translateKey(
                'medicalInformationUpdationFailed'
              ) + this.defaultValuesService.getHttpError(),
              '',
              {
                duration: 10000,
              }
            );
          }
        }); */
    }
  }
  }
  getAllStudentMedicalInfo(){
    this.studentMedicalInfoListModel.studentId = this.studentService.getStudentId();
    this.studentService.getAllStudentMedicalInfo(this.studentMedicalInfoListModel).subscribe(
      (res: StudentMedicalInfoListModel)  => {
        if (res){
           if (res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open( res._message, '', {
              duration: 10000
            });
          }
          else{
            this.studentMedicalInfoListModel = res;
            this.checkCriticalStatus()

            if (this.studentMedicalInfoListModel.studentMedicalProviderList.length < 1){
              this.addEditStudentMedicalProviderModel = new AddEditStudentMedicalProviderModel();
            }
            else{
              this.addEditStudentMedicalProviderModel.studentMedicalProvider =
              this.studentMedicalInfoListModel.studentMedicalProviderList[0];
              this.addEditStudentMedicalProviderModel.fieldsCategoryList =
              this.checkViewPermission(
                this.studentMedicalInfoListModel.fieldsCategoryList
              );
            }
          }
        }else{
          this.snackbar.open( this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      }
    );
  }
  checkViewPermission(category) {
    let permittedTabs =this.pageRolePermissions.getPermittedSubCategories('/school/students')
    category = category.map((item) => {
      for (const permission of permittedTabs) {
        if (
          item.title.toLowerCase() ===
          permission.title.toLowerCase()
        ) {
            return item;
        }
      }
    });
    return category;
  }

  checkCriticalStatus(){
      this.studentService.setStudentCriticalInfo(this.studentMedicalInfoListModel.studentMedicalAlertList);
  }

  ngOnDestroy() {
    // this.imageCropperService.enableUpload({
    //   module: this.moduleIdentifier.STUDENT,
    //   upload: false,
    //   mode: this.studentCreate.VIEW,
    // });
  }
}
