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

import { Component, OnInit, Input, ViewChild, ChangeDetectorRef, OnDestroy, EventEmitter, Output, AfterViewInit, ElementRef } from '@angular/core';
import { FormControl, FormGroup, NgForm, Validators } from '@angular/forms';
import { fadeInUp400ms } from '../../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../../@vex/animations/stagger.animation';
import { fadeInRight400ms } from '../../../../../@vex/animations/fade-in-right.animation';
import { StudentService } from '../../../../services/student.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { CommonService } from '../../../../services/common.service';
import { CheckStudentInternalIdViewModel, StudentAddModel } from '../../../../models/student.model';
import { CountryModel } from '../../../../models/country.model';
import { LanguageModel } from '../../../../models/language.model';
import { LoginService } from '../../../../services/login.service';
import * as _moment from 'moment';
import { default as _rollupMoment } from 'moment';
import { MomentDateAdapter } from '@angular/material-moment-adapter';
import { DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE } from '@angular/material/core';
import { MY_FORMATS } from '../../../shared/format-datepicker';
import { SharedFunction } from '../../../shared/shared-function';
import { SchoolCreate } from '../../../../enums/school-create.enum';
import icEdit from '@iconify/icons-ic/edit';
import { Subject } from 'rxjs/internal/Subject';
import { auditTime, debounceTime, distinctUntilChanged, takeUntil, shareReplay, take } from 'rxjs/operators';
import icVisibility from '@iconify/icons-ic/twotone-visibility';
import icVisibilityOff from '@iconify/icons-ic/twotone-visibility-off';
import { SectionService } from '../../../../services/section.service';
import { GetAllSectionModel, TableSectionList } from '../../../../models/section.model';
import { CheckUserEmailAddressViewModel } from '../../../../models/user.model';
import { ImageCropperService } from '../../../../services/image-cropper.service';
import { LovList } from '../../../../models/lov.model';
import { MiscModel } from '../../../../models/misc-data-student.model';
import { CommonLOV } from '../../../shared-module/lov/common-lov';
import { ModuleIdentifier } from '../../../../enums/module-identifier.enum';
import { CryptoService } from '../../../../services/Crypto.service';
import { Permissions, RolePermissionListViewModel, RolePermissionViewModel } from '../../../../models/roll-based-access.model';
import { Router } from '@angular/router';
import { DefaultValuesService } from '../../../../common/default-values.service';
import { ReplaySubject } from 'rxjs';
import { MatSelect } from '@angular/material/select';
import { PageRolesPermission } from '../../../../common/page-roles-permissions.service';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '../../../shared-module/confirm-dialog/confirm-dialog.component';
import { ActiveDeactiveUserModel } from 'src/app/models/common.model';
@Component({
  selector: 'vex-student-generalinfo',
  templateUrl: './student-generalinfo.component.html',
  styleUrls: ['./student-generalinfo.component.scss'],
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
export class StudentGeneralinfoComponent implements OnInit, AfterViewInit, OnDestroy {
  icEdit = icEdit;
  icVisibility = icVisibility;
  icVisibilityOff = icVisibilityOff;
  studentCreate = SchoolCreate;
  studentCreateMode;
  studentDetailsForViewAndEdit;
  categoryId = 0;
  @ViewChild('f') currentForm: NgForm;
  data;
  moduleIdentifier = ModuleIdentifier;
  nameOfMiscValuesForView: MiscModel = new MiscModel(); // This Object contains Section Name, Nationality, Country, languages for View Mode.
  countryListArr = [];
  ethnicityList = [];
  raceList = [];
  genderList = [];
  suffixList = [];
  salutationList = [];
  maritalStatusList = [];
  countryModel: CountryModel = new CountryModel();
  destroySubject$: Subject<void> = new Subject();
  form: FormGroup;
  nationalityCtrl: FormControl = new FormControl();
  nationalityFilterCtrl: FormControl = new FormControl();
  public filteredNationality: ReplaySubject<any> = new ReplaySubject<any>(1);
  @ViewChild('singleSelect') singleSelect: MatSelect;
  countryOfBirthCtrl: FormControl = new FormControl();
  countryOfBirthFilterCtrl: FormControl = new FormControl();
  firstLanguageFilterCtrl:FormControl=new FormControl();
  secondLanguageFilterCtrl:FormControl=new FormControl();
  thirdLanguageFilterCtrl:FormControl=new FormControl();  
  public filteredcountryOfBirth: ReplaySubject<any> = new ReplaySubject<any>(1);
  public filteredFirstLanguage: ReplaySubject<any> = new ReplaySubject<any>(1);
  public filteredSecondLanguage: ReplaySubject<any> = new ReplaySubject<any>(1);
  public filteredThirdLanguage: ReplaySubject<any> = new ReplaySubject<any>(1);
  studentAddModel: StudentAddModel = new StudentAddModel();
  checkStudentInternalIdViewModel: CheckStudentInternalIdViewModel = new CheckStudentInternalIdViewModel();
  checkUserEmailAddressViewModel: CheckUserEmailAddressViewModel = new CheckUserEmailAddressViewModel();
  activeDeactiveUserModel: ActiveDeactiveUserModel = new ActiveDeactiveUserModel();
  sectionList: GetAllSectionModel = new GetAllSectionModel();
  languages: LanguageModel = new LanguageModel();
  lovListViewModel: LovList = new LovList();
  protected _onDestroy = new Subject<void>();
  module = 'Student';
  saveAndNext = 'saveAndNext';
  pageStatus: string;
  languageList;
  inputType = 'password';
  studentInternalId = '';
  studentPortalId = '';
  visible = false;
  pass = '';
  isUser = false;
  isStudentInternalId = false;
  hidePasswordAccess = false;
  hideAccess = false;
  fieldDisabled = false;
  internalId: FormControl;
  loginEmail: FormControl;
  today = new Date();
  cloneStudentModel;
  permissions: Permissions = new Permissions();
  @Output() dataAfterSavingGeneralInfo = new EventEmitter<any>();
  constructor(
    private el: ElementRef,
    private dialog: MatDialog,
    public translateService: TranslateService,
    private snackbar: MatSnackBar,
    private studentService: StudentService,
    private commonService: CommonService,
    private loginService: LoginService,
    private commonFunction: SharedFunction,
    private sectionService: SectionService,
    private defaultValuesService: DefaultValuesService,
    private cd: ChangeDetectorRef,
    private router: Router,
    private imageCropperService: ImageCropperService,
    private cryptoService: CryptoService,
    private pageRolePermission: PageRolesPermission,
    private commonLOV: CommonLOV,
    ) {
    //translateService.use('en');
    this.defaultValuesService.checkAcademicYear() && !this.studentService.getStudentId() ? this.studentService.redirectToGeneralInfo() : !this.defaultValuesService.checkAcademicYear() && !this.studentService.getStudentId() ? this.studentService.redirectToStudentList() : '';
  }

  ngOnInit(): void {
    this.getStudentDetails().then(() => {
      this.accessPortal();
      this.GetAllLanguage();
      this.getAllCountry();
    });

    this.studentService.studentCreatedMode.subscribe((res)=>{
      this.studentCreateMode = res;
    })
    this.studentService.studentDetailsForViewedAndEdited.subscribe((res)=>{
      if(res){
      this.studentDetailsForViewAndEdit = res;
      this.studentAddModel = this.studentDetailsForViewAndEdit;
      this.countryOfBirthCtrl.setValue(res?.studentMaster?.countryOfBirth);
      this.nationalityCtrl.setValue(res?.studentMaster?.nationality);
      }
    })
    // this.studentService.categoryIdSelected.subscribe((res)=>{
    //   this.categoryId = res;
    // })
    this.permissions = this.pageRolePermission.checkPageRolePermission();

    this.internalId = new FormControl('');
    this.loginEmail = new FormControl('', Validators.required);
    if (this.studentCreateMode === this.studentCreate.ADD) {

      this.initializeDropdownsInAddMode();
    } else if (this.studentCreateMode === this.studentCreate.VIEW) {
      this.studentService.changePageMode(this.studentCreateMode);
      this.studentAddModel = this.studentDetailsForViewAndEdit;
      this.data = this.studentDetailsForViewAndEdit?.studentMaster;
      if (!this.studentAddModel.studentMaster.studentPortalId) {
        this.data.studentPortalId = this.studentAddModel.loginEmail;
      }
      this.accessPortal();
      this.cloneStudentModel = JSON.stringify(this.studentAddModel);
      if(!this.studentService.getStudentFirstView()){
        this.GetAllLanguage();
        this.getAllCountry();
      }

    } else if (this.studentCreateMode === this.studentCreate.EDIT && (this.studentDetailsForViewAndEdit !== undefined || this.studentDetailsForViewAndEdit !== null)) {
      this.studentAddModel = this.studentDetailsForViewAndEdit;
      this.cloneStudentModel = JSON.stringify(this.studentAddModel);
      this.data = this.studentAddModel.studentMaster;
      if (!this.studentAddModel.studentMaster.studentPortalId) {
        this.data.studentPortalId = this.studentAddModel.loginEmail;
      }
      else {
        this.studentPortalId = this.studentAddModel.studentMaster.studentPortalId;
      }
      this.studentService.changePageMode(this.studentCreateMode);
      this.accessPortal();
      this.initializeDropdownsInAddMode();
      this.saveAndNext = 'update';
      if (this.studentAddModel.studentMaster.studentPortalId) {
        this.hideAccess = true;
        this.fieldDisabled = true;
      }
    }
  }
  filterNationalitis(){
    if (!this.countryListArr) {
      return;
    }
    let search = this.nationalityFilterCtrl.value;
    if (!search) {
      this.filteredNationality.next(this.countryListArr.slice());
      return;
    }
    else {
      search = search.toLowerCase();
    }
    this.filteredNationality.next(
      this.countryListArr.filter(country => country.name.toLowerCase().indexOf(search) > -1)
    );
  }
  filterFirstLanguage(){
    if (!this.languageList) {
      return;
    }
    let search = this.firstLanguageFilterCtrl.value;
    if (!search) {
      this.filteredFirstLanguage.next(this.languageList.slice());
      return;
    }
    else {
      search = search.toLowerCase();
    }
    this.filteredFirstLanguage.next(
      this.languageList.filter(language => language.locale.toLowerCase().indexOf(search) > -1)
    );
  }
   filterSecondLanguage(){
    if (!this.languageList) {
      return;
    }
    let search = this.secondLanguageFilterCtrl.value;
    if (!search) {
      this.filteredSecondLanguage.next(this.languageList.slice());
      return;
    }
    else {
      search = search.toLowerCase();
    }
    this.filteredSecondLanguage.next(
      this.languageList.filter(language => language.locale.toLowerCase().indexOf(search) > -1)
    );
  }
   filterThirdLanguage(){
    if (!this.languageList) {
      return;
    }
    let search = this.thirdLanguageFilterCtrl.value;
    if (!search) {
      this.filteredThirdLanguage.next(this.languageList.slice());
      return;
    }
    else {
      search = search.toLowerCase();
    }
    this.filteredThirdLanguage.next(
      this.languageList.filter(language => language.locale.toLowerCase().indexOf(search) > -1)
    );
  }
  filterCountryOfBirth(){
    if (!this.countryListArr) {
      return;
    }
    let search = this.countryOfBirthFilterCtrl.value;
    if (!search) {
      this.filteredcountryOfBirth.next(this.countryListArr.slice());
      return;
    }
    else {
      search = search.toLowerCase();
    }
    this.filteredcountryOfBirth.next(
      this.countryListArr.filter(country => country.name.toLowerCase().indexOf(search) > -1)
    );
  }
  getStudentDetails() {
    return new Promise((resolve, rej) => {
      this.studentService.getStudentDetailsForGeneral.pipe(takeUntil(this.destroySubject$)).subscribe((res: StudentAddModel) => {        
        this.studentAddModel = res;
        this.studentAddModel.loginEmail = this.studentAddModel.studentMaster.studentPortalId;
        this.data = this.studentAddModel?.studentMaster;
        this.cloneStudentModel = JSON.stringify(this.studentAddModel);
        this.studentInternalId = this.data.studentInternalId;
        this.studentPortalId = this.data.studentPortalId;
        if (this.studentAddModel.studentMaster?.studentId) {
          resolve([]);
        }
      });
    });
  }


  initializeDropdownsInAddMode() {
    this.callLOVs();
    this.getAllCountry();
    this.GetAllLanguage();
  }

  callLOVs() {
    this.commonLOV.getLovByName('Salutation').pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      this.salutationList = res;
    });
    this.commonLOV.getLovByName('Suffix').pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      this.suffixList = res;
    });
    this.commonLOV.getLovByName('Gender').pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      this.genderList = res;
    });
    this.commonLOV.getLovByName('Race').pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      this.raceList = res;
    });
    this.commonLOV.getLovByName('Ethnicity').pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      this.ethnicityList = res;
    });
    this.commonLOV.getLovByName('Marital Status').pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      this.maritalStatusList = res;
    });
  }

  nationalityValueChange(){
    this.nationalityFilterCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe((res) => {
        this.filterNationalitis();
      });
  }
  countryOfBirthValueChange(){
    this.countryOfBirthFilterCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe((res) => {
        this.filterCountryOfBirth();
      });
  }
  firstLanguageValueChange(){
    this.firstLanguageFilterCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe((res) => {
        this.filterFirstLanguage();
      });
  }
   secondLanguageValueChange(){
    this.secondLanguageFilterCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe((res) => {
        this.filterSecondLanguage();
      });
  }
   thirdLanguageValueChange(){
    this.thirdLanguageFilterCtrl.valueChanges
      .pipe(takeUntil(this._onDestroy))
      .subscribe((res) => {
        this.filterThirdLanguage();
      });
  }
  ngAfterViewInit() {
    this.countryOfBirthValueChange();
    this.nationalityValueChange();
    this.firstLanguageValueChange();
    this.secondLanguageValueChange();
    this.thirdLanguageValueChange();
    this.studentInternalId = this.data?.studentInternalId;
    if (this.studentAddModel.studentMaster.studentPortalId == null) {
      this.studentPortalId = this.studentAddModel.loginEmail;
    }
    else {
      this.studentPortalId = this.data?.studentPortalId;
    }
    // For Checking Internal Id
    this.internalId.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term !== '') {
        if (this.studentInternalId === term) {
          this.internalId.setErrors(null);
        }
        else {
          this.isStudentInternalId = true;
          this.checkStudentInternalIdViewModel.studentInternalId = term;
          this.studentService.checkStudentInternalId(this.checkStudentInternalIdViewModel).subscribe(data => {
            if(data._failure){
              this.commonService.checkTokenValidOrNot(data._message);
            } else {
              if (data.isValidInternalId) {
                this.internalId.setErrors(null);
                this.isStudentInternalId = false;
              }
              else {
                this.internalId.markAsTouched();
                this.internalId.setErrors({ nomatch: true });
                this.isStudentInternalId = false;
              }
            }

          });
        }
      } else {
        this.internalId.markAsTouched();
        this.isStudentInternalId = false;
      }
    });

    this.loginEmail.valueChanges
      .pipe(debounceTime(600), distinctUntilChanged())
      .subscribe(term => {
        if (term) {
          if (this.studentPortalId === term) {
            this.loginEmail.setErrors(null);
          }
          else {
            this.isUser = true;
            this.checkUserEmailAddressViewModel.emailAddress = term;
            this.loginService.checkUserLoginEmail(this.checkUserEmailAddressViewModel).subscribe(data => {
              if(data._failure){
                this.commonService.checkTokenValidOrNot(data._message);
              } else {
                if (data.isValidEmailAddress) {
                  this.loginEmail.setErrors(null);
                  this.isUser = false;
                }
                else {
                  this.loginEmail.markAsTouched();
                  this.loginEmail.setErrors({ nomatch: true });
                  this.isUser = false;
                }
              }
            });
          }
        } else {
          // this.loginEmail.markAsTouched();
          this.isUser = false;
        }
      });
  }

  accessPortal() {
    if (this.data?.studentPortalId) {
      this.hideAccess = true;
      this.fieldDisabled = true;
      this.hidePasswordAccess = false;
    } else {
      this.hideAccess = false;
      this.fieldDisabled = false;
      this.hidePasswordAccess = false;
    }
  }

  omitSpecialChar(event) {
    let k = event.charCode;
    return ((k > 64 && k < 91) || (k > 96 && k < 123) || k === 8 || (k >= 48 && k <= 57));
  }

  isPortalAccess(event) {
    if (event.checked) {
      if (this.studentCreateMode === this.studentCreate.ADD) {
        this.hidePasswordAccess = true;
      }
      else {
        if (this.data.studentPortalId !== null) {
          this.hidePasswordAccess = false;
        }
        else {
          this.hidePasswordAccess = true;
        }
      }
      this.hideAccess = true;
      this.studentAddModel.portalAccess = true;
    }
    else {
      this.hideAccess = false;
      this.hidePasswordAccess = false;
      this.studentAddModel.portalAccess = false;
    }
  }

  getAllCountry() {
    if (!this.countryModel.isCountryAvailable) {
      this.countryModel.isCountryAvailable = true;
      this.commonService.GetAllCountry(this.countryModel).pipe(takeUntil(this.destroySubject$)).subscribe(data => {
        if (data) {
         if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
            this.countryListArr = [];
            if(!data.tableCountry){
              this.snackbar.open(data._message, '', {
                duration: 10000
              });
            }
          } else {
            // this.nationalityCtrl.setValue(data.tableCountry?.sort((a, b) => a.name < b.name ? -1 : 1));
            this.filteredNationality.next(data.tableCountry?.slice());
            // this.countryOfBirthCtrl.setValue(data.tableCountry?.sort((a, b) => a.name < b.name ? -1 : 1));
            this.filteredcountryOfBirth.next(data.tableCountry?.slice());
            this.countryListArr = data.tableCountry?.sort((a, b) => a.name < b.name ? -1 : 1);
            if (this.studentCreateMode === this.studentCreate.VIEW) {
              this.findCountryNationalityById();
            }
          }
        }
        else {
          this.countryListArr = [];
        }
      });
    }
  }

  findCountryNationalityById() {
    this.countryListArr.map((val) => {
      const countryInNumber = +this.data.countryOfBirth;
      const nationality = +this.data.nationality;
      if (val.id === countryInNumber) {
        this.nameOfMiscValuesForView.countryName = val.name;
      }
      if (val.id === nationality) {
        this.nameOfMiscValuesForView.nationality = val.name;
      }
    });
  }
  editGeneralInfo() {
    this.studentCreateMode = this.studentCreate.EDIT;
    this.callLOVs();
    this.studentService.changePageMode(this.studentCreateMode);
    this.saveAndNext = 'update';
  }

  cancelEdit() {
    if (JSON.stringify(this.studentAddModel) !== this.cloneStudentModel) {
      this.studentAddModel = JSON.parse(this.cloneStudentModel);
      this.studentDetailsForViewAndEdit = JSON.parse(this.cloneStudentModel);
      this.studentService.sendDetails(JSON.parse(this.cloneStudentModel));
    }
    this.accessPortal();
    this.findCountryNationalityById();
    this.findLanguagesById();
    this.studentCreateMode = this.studentCreate.VIEW;
    this.imageCropperService.cancelImage('student');
    this.studentService.changePageMode(this.studentCreateMode);
    this.data = this.studentAddModel.studentMaster;
  }

  GetAllLanguage() {
    if (!this.languages.isLanguageAvailable) {
      this.languages.isLanguageAvailable = true;
      
      this.languages._tenantName=this.defaultValuesService.getTenantName();
      this.loginService.getAllLanguage(this.languages).pipe(takeUntil(this.destroySubject$)).subscribe(
        (res) => {
          if (res) {
           if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
              this.languageList = [];
              if(!res.tableLanguage){
                this.snackbar.open(res._message, '', {
                  duration: 10000
                });
              }
            }else{
              this.filteredFirstLanguage.next(res.tableLanguage?.slice());
              this.filteredSecondLanguage.next(res.tableLanguage?.slice());
              this.filteredThirdLanguage.next(res.tableLanguage?.slice());
            this.languageList = res.tableLanguage?.sort((a, b) => a.locale < b.locale ? -1 : 1);
            if (this.studentCreateMode === this.studentCreate.VIEW) {
              this.findLanguagesById();
            }
          }
          }
          else {
            this.languageList = [];
          }
        }
      );
    }
  }

  findLanguagesById() {
    this.languageList.map((val) => {
      const firstLanguageId = + this.data.firstLanguageId;
      const secondLanguageId = + this.data.secondLanguageId;
      const thirdLanguageId = + this.data.thirdLanguageId;

      if (val.langId === firstLanguageId) {
        this.nameOfMiscValuesForView.firstLanguage = val.locale;
      }
      if (val.langId === secondLanguageId) {
        this.nameOfMiscValuesForView.secondLanguage = val.locale;
      }
      if (val.langId === thirdLanguageId) {
        this.nameOfMiscValuesForView.thirdLanguage = val.locale;
      }
    });
  }
  generate() {
    this.inputType = 'text';
    this.visible = true;
    this.currentForm.controls.passwordHash.setValue(this.commonFunction.autoGeneratePassword());
  }
  protected setInitialValue() {
    this.filteredNationality
    .pipe(take(1), takeUntil(this._onDestroy))
    .subscribe(() => {
      this.singleSelect.compareWith = (a, b) => a && b && a.name === b.name;
    });
    this.filteredcountryOfBirth
    .pipe(take(1), takeUntil(this._onDestroy))
    .subscribe(() => {
      this.singleSelect.compareWith = (a, b) => a && b && a.name === b.name;
    });
    this.filteredFirstLanguage
    .pipe(take(1), takeUntil(this._onDestroy))
    .subscribe(() => {
      this.singleSelect.compareWith = (a, b) => a && b && a.locale === b.locale;
    });
    this.filteredSecondLanguage
    .pipe(take(1), takeUntil(this._onDestroy))
    .subscribe(() => {
      this.singleSelect.compareWith = (a, b) => a && b && a.locale === b.locale;
    });
    this.filteredThirdLanguage
    .pipe(take(1), takeUntil(this._onDestroy))
    .subscribe(() => {
      this.singleSelect.compareWith = (a, b) => a && b && a.locale === b.locale;
    });
  }

  activateUser(event) {
    if (event === false) {
      this.activeDeactiveUserModel.userId = this.studentAddModel.studentMaster.studentId;
      this.activeDeactiveUserModel.isActive = true;
      this.activeDeactiveUserModel.module = 'student';
      this.activeDeactiveUserModel.loginEmail = this.studentAddModel.loginEmail;
      this.commonService.activeDeactiveUser(this.activeDeactiveUserModel).subscribe(res => {
        if (res) {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          } else {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
            // this.studentAddModel.studentMaster.isActive = true;
          }
        } else {
          this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      });
    }
  }

  submit() {
    this.currentForm.form.markAllAsTouched();
    if (this.currentForm.controls.passwordHash !== undefined) {
      this.studentAddModel.passwordHash = this.currentForm.controls.passwordHash.value;
    }
    let emailError;
    if(this.studentAddModel.portalAccess && this.loginEmail.errors) {
      emailError = true
    }
    if (this.currentForm.form.valid && !emailError) {
      if (this.studentAddModel.fieldsCategoryList?.length>0) {
        this.studentAddModel.selectedCategoryId = + this.studentAddModel.fieldsCategoryList[this.categoryId]?.categoryId;
        for (const studentCustomField of this.studentAddModel.fieldsCategoryList[this.categoryId]?.customFields) {
          if (studentCustomField.type === 'Multiple SelectBox' && this.studentService.getStudentMultiselectValue() !== undefined) {
            studentCustomField.customFieldsValue[0].customFieldValue = this.studentService.getStudentMultiselectValue().toString().replaceAll(',', '|');
          }
        }
      }
      if (this.studentCreateMode === this.studentCreate.EDIT) {
        this.updateStudent();
      } else {
        this.addStudent();
      }
    }
  }

  updateStudent() {
    if (this.internalId.invalid) {
      this.invalidScroll();
      return;
    }
    this.studentAddModel.studentMaster.dob = this.commonFunction.formatDateSaveWithoutTime(this.studentAddModel.studentMaster.dob);
    this.studentService.UpdateStudent(this.studentAddModel).pipe(takeUntil(this.destroySubject$)).subscribe(data => {
      if (data) {
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
        if(data.checkDuplicate === 1){
          this.confirmAddStudent();
        }
        else{
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
          this.cloneStudentModel = JSON.stringify(data);
          this.findCountryNationalityById();
          this.findLanguagesById();
          this.studentDetailsForViewAndEdit = data;
          this.studentService.setDataAfterSavingGeneralInfo(data);
          // this.dataAfterSavingGeneralInfo.emit(data);
          this.studentCreateMode = this.studentCreate.VIEW;
          this.studentService.changePageMode(this.studentCreateMode);
          this.studentAddModel.loginEmail = data.studentMaster.studentPortalId;
          this.accessPortal();
        }
      } else {
        this.snackbar.open(this.defaultValuesService.translateKey('studentUpdateFailed') + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  confirmAddStudent(){
    let mode= 'add';
    if(this.studentCreateMode===this.studentCreate.EDIT){
     mode= 'update';
    }
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: "400px",
      disableClose: true,
      data: {
          title: "Are you sure?",
          message: "You are about to "+ mode +" duplicate data"
        }
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if(dialogResult){
        this.studentAddModel.allowDuplicate = true;
        this.submit();
      }
   });
  }

  addStudent() {
    if (this.internalId.invalid) {
      this.invalidScroll();
      return;
    }
    this.studentAddModel.academicYear = this.defaultValuesService.getAcademicYear()?.toString();
    this.studentAddModel.studentMaster.dob = this.commonFunction.formatDateSaveWithoutTime(this.studentAddModel.studentMaster.dob);
    this.studentService.AddStudent(this.studentAddModel).pipe(takeUntil(this.destroySubject$)).subscribe(data => {
      if (data) {
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);

        if(data.checkDuplicate === 1){
          this.confirmAddStudent();
        }
        else{
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
        }
        } else {
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
         
          this.studentService.setStudentId(data.studentMaster.studentId);
          this.studentService.setStudentCloneImage(data.studentMaster.studentPhoto);
          this.studentService.setStudentDetails(data);
          this.studentService.setDataAfterSavingGeneralInfo(data);
          this.checkPermissionAndSwitchNextCategory();
          this.studentCreateMode = this.studentCreate.EDIT;
          this.studentService.changePageMode(this.studentCreateMode);
          this.countryOfBirthCtrl.setValue(data.studentMaster.countryOfBirth);
          this.nationalityCtrl.setValue(data.studentMaster.nationality);
          // this.dataAfterSavingGeneralInfo.emit(data);
          this.imageCropperService.enableUpload({module: this.moduleIdentifier.STUDENT, upload: true, mode: this.studentCreate.EDIT});
        }
      }
      else {
        this.snackbar.open(this.defaultValuesService.translateKey('studentSaveFailed') + this.defaultValuesService.getHttpError(), '', {
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

  checkPermissionAndSwitchNextCategory(){
    let permittedTabs = this.pageRolePermission.getPermittedSubCategories('/school/students')    
    this.studentService.setCategoryTitle(permittedTabs[1]?.title)
    this.router.navigateByUrl(permittedTabs[1]?.path)
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
    if (this.cloneStudentModel && JSON.stringify(this.studentAddModel) !== this.cloneStudentModel) {
      this.studentAddModel = JSON.parse(this.cloneStudentModel);
      this.studentDetailsForViewAndEdit = JSON.parse(this.cloneStudentModel);
      this.studentService.sendDetails(JSON.parse(this.cloneStudentModel));
    }
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
