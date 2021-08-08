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

import { Component, OnInit, ChangeDetectorRef, Input, OnDestroy } from '@angular/core';
import { FormBuilder, NgForm } from '@angular/forms';
import { fadeInUp400ms } from '../../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../../@vex/animations/stagger.animation';
import { fadeInRight400ms } from '../../../../../@vex/animations/fade-in-right.animation';
import { TranslateService } from '@ngx-translate/core';
import icAdd from '@iconify/icons-ic/baseline-add';
import icClear from '@iconify/icons-ic/baseline-clear';
import icVisibility from '@iconify/icons-ic/twotone-visibility';
import icVisibilityOff from '@iconify/icons-ic/twotone-visibility-off';
import { salutation, suffix, relationShip, userProfile } from '../../../../enums/studentAdd.enum';
import { AddParentInfoModel, ParentInfoList, RemoveAssociateParent } from '../../../../models/parent-info.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ParentInfoService } from '../../../../services/parent-info.service';
import { StudentService } from '../../../../services/student.service';
import icEdit from '@iconify/icons-ic/edit';
import icDelete from '@iconify/icons-ic/delete';
import icRemove from '@iconify/icons-ic/remove-circle';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { AddSiblingComponent } from '../../../student/add-student/student-familyinfo/add-sibling/add-sibling.component';
import { ViewSiblingComponent } from '../../../student/add-student/student-familyinfo/view-sibling/view-sibling.component';
import { ConfirmDialogComponent } from '../../../shared-module/confirm-dialog/confirm-dialog.component';
import { StudentSiblingAssociation } from '../../../../models/student.model';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { ImageCropperService } from 'src/app/services/image-cropper.service';
import { SchoolCreate } from '../../../../enums/school-create.enum';
import { LovList } from '../../../../models/lov.model';
import { CommonService } from '../../../../services/common.service';
import { ModuleIdentifier } from '../../../../enums/module-identifier.enum';
import { CommonLOV } from '../../../shared-module/lov/common-lov';
import { Permissions, RolePermissionListViewModel, RolePermissionViewModel } from '../../../../models/roll-based-access.model';
import { CryptoService } from '../../../../services/Crypto.service';
import { ResetPasswordComponent } from 'src/app/pages/shared-module/reset-password/reset-password.component';
import { PageRolesPermission } from '../../../../common/page-roles-permissions.service';

@Component({
  selector: 'vex-editparent-generalinfo',
  templateUrl: './editparent-generalinfo.component.html',
  styleUrls: ['./editparent-generalinfo.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms,
    fadeInRight400ms
  ]
})
export class EditparentGeneralinfoComponent implements OnInit, OnDestroy {
  schoolCreate = SchoolCreate;
  @Input() schoolCreateMode: SchoolCreate;
  @Input() parentDetailsForViewAndEdit;
  parentCreate = SchoolCreate;
  @Input() parentCreateMode: SchoolCreate;
  icAdd = icAdd;
  icClear = icClear;
  icVisibility = icVisibility;
  icVisibilityOff = icVisibilityOff;
  icEdit = icEdit;
  icDelete = icDelete;
  icRemove = icRemove;
  inputType = 'password';
  visible = false;
  salutationEnum = Object.keys(salutation);
  suffixEnum = Object.keys(suffix);
  relationShipEnum = Object.keys(relationShip);
  userProfileEnum = Object.keys(userProfile);
  f: NgForm;
  isPortalUser = false;
  parentDetails;
  mode = "view";
  associateStudentMode = "";
  addParentInfoModel: AddParentInfoModel = new AddParentInfoModel();
  duplicateAddParentInfoModel: AddParentInfoModel = new AddParentInfoModel();
  parentInfoList: ParentInfoList = new ParentInfoList();
  lovList: LovList = new LovList();
  studentSiblingAssociation: StudentSiblingAssociation = new StudentSiblingAssociation();
  removeAssociateParent: RemoveAssociateParent = new RemoveAssociateParent();
  parentInfo;
  studentInfo;
  salutationList;
  suffixList;
  salutation;
  suffix;
  moduleIdentifier = ModuleIdentifier;
  destroySubject$: Subject<void> = new Subject();
  editPermission = false;
  deletePermission = false;
  addPermission = false;
  permissions: Permissions;
  constructor(
    public translateService: TranslateService,
    private cd: ChangeDetectorRef,
    private parentInfoService: ParentInfoService,
    private snackbar: MatSnackBar,
    private router: Router,
    private dialog: MatDialog,
    private imageCropperService: ImageCropperService,
    private commonService: CommonService,
    private commonLOV: CommonLOV,
    private pageRolePermissions: PageRolesPermission,
    private cryptoService: CryptoService
  ) {
    //translateService.use('en');

  }

