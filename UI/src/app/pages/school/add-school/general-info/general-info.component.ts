import { ValidationService } from './../../../shared/validation.service';
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

import { Component, OnInit, Input, ViewChild, AfterViewInit, OnDestroy, ElementRef } from '@angular/core';
import { FormControl, NgForm, Validators } from '@angular/forms';
import { CheckSchoolInternalIdViewModel, SchoolAddViewModel } from '../../../../models/school-master.model';
import { fadeInUp400ms } from '../../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../../@vex/animations/stagger.animation';
import { fadeInRight400ms } from '../../../../../@vex/animations/fade-in-right.animation';
import { SchoolService } from '../../../../services/school.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import * as _moment from 'moment';
import { default as _rollupMoment } from 'moment';
import { MomentDateAdapter } from '@angular/material-moment-adapter';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { TranslateService } from '@ngx-translate/core';
import { WashInfoEnum } from '../../../../enums/wash-info.enum';
import { status } from '../../../../enums/wash-info.enum';
import { MY_FORMATS } from '../../../shared/format-datepicker';
import { __values } from 'tslib';
import { ReplaySubject, Subject } from 'rxjs';
import { CountryModel } from '../../../../models/country.model';
import { StateModel } from '../../../../models/state.model';
import { CityModel } from '../../../../models/city.model';
import { CommonService } from '../../../../services/common.service';
import { SharedFunction } from '../../../shared/shared-function';
import { ImageCropperService } from '../../../../services/image-cropper.service';
import { CustomFieldListViewModel } from '../../../../models/custom-field.model';
import { SchoolCreate } from '../../../../enums/school-create.enum';
import { debounceTime, distinctUntilChanged, take, takeUntil } from 'rxjs/operators';
import { LovList } from '../../../../models/lov.model';
import icEdit from '@iconify/icons-ic/twotone-edit';
import { CommonLOV } from '../../../shared-module/lov/common-lov';
import { ModuleIdentifier } from '../../../../enums/module-identifier.enum';
import { Permissions, RolePermissionListViewModel, RolePermissionViewModel } from '../../../../models/roll-based-access.model';
import { RollBasedAccessService } from '../../../../services/roll-based-access.service';
import { Router } from '@angular/router';
import { CryptoService } from '../../../../services/Crypto.service';
import { MatSelect } from '@angular/material/select';
import { DefaultValuesService } from '../../../../common/default-values.service';
import { PageRolesPermission } from '../../../../common/page-roles-permissions.service';
import { Module } from 'src/app/enums/module.enum';
import { MatDialog } from "@angular/material/dialog";
import { CaptureDateComponent } from '../capture-date/capture-date.component';

@Component({
  selector: 'vex-general-info',
  templateUrl: './general-info.component.html',
  styleUrls: ['./general-info.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms,
    fadeInRight400ms
  ],
  providers: [
    { provide: DateAdapter, useClass: MomentDateAdapter, deps: [MAT_DATE_LOCALE] },
    { provide: MAT_DATE_FORMATS, useValue: MY_FORMATS },
  ],
})

