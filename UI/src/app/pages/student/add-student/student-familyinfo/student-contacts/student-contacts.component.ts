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

import { Component, OnInit, Input, SimpleChanges } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { fadeInUp400ms } from '../../../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../../../@vex/animations/stagger.animation';
import { fadeInRight400ms } from '../../../../../../@vex/animations/fade-in-right.animation';
import { TranslateService } from '@ngx-translate/core';
import icEdit from '@iconify/icons-ic/twotone-edit';
import icDelete from '@iconify/icons-ic/twotone-delete';
import icAdd from '@iconify/icons-ic/baseline-add';
import icRemove from '@iconify/icons-ic/remove-circle';
import { MatDialog } from '@angular/material/dialog';
import { EditContactComponent } from '../edit-contact/edit-contact.component';
import { GetAllParentInfoModel, AddParentInfoModel, RemoveAssociateParent } from '../../../../../models/parent-info.model';
import { ParentInfoService } from '../../../../../services/parent-info.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ConfirmDialogComponent } from '../../../../shared-module/confirm-dialog/confirm-dialog.component';
import { Permissions, RolePermissionListViewModel, RolePermissionViewModel } from '../../../../../models/roll-based-access.model';
import { CryptoService } from '../../../../../services/Crypto.service';
import { DefaultValuesService } from '../../../../../common/default-values.service';
import { PageRolesPermission } from '../../../../../common/page-roles-permissions.service';
import { CommonService } from '../../../../../services/common.service';
import { CountryModel } from '../../../../../models/country.model';
import { LovList } from '../../../../../models/lov.model';
import { takeUntil } from 'rxjs/operators';
import { ReplaySubject, Subject } from 'rxjs';
import { CommonLOV } from '../../../../../pages/shared-module/lov/common-lov';
@Component({
  selector: 'vex-student-contacts',
  templateUrl: './student-contacts.component.html',
  styleUrls: ['./student-contacts.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms,
    fadeInRight400ms
  ]
})
export class StudentContactsComponent implements OnInit {
  @Input() studentDetailsForViewAndEditData;
  @Input() multipleData;
  @Input() parentInfoListForView;
  icEdit = icEdit;
  icDelete = icDelete;
  icAdd = icAdd;
  icRemove = icRemove;
  parentListArray = [];
  suffixList = [];
  salutationList = [];
  relationshipList = [];
  destroySubject$: Subject<void> = new Subject();
  contactType = 'Primary';
  getAllParentInfoModel: GetAllParentInfoModel = new GetAllParentInfoModel();
  lovListViewModel: LovList = new LovList();
  addParentInfoModel: AddParentInfoModel = new AddParentInfoModel();
  removeAssociateParent: RemoveAssociateParent = new RemoveAssociateParent();
  countryModel: CountryModel = new CountryModel();
  permissions: Permissions;
  countryListArr: any[];
  countryCtrl: FormControl = new FormControl('', [Validators.required]);
  public filteredCountry: ReplaySubject<any> = new ReplaySubject<any>(1);
  data: any;
  mode: string;
  viewData: any;
  disableAddButtonFlag:boolean = false;
  constructor(
    private fb: FormBuilder, private dialog: MatDialog,
    public translateService: TranslateService,
    public parentInfoService: ParentInfoService,
    private cryptoService: CryptoService,
    private defaultValuesService: DefaultValuesService,
    private pageRolePermissions: PageRolesPermission,
    private snackbar: MatSnackBar,
    private commonLOV: CommonLOV,
    private commonService: CommonService,
  ) { }

  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission();
    // console.log(this.parentListArray);
    // if(this.parentListArray.length === 0) {
    //   this.viewParentListForStudent();
    // }
    // this.parentListArray = this.parentInfoListForView;
    this.checkAndValidateData();
  }

  ngOnChanges(changes: SimpleChanges) {    
    if (changes.parentInfoListForView && changes.parentInfoListForView.currentValue) {
      this.parentListArray = [];
      this.parentListArray = changes.parentInfoListForView.currentValue;
    }
  }

  openAddNew(ctype) {
    this.dialog.open(EditContactComponent, {
      data: {
        contactType: ctype,
        studentDetailsForViewAndEditData: this.studentDetailsForViewAndEditData,
        contactModalData: {
          countryListArr: this.multipleData.countryList,
          relationshipList: this.multipleData.RelationshipLOV,
          salutationList: this.multipleData.SalutationLOV,
          suffixList: this.multipleData.SuffixLOV
        },
        mode: 'add'
      },
      width: '600px'
    }).afterClosed().subscribe(data => {
      if (data) {
        this.viewParentListForStudent();
      }
    });
  }

  openViewDetails(parentInfo) {

    this.dialog.open(EditContactComponent, {
      data: {
        contactType: this.contactType,
        studentDetailsForViewAndEditData: this.studentDetailsForViewAndEditData,
        parentInfo: parentInfo,
        mode: 'view'
      },
      width: '600px'
    });
  }

  editParentInfo(parentInfo) {
    this.dialog.open(EditContactComponent, {
      data: {
        parentInfo: parentInfo,
        studentDetailsForViewAndEditData: this.studentDetailsForViewAndEditData,
        contactModalData: {
          countryListArr: this.multipleData.countryList,
          relationshipList: this.multipleData.RelationshipLOV,
          salutationList: this.multipleData.SalutationLOV,
          suffixList: this.multipleData.SuffixLOV
        },
        mode: 'edit'
      },
      width: '600px'
    }).afterClosed().subscribe(data => {
      if (data) {
        this.viewParentListForStudent();
      }
    });
  }
  confirmDelete(deleteDetails) {
    // call our modal window
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: {
        title: 'Are you sure?',
        message: 'You are about to delete ' + deleteDetails.firstname + ' ' + deleteDetails.lastname + '.'
      }
    });
    // listen to response
    dialogRef.afterClosed().subscribe(dialogResult => {
      // if user pressed yes dialogResult will be true,
      // if user pressed no - it will be false
      if (dialogResult) {
        this.deleteParentInfo(deleteDetails.parentId);
      }
    });
  }
  deleteParentInfo(parentId) {
    this.removeAssociateParent.parentInfo.parentId = parentId;
    this.removeAssociateParent.studentSchoolId = this.studentDetailsForViewAndEditData.studentMaster.schoolId;
    this.removeAssociateParent.studentId = this.studentDetailsForViewAndEditData.studentMaster.studentId;
    this.parentInfoService.removeAssociatedParent(this.removeAssociateParent).subscribe(
      data => {
        if (data) {
          if (data._failure) {
            
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          }
          else {
            this.snackbar.open(data._message, '', {
              duration: 10000
            }).afterOpened().subscribe(() => {
              this.viewParentListForStudent();
            });
          }
        }
        else {
          this.snackbar.open(this.defaultValuesService.translateKey('parentInformationFailed') + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      });
  }
  viewParentListForStudent() {
    this.getAllParentInfoModel.studentId = this.studentDetailsForViewAndEditData.studentMaster.studentId;
    this.parentInfoService.viewParentListForStudent(this.getAllParentInfoModel).subscribe(
      data => {
        if (data) {
          this.parentListArray = [];
          this.contactType = 'Primary';
          if (data._failure) {
            
            if (!data.parentInfoListForView) {
              this.snackbar.open(data._message, '', {
                duration: 10000
              });
            }
          }
          else {
            this.parentListArray = data.parentInfoListForView;
            this.checkAndValidateData();
          }
        }
        else {
          this.snackbar.open(this.defaultValuesService.translateKey('parentInformationFailed') + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      });
  }

  checkAndValidateData() {
    let var1 = 0;
    let var2 = 0;
    let var3 = 0;
    this.parentListArray.forEach(val => {
      if (val.contactType === 'Primary') {
        var1++;
      } else if (val.contactType === 'Secondary') {
        var2++;
      } else if (val.contactType === 'Other') {
        var3++;
      }
    });
    if (var1 > 0 && var2 > 0) {
      this.contactType = 'Other';
    } else if (var1 > 0) {
      this.contactType = 'Secondary';
    } else {
      this.contactType = 'Primary';
    }
    this.disableAddButtonFlag = this.checkContactList(var1,var2,var3);
  }

  checkContactList(primaryCount,secondaryCount,otherCount) {
    if (primaryCount>0 && secondaryCount>0 && otherCount===6) {
      return true;
    } else if (primaryCount>0 && secondaryCount===0 && otherCount===7) {
      return true;
    } else {
      return false;
    }
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