  ngOnInit(): void {
    this.parentInfoService.parentCreatedMode.subscribe((res)=>{
      this.parentCreateMode = res;
    });
    this.permissions = this.pageRolePermissions.checkPageRolePermission();
    this.imageCropperService.enableUpload({ module: this.moduleIdentifier.PARENT, upload: true, mode: this.parentCreate.VIEW });
    this.callLOVs();
    this.parentInfo = {};
    this.getParentDetailsUsingId();
    /* if (this.parentDetailsForViewAndEdit.parentInfo.hasOwnProperty('firstname')) {
      this.addParentInfoModel = this.parentDetailsForViewAndEdit;
      this.parentInfo = this.addParentInfoModel.parentInfo;
      this.studentInfo = this.addParentInfoModel.getStudentForView;
      this.setEmptyValue(this.parentInfo, this.studentInfo);
    } else {
      this.parentInfoService.getParentDetailsForGeneral.subscribe((res: AddParentInfoModel) => {
        this.addParentInfoModel = res;
        this.parentInfo = this.addParentInfoModel.parentInfo;
        this.studentInfo = this.addParentInfoModel.getStudentForView;
        this.setEmptyValue(this.parentInfo, this.studentInfo);
      })
    } */

  }
  getParentDetailsUsingId(){
    this.addParentInfoModel.parentInfo.parentId = this.parentInfoService.getParentId();
    this.parentInfoService.viewParentInfo(this.addParentInfoModel).subscribe(
      (res) => {
        if (res){
          if(res._failure){
            this.commonService.checkTokenValidOrNot(res._message);
          }
          this.addParentInfoModel = res;
          this.duplicateAddParentInfoModel = JSON.parse(JSON.stringify(res));
          this.parentInfo = res.parentInfo;
          this.studentInfo = res.getStudentForView;
          this.parentInfoService.setParentDetailsForViewAndEdit(this.addParentInfoModel);
          this.setEmptyValue(this.parentInfo, this.studentInfo);
        }
      }
    );
  }
  setEmptyValue(parentInfo, studentInfo) {
    if (studentInfo !== undefined) {
      studentInfo.forEach(element => {

        if (element.middleName === null) {
          element.middleName = "";
        }
      });
    }

    if (parentInfo.isPortalUser === true) {
      this.isPortalUser = true;
      this.addParentInfoModel.parentInfo.isPortalUser = true;
    } else {
      this.isPortalUser = false;
      this.addParentInfoModel.parentInfo.isPortalUser = false;
    }
    if (parentInfo) {
      this.mode = "view";
    }
  }
  portalUserCheck(event) {
    if (event.checked === true) {
      this.isPortalUser = true;
      this.addParentInfoModel.parentInfo.isPortalUser = true;
    } else {
      this.isPortalUser = false;
      this.addParentInfoModel.parentInfo.isPortalUser = false;
    }
  }

  editGeneralInfo() {
    this.mode = "add";
    this.parentCreateMode = this.parentCreate.EDIT;
    this.addParentInfoModel.parentInfo = this.parentInfo;
    this.imageCropperService.enableUpload({ module: this.moduleIdentifier.PARENT, upload: true, mode: this.parentCreate.EDIT });
    this.parentInfoService.changePageMode(this.parentCreateMode);
  }