export class GeneralInfoComponent implements OnInit, AfterViewInit, OnDestroy {
  icEdit = icEdit;
  isMarkingPeriod: boolean = false;
  schoolCreate = SchoolCreate;
  @ViewChild('f') currentForm: NgForm;
  schoolCreateMode: SchoolCreate;
  // schoolDetailsForViewAndEdit;
  categoryId;
  editPermission = false;
  permissionList = [];
  permissionListViewModel: RolePermissionListViewModel = new RolePermissionListViewModel();
  permissionGroup: RolePermissionViewModel = new RolePermissionViewModel();
  moduleIdentifier = ModuleIdentifier;
  cityName: string;
  stateName: string;
  countryName = '';
  schoolLevelOptions;
  schoolClassificationOptions;
  genderOptions = [];
  isSchoolInternalId = false;
  gradeLevel = [];
  destroySubject$: Subject<void> = new Subject();
  customFieldModel = new CustomFieldListViewModel();
  schoolAddViewModel: SchoolAddViewModel = new SchoolAddViewModel();
  checkSchoolInternalIdViewModel: CheckSchoolInternalIdViewModel = new CheckSchoolInternalIdViewModel();
  countryModel: CountryModel = new CountryModel();
  stateModel: StateModel = new StateModel();
  cityModel: CityModel = new CityModel();
  lovList: LovList = new LovList();
  countryListArr = [];
  stateListArr = [];
  cityListArr = [];
  schoolInternalId = '';
  module = Module.SCHOOL;
  generalInfo = WashInfoEnum;
  statusInfo = status;
  city: number;
  f: NgForm;
  stateCount: number;
  minDate: string | Date;
  selectedLowGradeLevelIndex: number;
  selectedHighGradeLevelIndex: number;
  // formActionButtonTitle = 'submit';
  internalId: FormControl;
  cloneSchool: string;
  countryCtrl: FormControl = new FormControl('', [Validators.required]);
  countryFilterCtrl: FormControl = new FormControl();
  public filteredCountry: ReplaySubject<any> = new ReplaySubject<any>(1);
  @ViewChild('singleSelect') singleSelect: MatSelect;
  protected _onDestroy = new Subject<void>();
  permissions: Permissions;
  constructor(
    private schoolService: SchoolService,
    private el: ElementRef,
    private snackbar: MatSnackBar,
    public translateService: TranslateService,
    private commonService: CommonService,
    private commonFunction: SharedFunction,
    private imageCropperService: ImageCropperService,
    private defaultValuesService: DefaultValuesService,
    private commonLOV: CommonLOV,
    private pageRolePermission: PageRolesPermission,
    private router: Router,
    private dialog: MatDialog,
    public rollBasedAccessService: RollBasedAccessService,
    private cryptoService: CryptoService
  ) {
    this.internalId = new FormControl('', Validators.required);

    // this.schoolCreateMode = this.router.getCurrentNavigation().extras?.state ? this.router.getCurrentNavigation().extras.state.type : SchoolCreate.ADD;
    //translateService.use('en');
    // this.schoolService.getSchoolDetailsForGeneral.pipe(takeUntil(this.destroySubject$)).subscribe((res: SchoolAddViewModel) => {
    //   this.schoolAddViewModel = res;
    //   this.isMarkingPeriod = res.isMarkingPeriod;
    //   this.cloneSchool = JSON.stringify(res);
    //   this.schoolDetailsForViewAndEdit = res;
    //   this.schoolInternalId = res.schoolMaster.schoolInternalId;
    // });
  }
  protected setInitialValue() {
    this.filteredCountry
      .pipe(take(1), takeUntil(this._onDestroy))
      .subscribe(() => {
        this.singleSelect.compareWith = (a, b) => a && b && a.name === b.name;
      });
  }
  ngOnInit(): void {
    this.permissions = this.pageRolePermission.checkPageRolePermission(null, null, true);
    if(!this.permissions.view) return;
    this.schoolService.schoolCreatedMode.subscribe((res) => {
      this.schoolCreateMode = res;
    });

    this.schoolService.schoolDetailsForViewedAndEdited.pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      if(res) {
        this.schoolAddViewModel = res;
        this.countryCtrl.setValue(res.schoolMaster.country);
        this.internalId.setValue(res.schoolMaster.schoolInternalId);
        this.internalId.setErrors(null);


        if (this.schoolCreateMode === this.schoolCreate.ADD) {
          this.initializeDropdownsForSchool();
          this.getAllCountry();
          this.openCaptureDate()
        } else if (this.schoolCreateMode === this.schoolCreate.VIEW) {
          this.cloneSchool = JSON.stringify(this.schoolAddViewModel);
          this.imageCropperService.enableUpload({ module: this.moduleIdentifier.SCHOOL, upload: true, mode: this.schoolCreate.VIEW });
        }
        else if (this.schoolCreateMode === this.schoolCreate.EDIT && this.schoolAddViewModel) {
          this.getAllCountry();
          this.initializeDropdownsForSchool();
          this.cloneSchool = JSON.stringify(this.schoolAddViewModel);
        }
      }
      
    });

    this.genderOptions = ['Male', 'Female', 'Co-education'];
   
  }
  filterContry() {
    if (!this.countryListArr) {
      return;
    }
    let search = this.countryFilterCtrl.value;
    if (!search) {
      this.filteredCountry.next(this.countryListArr.slice());
      return;
    }
    else {
      search = search.toLowerCase();
    }
    this.filteredCountry.next(
      this.countryListArr.filter(country => country.name.toLowerCase().indexOf(search) > -1)
    );
  }
  countryValueChange() {
    this.countryFilterCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe((res) => {
        this.filterContry();
      });
  }

  initializeDropdownsForSchool() {
    if (this.schoolCreateMode === this.schoolCreate.EDIT){
      this.commonLOV.getValueForEditSchool(true);
    }

    this.commonLOV.getLovByName('School Level', this.schoolCreateMode === this.schoolCreate.ADD).pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      this.schoolLevelOptions = res;
    });
    this.commonLOV.getLovByName('School Classification', this.schoolCreateMode === this.schoolCreate.ADD).pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      this.schoolClassificationOptions = res;
    });
    this.commonLOV.getLovByName('Grade Level').pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      this.gradeLevel = res;
      if (this.schoolCreateMode === this.schoolCreate.EDIT) {        
        this.checkGradeLevelsOnEdit();
      }
    });

  }

  ngAfterViewInit() {
    this.countryValueChange();
    // this.internalId.setErrors({ 'nomatch': false });
    this.internalId.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term !== '') {
        if (this.schoolAddViewModel.schoolMaster.schoolInternalId === term) {
          this.internalId.setErrors(null);
        }
        else {
          this.isSchoolInternalId = true;
          this.checkSchoolInternalIdViewModel.schoolInternalId = term;
          this.schoolService.checkSchoolInternalId(this.checkSchoolInternalIdViewModel).pipe(debounceTime(500), distinctUntilChanged()).subscribe(data => {
            if(data._failure){
              this.commonService.checkTokenValidOrNot(data._message);
            }
            else {
              if (data.isValidInternalId) {
                this.internalId.setErrors(null);
                this.isSchoolInternalId = false;
              } else {
                this.internalId.markAsTouched();
                this.internalId.setErrors({ nomatch: true });
                this.isSchoolInternalId = false;
              }
            }
          });
        }
      } else {
        this.internalId.markAsTouched();
        this.isSchoolInternalId = false;
      }
    });
  }

  editGeneralInfo() {
    this.schoolCreateMode = this.schoolCreate.EDIT;
    this.schoolService.setSchoolCreateMode(this.schoolCreateMode);
    this.getAllCountry();
    this.initializeDropdownsForSchool();
    this.imageCropperService.enableUpload({ module: this.moduleIdentifier.SCHOOL, upload: true, mode: this.schoolCreate.EDIT });
  }
  cancelEdit() {
    this.schoolCreateMode = this.schoolCreate.VIEW;
    // this.imageCropperService.cancelImage('school');
    this.imageCropperService.enableUpload({ module: this.moduleIdentifier.SCHOOL, upload: true, mode: this.schoolCreate.VIEW });
    if (JSON.stringify(this.schoolAddViewModel) !== this.cloneSchool) {
      this.schoolAddViewModel = JSON.parse(this.cloneSchool);
      // this.schoolDetailsForViewAndEdit = this.schoolAddViewModel;
      this.schoolService.sendDetails(this.schoolAddViewModel);
    }
    this.schoolService.setSchoolCreateMode(this.schoolCreateMode);
  }

  checkLowGradeLevel(event) {
    const index = this.gradeLevel?.findIndex((val) => {
      return val.lovColumnValue === event.value;
    });
    this.selectedLowGradeLevelIndex = index;
    if (index === -1) {
      this.currentForm.form.controls.lowestGradeLevel.markAsTouched();
    } else {
      if (index > this.selectedHighGradeLevelIndex) {
        this.currentForm.form.controls.lowestGradeLevel.setErrors({ nomatch: true });
      } else {
        this.currentForm.controls.lowestGradeLevel.setErrors(null);
        if (this.currentForm.controls.highestGradeLevel.errors != null) {
          this.currentForm.form.get('highestGradeLevel').setErrors(null);
          this.currentForm.form.controls.highestGradeLevel.markAsUntouched();
        }
      }
    }
  }

  checkHighGradeLevel(event) {
    const index = this.gradeLevel?.findIndex((val) => {
      return val.lovColumnValue === event.value;
    });
    this.selectedHighGradeLevelIndex = index;
    if (index === -1) {
      this.currentForm.form.controls.highestGradeLevel.markAsTouched();
    } else {
      if (this.selectedLowGradeLevelIndex > index) {
        this.currentForm.form.controls.highestGradeLevel.setErrors({ nomatch: true });
      } else {
        this.currentForm.controls.highestGradeLevel.setErrors(null);
        if (this.currentForm.form.controls.lowestGradeLevel.errors != null) {
          this.currentForm.form.get('lowestGradeLevel').setErrors(null);
          this.currentForm.form.controls.lowestGradeLevel.markAsUntouched();
        }
      }
    }
  }

  checkGradeLevelsOnEdit() {
    const lowGradeIndex = this.gradeLevel?.findIndex((val) => {
      return val.lovColumnValue === this.schoolAddViewModel.schoolMaster.schoolDetail[0].lowestGradeLevel;
    });
    this.selectedLowGradeLevelIndex = lowGradeIndex;
    const highGradeIndex = this.gradeLevel?.findIndex((val) => {
      return val.lovColumnValue === this.schoolAddViewModel.schoolMaster.schoolDetail[0].highestGradeLevel;
    });
    this.selectedHighGradeLevelIndex = highGradeIndex;
  }

  dateCompare() {
    this.schoolAddViewModel.schoolMaster.schoolDetail[0].dateSchoolClosed = null;
    this.minDate = this.schoolAddViewModel.schoolMaster.schoolDetail[0].dateSchoolOpened;
    const minDate = new Date(this.minDate);
    this.minDate = new Date(minDate.setDate(minDate.getDate() + 1));
  }

  getAllCountry() {
    this.commonService.GetAllCountry(this.countryModel).pipe(takeUntil(this.destroySubject$)).subscribe(data => {
      if (typeof (data) === 'undefined') {
        this.countryListArr = [];
      }
      else {
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          if (data.tableCountry) {
            this.countryListArr = [];
          } else {
            this.countryListArr = [];
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          }
        } else {
          // this.countryCtrl.setValue(data.tableCountry?.sort((a, b) => a.name < b.name ? -1 : 1));
          this.filteredCountry.next(data.tableCountry?.slice());
          this.countryListArr = data.tableCountry?.sort((a, b) => a.name < b.name ? -1 : 1);
          this.stateCount = data.stateCount;
          if (this.schoolCreateMode === SchoolCreate.VIEW) {
            // this.findCountryNameByIdOnViewMode(); //No Need Now, because we will send the name directly
          }
        }
      }
    });
  }

  findCountryNameByIdOnViewMode() {
    const index = this.countryListArr.findIndex((x) => {
      return x.id === +this.schoolAddViewModel.schoolMaster.country;
    });
    this.countryName = this.countryListArr[index]?.name;
  }

  getAllStateByCountry(data) {
    if (this.stateCount > 0) {
      if ((this.commonFunction.checkEmptyObject(this.schoolAddViewModel) === true)
        || (this.commonFunction.checkEmptyObject(this.schoolAddViewModel) === true)
        || (this.commonFunction.checkEmptyObject(this.schoolAddViewModel) === true)) {

        if (data.value === '') {
          this.stateModel.countryId = 0;
          this.countryName = '';
          this.cityListArr = [];
          this.stateListArr = [];
        } else {
          if (data.value === undefined && data) {
            this.stateModel.countryId = data;
            this.countryName = data.toString();
          } else {
            this.stateModel.countryId = data.value;
            this.countryName = data.value.toString();
          }
        }
      } else {
        if (data.value === '') {
          this.stateModel.countryId = 0;
          this.countryName = '';
          this.cityListArr = [];
          this.stateListArr = [];
        } else {
          this.stateModel.countryId = data.value;
          this.countryName = data.value.toString();
        }
      }

      if (this.stateModel.countryId !== 0) {

        this.commonService.GetAllState(this.stateModel).subscribe(data => {
          if (typeof (data) === 'undefined') {
            this.stateListArr = [];
          }
          else {
           if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
              this.stateListArr = [];

            } else {
              this.cityListArr = [];
              this.stateListArr = data.tableState;

            }
          }

        });
      }
    }
  }
  getAllCitiesByState(data) {
    if ((this.commonFunction.checkEmptyObject(this.schoolAddViewModel) === true)
      || (this.commonFunction.checkEmptyObject(this.schoolAddViewModel) === true)
      || (this.commonFunction.checkEmptyObject(this.schoolAddViewModel) === true)) {
      if (data.value === '') {
        this.cityModel.stateId = 0;
        this.stateName = '';
        this.cityListArr = [];
      } else {
        if (data.value === undefined && data) {
          this.cityModel.stateId = data;
          this.stateName = data.toString();
        } else {
          this.cityModel.stateId = data.value;
          this.stateName = data.value.toString();
        }
      }
    } else {
      if (data.value === '') {
        this.cityModel.stateId = 0;
        this.stateName = '';
        this.cityListArr = [];
      } else {
        this.cityModel.stateId = data.value;
        this.stateName = data.value.toString();
      }
    }
    if (this.cityModel.stateId !== 0) {
      this.commonService.GetAllCity(this.cityModel).subscribe(val => {
        if (typeof (val) === 'undefined') {
          this.cityListArr = [];
        }
        else {
          if (val._failure) {
        this.commonService.checkTokenValidOrNot(val._message);
            this.cityListArr = [];
          } else {
            this.cityListArr = val.tableCity;
          }
        }

      });
    }
  }
  onStatusChange(event: boolean) {
    const schoolClosedDate = this.currentForm.value.date_school_closed;
    if (event === false && (schoolClosedDate === null || schoolClosedDate === undefined)) {
      this.currentForm.controls.date_school_closed.setValidators(Validators.required);
      this.currentForm.controls.date_school_closed.setErrors({ required: true });
      this.currentForm.controls.date_school_closed.markAsTouched();
    } else {
      if (this.currentForm.controls.date_school_closed.errors?.required) {
        this.currentForm.controls.date_school_closed.setErrors(null);
      }
    }
  }

  checkClosedDate() {
    const startDate = new Date(this.schoolAddViewModel.schoolMaster.schoolDetail[0].dateSchoolOpened).getTime();
    const closedDate = new Date(this.schoolAddViewModel.schoolMaster.schoolDetail[0].dateSchoolClosed).getTime();
    if (closedDate <= startDate && startDate !== null && closedDate !== 0) {
      this.currentForm.controls.date_school_closed.setErrors({ nomatch: true });
    } else {
      if (this.currentForm.controls.date_school_closed.errors?.nomatch) {
        this.currentForm.controls.date_school_closed.setErrors(null);
      }
    }
    this.onStatusChange(this.schoolAddViewModel.schoolMaster.schoolDetail[0].status);
  }

  submit() {
    this.internalId.markAsTouched();
    this.countryCtrl.markAsTouched();
    this.currentForm.form.markAllAsTouched();
    if (this.currentForm.form.valid) {
      if (this.countryCtrl.valid) {
        if (this.schoolCreateMode == this.schoolCreate.EDIT) {
          if (this.schoolAddViewModel.schoolMaster.fieldsCategory) {
            this.modifyCustomFields();
          }
          this.updateSchool();
        } else {
          this.addSchool();
        }
      }
    }
  }

  modifyCustomFields() {
    this.schoolAddViewModel.selectedCategoryId = this.schoolAddViewModel.schoolMaster.fieldsCategory[0].categoryId;
    for (const schoolCustomField of this.schoolAddViewModel.schoolMaster.fieldsCategory[0].customFields) {
      if (schoolCustomField.type === 'Multiple SelectBox' && this.schoolService.getSchoolMultiselectValue() !== undefined) {
        schoolCustomField.customFieldsValue[0].customFieldValue = this.schoolService.getSchoolMultiselectValue().toString().replaceAll(',', '|');
      }
    }
  }

  addSchool() {

    if (this.internalId.invalid) {
      this.invalidScroll();
      return;
    }    
    if(!this.schoolAddViewModel.StartDate || !this.schoolAddViewModel.EndDate) {
      this.openCaptureDate();
      this.snackbar.open('Please enter start date and end date.', '', {
        duration: 10000
      });
      return;
    }
    // this.schoolAddViewModel.schoolId= this.defaultValuesService.getSchoolID();
    this.schoolAddViewModel.schoolMaster.schoolInternalId = this.internalId.value;

    this.schoolAddViewModel.schoolMaster.schoolDetail[0].dateSchoolOpened = this.commonFunction.formatDateSaveWithoutTime(this.schoolAddViewModel.schoolMaster.schoolDetail[0].dateSchoolOpened);
    this.schoolAddViewModel.schoolMaster.schoolDetail[0].dateSchoolClosed = this.commonFunction.formatDateSaveWithoutTime(this.schoolAddViewModel.schoolMaster.schoolDetail[0].dateSchoolClosed);
    this.schoolService.AddSchool(this.schoolAddViewModel).pipe(takeUntil(this.destroySubject$)).subscribe(
      (data: any) => {
        if (data) {
         if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          } else {
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
            this.schoolService.setSchoolId(data.schoolMaster.schoolId);
            this.schoolService.setSchoolCloneImage(data.schoolMaster.schoolDetail[0].schoolLogo);
            this.defaultValuesService.setSchoolID(data.schoolMaster.schoolId.toString());
            this.schoolCreateMode = this.schoolCreate.EDIT
            this.schoolService.setSchoolCreateMode(this.schoolCreateMode);
            this.schoolService.changeMessage(true);
            this.schoolService.setSchoolDetails(data);
            this.schoolService.setSchoolDetailsForViewAndEdit(data);
            this.router.navigate(['/school', 'schoolinfo', 'washinfo'],{state: {type: this.schoolCreateMode}});
            this.schoolService.changeCategory(2);
          }
        }
        else {
          this.snackbar.open('General Info Submission failed. ' + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }

      }
    );
  }

  updateSchool() {

    if (this.internalId.invalid) {
      this.invalidScroll();
      return;
    }
    this.schoolAddViewModel.schoolMaster.schoolInternalId = this.internalId.value;
    this.schoolAddViewModel.schoolMaster.schoolDetail[0].dateSchoolOpened = this.commonFunction.formatDateSaveWithoutTime(this.schoolAddViewModel.schoolMaster.schoolDetail[0].dateSchoolOpened);
    this.schoolAddViewModel.schoolMaster.schoolDetail[0].dateSchoolClosed = this.commonFunction.formatDateSaveWithoutTime(this.schoolAddViewModel.schoolMaster.schoolDetail[0].dateSchoolClosed);
    this.schoolService.UpdateSchool(this.schoolAddViewModel).pipe(takeUntil(this.destroySubject$)).subscribe(
      data => {
        if (data) {
         if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          } else {
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
            this.schoolService.setSchoolCloneImage(data.schoolMaster.schoolDetail[0].schoolLogo);
            data.schoolMaster.schoolDetail[0].schoolLogo = null;
            this.schoolCreateMode = this.schoolCreate.VIEW;
            this.cloneSchool = JSON.stringify(data);
            this.schoolService.updateSchoolName(data.schoolMaster.schoolName);
            this.schoolService.setSchoolCreateMode(this.schoolCreateMode);
            this.imageCropperService.enableUpload({ module: this.moduleIdentifier.SCHOOL, upload: true, mode: this.schoolCreate.VIEW });
          }
        }
        else {
          this.snackbar.open('General Info Updation failed. ' + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      });
  }

  invalidScroll() {
    const firstInvalidControl: HTMLElement = this.el.nativeElement.querySelector(
      'input.ng-invalid'
    );
    firstInvalidControl.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }

  openCaptureDate(){
    this.dialog.open(CaptureDateComponent, {
      width: '500px',
      disableClose: true,
      data: {schoolAddViewModel: this.schoolAddViewModel}
    }).afterClosed().subscribe(result => {
      if(result) {
        this.schoolAddViewModel = result;
      }
    });
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
