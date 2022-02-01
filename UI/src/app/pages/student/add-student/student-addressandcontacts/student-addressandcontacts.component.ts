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

import { Component, OnInit, Input, AfterViewInit, OnDestroy, ViewChild } from '@angular/core';
import { FormControl, NgForm, Validators } from '@angular/forms';
import { fadeInUp400ms } from '../../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../../@vex/animations/stagger.animation';
import { fadeInRight400ms } from '../../../../../@vex/animations/fade-in-right.animation';
import { StudentService } from '../../../../services/student.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from '../../../../services/common.service';
import { StudentAddModel } from '../../../../models/student.model';
import { CountryModel } from '../../../../models/country.model';
import * as _moment from 'moment';
import { default as _rollupMoment } from 'moment';
import { SchoolCreate } from '../../../../enums/school-create.enum';
import icCheckBox from '@iconify/icons-ic/check-box';
import icCheckBoxOutlineBlank from '@iconify/icons-ic/check-box-outline-blank';
import icEdit from '@iconify/icons-ic/edit';
import { ImageCropperService } from '../../../../services/image-cropper.service';
import { MiscModel } from '../../../../models/misc-data-student.model';
import { ModuleIdentifier } from '../../../../enums/module-identifier.enum';
import { Permissions, RolePermissionListViewModel, RolePermissionViewModel } from '../../../../models/roll-based-access.model';
import { CryptoService } from '../../../../services/Crypto.service';
import { DefaultValuesService } from '../../../../common/default-values.service';
import { ReplaySubject, Subject } from 'rxjs';
import { MatSelect } from '@angular/material/select';
import { takeUntil } from 'rxjs/operators';
import { PageRolesPermission } from '../../../../common/page-roles-permissions.service';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '../../../shared-module/confirm-dialog/confirm-dialog.component';
@Component({
  selector: 'vex-student-addressandcontacts',
  templateUrl: './student-addressandcontacts.component.html',
  styleUrls: ['./student-addressandcontacts.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms,
    fadeInRight400ms
  ],
})
export class StudentAddressandcontactsComponent implements OnInit, OnDestroy, AfterViewInit {
  @Input() studentDetailsForViewAndEdit;
  @ViewChild('f') currentForm: NgForm;
  @ViewChild('checkBox') checkBox;
  f: NgForm;
  nameOfMiscValuesForView: MiscModel = new MiscModel();
  icEdit = icEdit;
  icCheckBox = icCheckBox;
  icCheckBoxOutlineBlank = icCheckBoxOutlineBlank;
  countryListArr = [];
  countryName = '-';
  mailingAddressCountry = '-';
  countryModel: CountryModel = new CountryModel();
  data;
  studentCreate = SchoolCreate;
  @Input() studentCreateMode: SchoolCreate;
  studentAddModel: StudentAddModel = new StudentAddModel();
  languageList;
  checkBoxChecked = false;
  actionButtonTitle = 'submit';
  cloneStudentAddModel;
  protected _onDestroy = new Subject<void>();
  homeAddressCountryCtrl: FormControl = new FormControl();
  homeAddressCountryFilterCtrl: FormControl = new FormControl();
  public filteredHomeAddressCountry: ReplaySubject<any> = new ReplaySubject<any>(1);
  @ViewChild('singleSelect') singleSelect: MatSelect;
  mailingAddressCountryCtrl: FormControl = new FormControl();
  mailingAddressCountryFilterCtrl: FormControl = new FormControl();
  public filteredMailingAddressCountry: ReplaySubject<any> = new ReplaySubject<any>(1);
  permissions: Permissions;
  constructor(
    public translateService: TranslateService,
    private snackbar: MatSnackBar,
    private dialog: MatDialog,
    private studentService: StudentService,
    private commonService: CommonService,
    private cryptoService: CryptoService,
    private defaultValuesService: DefaultValuesService,
    private pageRolePermissions: PageRolesPermission,
    private imageCropperService: ImageCropperService
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
    this.permissions = this.pageRolePermissions.checkPageRolePermission();
    this.getAllCountry();
    if (this.studentCreateMode === this.studentCreate.VIEW) {
      this.studentService.changePageMode(this.studentCreateMode);
      this.data = this.studentDetailsForViewAndEdit?.studentMaster;
      this.studentAddModel = this.studentDetailsForViewAndEdit;
      this.cloneStudentAddModel = JSON.stringify(this.studentAddModel);
    } else {
      this.studentService.changePageMode(this.studentCreateMode);
      this.studentAddModel = this.studentService.getStudentDetails();
      this.cloneStudentAddModel = JSON.stringify(this.studentAddModel);
      this.data = this.studentAddModel?.studentMaster;
    }
  }
  ngAfterViewInit() {
    this.homeAddressCountryValueChange();
    this.mailingAddressCountryValueChange();
  }
  filterHomeAddressCountry() {
    if (!this.countryListArr) {
      return;
    }
    let search = this.homeAddressCountryFilterCtrl.value;
    if (!search) {
      this.filteredHomeAddressCountry.next(this.countryListArr.slice());
      return;
    }
    else {
      search = search.toLowerCase();
    }
    this.filteredHomeAddressCountry.next(
      this.countryListArr.filter(country => country.name.toLowerCase().indexOf(search) > -1)
    );
  }
  filterMailingAddressCountry() {
    if (!this.countryListArr) {
      return;
    }
    let search = this.mailingAddressCountryFilterCtrl.value;
    if (!search) {
      this.filteredMailingAddressCountry.next(this.countryListArr.slice());
      return;
    }
    else {
      search = search.toLowerCase();
    }
    this.filteredMailingAddressCountry.next(
      this.countryListArr.filter(country => country.name.toLowerCase().indexOf(search) > -1)
    );
  }
  homeAddressCountryValueChange() {
    this.homeAddressCountryFilterCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterHomeAddressCountry();
      });
  }
  mailingAddressCountryValueChange() {
    this.mailingAddressCountryFilterCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe(() => {
        this.filterMailingAddressCountry();
      });
  }



  editAddressContactInfo() {
    this.studentCreateMode = this.studentCreate.EDIT;
    this.studentService.changePageMode(this.studentCreateMode);
    this.actionButtonTitle = 'update';
    this.getAllCountry();
    if( this.studentAddModel.studentMaster.homeAddressCountry == 0 || this.studentAddModel.studentMaster.homeAddressCountry === null){
      this.studentAddModel.studentMaster.homeAddressCountry = null;
    } else {
      this.studentAddModel.studentMaster.homeAddressCountry = +this.studentAddModel.studentMaster.homeAddressCountry;
    }
    this.studentAddModel.studentMaster.mailingAddressCountry = +this.studentAddModel.studentMaster.mailingAddressCountry;
  }

  cancelEdit() {
    if (JSON.stringify(this.studentAddModel) !== this.cloneStudentAddModel) {
      this.studentAddModel = JSON.parse(this.cloneStudentAddModel);
      this.studentDetailsForViewAndEdit = JSON.parse(this.cloneStudentAddModel);
      this.studentService.sendDetails(JSON.parse(this.cloneStudentAddModel));
    }
    this.findCountryNameById();
    this.studentCreateMode = this.studentCreate.VIEW;
    this.studentService.changePageMode(this.studentCreateMode);
    this.data = this.studentAddModel.studentMaster;
    this.imageCropperService.cancelImage('student');
  }

  copyHomeAddress(check) {
    if (this.studentAddModel.studentMaster.mailingAddressSameToHome === false || this.studentAddModel.studentMaster.mailingAddressSameToHome === null) {
      if (this.studentAddModel.studentMaster.homeAddressLineOne !== undefined && this.studentAddModel.studentMaster.homeAddressCity !== undefined &&
        this.studentAddModel.studentMaster.homeAddressState !== undefined && this.studentAddModel.studentMaster.homeAddressZip !== undefined) {
        this.studentAddModel.studentMaster.mailingAddressLineOne = this.studentAddModel.studentMaster.homeAddressLineOne;
        this.studentAddModel.studentMaster.mailingAddressLineTwo = this.studentAddModel.studentMaster.homeAddressLineTwo;
        this.studentAddModel.studentMaster.mailingAddressCity = this.studentAddModel.studentMaster.homeAddressCity;
        this.studentAddModel.studentMaster.mailingAddressState = this.studentAddModel.studentMaster.homeAddressState;
        this.studentAddModel.studentMaster.mailingAddressZip = this.studentAddModel.studentMaster.homeAddressZip;
        this.studentAddModel.studentMaster.mailingAddressCountry = +this.studentAddModel.studentMaster.homeAddressCountry;

      } else {
        this.checkBoxChecked = check ? true : false;
        this.snackbar.open('Please Provide All Mandatory Fields First', '', {
          duration: 10000
        });
      }

    } else {
      this.studentAddModel.studentMaster.mailingAddressLineOne = '';
      this.studentAddModel.studentMaster.mailingAddressLineTwo = '';
      this.studentAddModel.studentMaster.mailingAddressCity = '';
      this.studentAddModel.studentMaster.mailingAddressState = '';
      this.studentAddModel.studentMaster.mailingAddressZip = '';
      this.studentAddModel.studentMaster.mailingAddressCountry = null;
    }
  }
  getAllCountry() {
    this.commonService.GetAllCountry(this.countryModel).subscribe(data => {
      if (data) {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          this.countryListArr = [];
          if (!data.tableCountry) {
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          }
        } else {
          this.filteredHomeAddressCountry.next(data.tableCountry?.slice());
          this.countryListArr = data.tableCountry?.sort((a, b) => a.name < b.name ? -1 : 1);
          if (this.studentCreateMode === this.studentCreate.VIEW) {
            this.findCountryNameById();
          }
        }
      } else {
        this.countryListArr = [];
      }
    });
  }

  findCountryNameById() {
    this.countryListArr.map((val) => {
      const countryInNumber = +this.data.homeAddressCountry;
      const mailingAddressCountry = +this.data.mailingAddressCountry;
      if (val.id === countryInNumber) {
        this.nameOfMiscValuesForView.countryName = val.name;
      }
      if (val.id === mailingAddressCountry) {
        this.nameOfMiscValuesForView.mailingAddressCountry = val.name;
      }
    });
  }

  checkBoxCheckInEditMode() {
    if (this.checkBox?.checked) {
      this.studentAddModel.studentMaster.mailingAddressLineOne = this.studentAddModel.studentMaster.homeAddressLineOne;
      this.studentAddModel.studentMaster.mailingAddressLineTwo = this.studentAddModel.studentMaster.homeAddressLineTwo;
      this.studentAddModel.studentMaster.mailingAddressCity = this.studentAddModel.studentMaster.homeAddressCity;
      this.studentAddModel.studentMaster.mailingAddressState = this.studentAddModel.studentMaster.homeAddressState;
      this.studentAddModel.studentMaster.mailingAddressZip = this.studentAddModel.studentMaster.homeAddressZip;
      this.studentAddModel.studentMaster.mailingAddressCountry = +this.studentAddModel.studentMaster.homeAddressCountry;
    }
  }

  confirmAddStudent() {
    let mode = 'add';
    if (this.studentCreateMode === this.studentCreate.EDIT) {
      mode = 'update';
    }
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: "400px",
      disableClose: true,
      data: {
        title: "Are you sure?",
        message: "You are about to " + mode + " duplicate data"
      }
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult) {
        this.studentAddModel.allowDuplicate = true;
        this.submit();
      }
    });
  }

  submit() {
    this.checkBoxCheckInEditMode();
    this.studentService.UpdateStudent(this.studentAddModel).subscribe(data => {
      if (data) {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          if (data.checkDuplicate === 1) {
            this.confirmAddStudent();
          }
          else {
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          }
        } else {
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
          this.studentService.setStudentCloneImage(data.studentMaster.studentPhoto);
          data.studentMaster.studentPhoto = null;
          this.data = data.studentMaster;
          this.studentAddModel = data;
          this.cloneStudentAddModel = JSON.stringify(data);
          this.studentDetailsForViewAndEdit = data;
          this.findCountryNameById();
          this.studentCreateMode = this.studentCreate.VIEW;
          this.studentService.changePageMode(this.studentCreateMode);
        }
      }
      else {
        this.snackbar.open(this.defaultValuesService.translateKey('studentUpdationfailed') +
          this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  ngOnDestroy() {
    if(this.studentService.getStudentId()){
    if (JSON.stringify(this.studentAddModel) !== this.cloneStudentAddModel) {
      this.studentAddModel = JSON.parse(this.cloneStudentAddModel);
      this.studentDetailsForViewAndEdit = JSON.parse(this.cloneStudentAddModel);
      this.studentService.sendDetails(JSON.parse(this.cloneStudentAddModel));
    }
  }
  }
}