  cancelEdit() {
    this.addParentInfoModel = JSON.parse(JSON.stringify(this.duplicateAddParentInfoModel));
    this.parentInfo = this.addParentInfoModel.parentInfo;
    this.studentInfo = this.addParentInfoModel.getStudentForView;
    this.setEmptyValue(this.parentInfo, this.studentInfo);
  }

  callLOVs() {
    this.commonLOV.getLovByName('Salutation').pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      this.salutationList = res;
    });
    this.commonLOV.getLovByName('Suffix').pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      this.suffixList = res;
    });
  }

  submit() {
    this.addParentInfoModel.parentInfo.parentId = this.parentInfoService.getParentId();
    this.parentInfoService.updateParentInfo(this.addParentInfoModel).subscribe(data => {
      if (typeof (data) == 'undefined') {
        this.snackbar.open('Parent Information Updation failed. ' + sessionStorage.getItem("httpError"), '', {
          duration: 10000
        });
      }
      else {
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
        }
        else {
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
          this.getParentDetailsUsingId();
        }
      }
    })
  }

  // This openResetPassword method is used for open Reset Password dialog.
  openResetPassword() {
    this.dialog.open(ResetPasswordComponent, {
      width: '500px',
      data: { userId: this.addParentInfoModel.parentInfo.parentId, emailAddress: this.addParentInfoModel.parentInfo.loginEmail }
    });
  }

  openViewDetails(studentDetails) {
    this.dialog.open(ViewSiblingComponent, {
      data: {
        siblingDetails: this.parentInfo,
        studentDetails: studentDetails,
        flag: "Parent"
      },
      width: '600px'
    })
  }

  associateStudent() {
    this.associateStudentMode = "search";
    this.dialog.open(AddSiblingComponent, {
      data: {
        data: this.addParentInfoModel
      },
      width: '600px'
    }).afterClosed().subscribe(data => {
      if (data) {
        this.getParentDetailsUsingId();
      }

    });
  }

  confirmDelete(deleteDetails) {
    // call our modal window
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: "400px",
      data: {
        title: "Are you sure?",
        message: "You are about to delete " + deleteDetails.firstGivenName + " " + deleteDetails.lastFamilyName + "."
      }
    });
    // listen to response
    dialogRef.afterClosed().subscribe(dialogResult => {
      // if user pressed yes dialogResult will be true, 
      // if user pressed no - it will be false
      if (dialogResult) {
        this.deleteParentInfo(deleteDetails);
      }
    });
  }
  deleteParentInfo(deleteDetails) {
    this.removeAssociateParent.studentId = deleteDetails.studentId;
    this.removeAssociateParent.studentSchoolId = deleteDetails.schoolId;
    this.removeAssociateParent.parentInfo.parentId = this.parentInfo.parentId;
    this.parentInfoService.removeAssociatedParent(this.removeAssociateParent).subscribe(
      data => {
        if (typeof (data) == 'undefined') {
          this.snackbar.open('Student Information failed. ' + sessionStorage.getItem("httpError"), '', {
            duration: 10000
          });
        }
        else {
         if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          }
          else {
            this.snackbar.open(data._message, '', {
              duration: 10000
            }).afterOpened().subscribe(data => {
              this.getParentDetailsUsingId();
            });

          }
        }
      })
  }



  toggleVisibility() {
    if (this.visible) {
      this.inputType = 'password';
      this.visible = false;
      this.cd.markForCheck();
    } else {
      this.inputType = 'text';
      this.visible = true;
      this.cd.markForCheck();
    }
  }

  ngOnDestroy() {
    this.imageCropperService.enableUpload({ module: this.moduleIdentifier.PARENT, upload: false, mode: this.parentCreate.VIEW });
    this.destroySubject$.next();
    this.destroySubject$.complete();

  }

}
