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

import { ChangeDetectionStrategy, ChangeDetectorRef, Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Subject, Subscription } from 'rxjs';
import { fadeInRight400ms } from '../../../../@vex/animations/fade-in-right.animation';
import { MatSnackBar } from '@angular/material/snack-bar';
import { stagger60ms } from '../../../../@vex/animations/stagger.animation';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import icGeneralInfo from '@iconify/icons-ic/outline-account-circle';
import icSchoolInfo from '@iconify/icons-ic/outline-corporate-fare';
import icSchool from "@iconify/icons-ic/outline-school";
import icCalendar from "@iconify/icons-ic/outline-calendar-today";
import icLoginInfo from '@iconify/icons-ic/outline-lock-open';
import icAddressInfo from '@iconify/icons-ic/outline-location-on';
import icCertificationInfo from '@iconify/icons-ic/outline-military-tech';
import icSchedule from '@iconify/icons-ic/outline-schedule';
import icCustomCategory from '@iconify/icons-ic/outline-article';
import { ImageCropperService } from '../../../services/image-cropper.service';
import { TranslateService } from '@ngx-translate/core';
import { SchoolCreate } from '../../../../../src/app/enums/school-create.enum';
import { StaffAddModel } from '../../../models/staff.model';
import { StaffService } from '../../../services/staff.service';
import { FieldsCategoryListView, FieldsCategoryModel } from '../../../models/fields-category.model';
import { CustomFieldService } from '../../../services/custom-field.service';
import { filter, takeUntil } from 'rxjs/operators';
import { LoaderService } from '../../../services/loader.service';
import { ModuleIdentifier } from '../../../enums/module-identifier.enum';
import { RolePermissionListViewModel } from '../../../models/roll-based-access.model';
import { CryptoService } from '../../../services/Crypto.service';
import { NavigationEnd, Router, RouterEvent } from '@angular/router';
import { CommonService } from '../../../services/common.service';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { Module } from 'src/app/enums/module.enum';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'vex-add-staff',
  templateUrl: './add-staff.component.html',
  styleUrls: ['./add-staff.component.scss'],
  animations: [
    fadeInRight400ms,
    stagger60ms,
    fadeInUp400ms
  ]
})
export class AddStaffComponent implements OnInit, OnDestroy {
  destroySubject$: Subject<void> = new Subject();
  staffCreate = SchoolCreate;
  @Input() staffCreateMode: SchoolCreate =  SchoolCreate.ADD;
  staffAddModel: StaffAddModel = new StaffAddModel();
  staffId: number;
  fieldsCategory = [];
  fieldsCategoryListView = new FieldsCategoryListView();
  permissionListViewModel:RolePermissionListViewModel = new RolePermissionListViewModel();
  currentCategory: number = 12; // because 12 is the id of general info.
  indexOfCategory: number = 0;
  staffTitle = "addStaffInformation";
  pageStatus = "addStaff";
  module = 'Staff';
  secondarySidebar = 0;
  responseImage: string;
  enableCropTool = true;
  icGeneralInfo = icGeneralInfo;
  icSchoolInfo = icSchoolInfo;
  icSchool = icSchool;
  icCalendar = icCalendar;
  icLoginInfo = icLoginInfo;
  icAddressInfo = icAddressInfo;
  icCertificationInfo = icCertificationInfo;
  icSchedule = icSchedule;
  icCustomCategory = icCustomCategory;
  loading: boolean;
  moduleIdentifier=ModuleIdentifier;
  profile:string;
  categoryTitle: string;
  categoryPath: string;
  currentRolePermission;

  constructor(public translateService: TranslateService,
    private staffService: StaffService,
    private customFieldService: CustomFieldService,
    private snackbar: MatSnackBar,
    private loaderService:LoaderService,
    private commonService: CommonService,
    private cdr: ChangeDetectorRef,
    private cryptoService: CryptoService,
    private imageCropperService:ImageCropperService,
    private pageRolePermission: PageRolesPermission,
    private router: Router,
    private defaultValuesService: DefaultValuesService
    ) {
      if ((this.defaultValuesService.getUserMembershipType() === "Super Administrator" || this.defaultValuesService.getUserMembershipType() === "School Administrator" ||
        this.defaultValuesService.getUserMembershipType() === "Admin Assistant") && this.router.url !== "/school/staff/staff-generalinfo") {
        this.defaultValuesService.checkAcademicYear() && !this.staffService.getStaffId() ? this.staffService.redirectToGeneralInfo() :
          !this.defaultValuesService.checkAcademicYear() && !this.staffService.getStaffId() ? this.staffService.redirectToStaffList() : ''
      } else if (this.defaultValuesService.getUserMembershipType() === "Teacher" || this.defaultValuesService.getUserMembershipType() === "Homeroom Teacher") {
        !this.staffService.getStaffId() ? this.router.navigate(['school/teacher/dashboards']) : ''
      }

    // this.imageCropperService.getCroppedEvent().pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
    //   this.staffService.setStaffImage(res[1]);
    // });
    this.staffService.selectedCategoryTitle.pipe(takeUntil(this.destroySubject$)).subscribe((res:string) => {
      if(res){
        this.categoryTitle=res;        
        let index=0;
        if(this.fieldsCategory.length>0){
          this.fieldsCategory.map((item,i)=>{
            if(item.title===this.categoryTitle){
              // this.currentCategory=item.categoryId;
              // this.categoryPath=item.path;
              index=i;
              this.changeCategory(item, index, true)
            }
          });
        this.staffService.setCategoryId(index);
        this.checkCurrentCategoryAndRoute();
        }
      }
    });
    this.staffService.modeToUpdate.pipe(takeUntil(this.destroySubject$)).subscribe((res:any)=>{
      if(res==this.staffCreate.VIEW){
        this.pageStatus="viewStaff";
      }else{
        this.pageStatus="editStaff";
      }
    });
    this.staffService.getStaffDetailsForGeneral.pipe(takeUntil(this.destroySubject$)).subscribe((res: StaffAddModel) => {
      this.staffAddModel=res;
      this.staffService.setStaffDetailsForViewAndEdit(this.staffAddModel);
    })
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((currentState) => {
      this.loading = currentState;
    });
    this.checkInitialState();

  }

  ngOnInit(): void {
    this.commonService.setModuleName(this.module);
    this.staffService.dataAfterSavedGeneralInfo.subscribe((res)=>{
      if(res){
        this.afterSavingGeneralInfo(res);
      }
    });

    this.staffService.checkedUpdatedProfileName.subscribe((res)=>{
      this.profileFromSchoolInfo(res);
    });


    this.router.onSameUrlNavigation = 'reload';

    this.router.events.pipe(takeUntil(this.destroySubject$)).pipe(
      filter((event: RouterEvent) => event instanceof NavigationEnd)
    ).subscribe((res) => {
     this.checkInitialState();
    });



  }

  checkInitialState() {
  this.currentRolePermission = this.router.getCurrentNavigation().extras.state ? this.router.getCurrentNavigation().extras.state.permissions : undefined;
  this.staffCreateMode = this.router.getCurrentNavigation().extras.state ? this.router.getCurrentNavigation().extras.state.type : this.staffCreateMode;
  this.staffService.setStaffCreateMode(this.staffCreateMode);
  
    this.staffId = this.staffService.getStaffId();

    if (this.staffCreateMode === SchoolCreate.VIEW || this.staffCreateMode === SchoolCreate.EDIT) {
     this.imageCropperService.enableUpload({module:this.moduleIdentifier.STAFF,upload:true,mode:this.staffCreate.VIEW});
     if(!this.staffAddModel.staffMaster.staffId) {
      this.getStaffDetailsUsingId();
    } else {
    if(this.staffAddModel.staffMaster.staffId !== this.staffService.getStaffId()){
      this.getStaffDetailsUsingId();
    }
  }
      this.onViewMode();
    } else if (this.staffCreateMode == SchoolCreate.ADD) {
      this.staffTitle = "addStaffInformation";
      this.staffAddModel = new StaffAddModel();
      this.responseImage = undefined;
      this.staffService.setStaffDetailsForViewAndEdit(this.staffAddModel);
      this.getAllFieldsCategory();
     this.imageCropperService.enableUpload({module:this.moduleIdentifier.STAFF,upload:true,mode:this.staffCreate.ADD});
     
    }
  }

  ngAfterViewChecked(){
    this.cdr.detectChanges();
 }
  onViewMode() {
    //this.staffService.setStaffImage(this.responseImage);
    this.pageStatus = "viewStaff"
  }

  afterSavingGeneralInfo(data){
    if(data?.staffMaster?.salutation!=null){
      this.staffTitle = data.staffMaster.salutation+" "+ data.staffMaster.firstGivenName + " " + data.staffMaster.lastFamilyName;
    }else{
      this.staffTitle = data?.staffMaster.firstGivenName + " " + data?.staffMaster.lastFamilyName;
    }
    
  }

  profileFromSchoolInfo(data){
    this.profile=data;
  }


  changeCategory(field, index, triggerEvent?) {

    this.categoryTitle = field.title;
    this.commonService.setModuleName(this.module);
    this.staffService.setStaffFirstView(false);

    let staffDetails = this.staffService.getStaffDetails();
    if (staffDetails) {
      if(!triggerEvent)
    this.staffService.setCategoryTitle(this.categoryTitle);
      this.staffCreateMode = this.staffCreate.EDIT;
      this.currentCategory = field.categoryId;
      this.categoryPath = field.path;

      this.indexOfCategory = index;
      this.staffAddModel = staffDetails;
      this.staffService.setStaffDetailsForViewAndEdit(this.staffAddModel);
    }

    if (this.staffCreateMode == this.staffCreate.VIEW) {
      this.categoryPath = field.path;
      if(!triggerEvent)
      this.staffService.setCategoryTitle(this.categoryTitle);
      this.currentCategory = field.categoryId;
      this.indexOfCategory = index;
      this.pageStatus = "viewStaff"
    }
    this.staffService.setStaffCreateMode(this.staffCreateMode);
    this.staffService.setCategoryId(this.indexOfCategory);
    this.secondarySidebar = 0; // Close secondary sidenav in mobile view
    if(this.pageStatus === "viewStaff" || this.pageStatus==="editStaff") { // only cange category in view and edit mode 
    this.checkCurrentCategoryAndRoute();
    }
  }

  checkCurrentCategoryAndRoute() {    
    if(this.categoryPath === '/school/staff/staff-generalinfo') {
      this.router.navigate(['/school', 'staff', 'staff-generalinfo']);
    } else if(this.categoryPath === '/school/staff/staff-schoolinfo') {
      this.router.navigate(['/school', 'staff', 'staff-schoolinfo']);
    } else if(this.categoryPath === '/school/staff/staff-addressinfo' ) {
      this.router.navigate(['/school', 'staff', 'staff-addressinfo']);
    } else if(this.categoryPath === '/school/staff/staff-certificationinfo' ) {
        this.router.navigate(['/school', 'staff', 'staff-certificationinfo']);
    } else if(this.categoryPath === '/school/staff/staff-course-schedule') {
      this.router.navigate(['/school', 'staff', 'staff-course-schedule']);
    } else{
      this.router.navigate(['/school', 'staff', 'custom', this.categoryTitle.trim().toLowerCase().split(' ').join('-')]);
    }
  }

  changeTempCategory(step) {
    if(this.pageStatus === "viewStaff" || this.pageStatus==="editStaff") {
      this.currentCategory = null;
      this.secondarySidebar = 0; // Close secondary sidenav in mobile view
      this.categoryPath = step;
      this.checkCurrentCategoryAndRoute();
    }
  }

  toggleSecondarySidebar() {
    if(this.secondarySidebar === 0){
      this.secondarySidebar = 1;
    } else {
      this.secondarySidebar = 0;
    }
  }

  showPage(pageId) {
    this.defaultValuesService.setPageId(pageId);
    //this.disableSection();
  }

  getStaffDetailsUsingId() {
    this.staffAddModel.staffMaster.staffId = this.staffId;
    this.staffService.viewStaff(this.staffAddModel).subscribe((data: any) => {
      if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
      } else {
        this.staffAddModel = data;
        this.staffAddModel.fieldsCategoryList = this.checkViewPermission(data.fieldsCategoryList);
        this.fieldsCategory = this.staffAddModel.fieldsCategoryList;
        this.fieldsCategory.map((item)=>{
          if(item.title===this.categoryTitle){
            this.currentCategory=item.categoryId;
          }
        });
        this.profileFromSchoolInfo(data.staffMaster.profile)
        this.responseImage = this.staffAddModel.staffMaster.staffPhoto;
        this.staffService.setStaffCloneImage(this.staffAddModel.staffMaster.staffPhoto);
        this.staffAddModel.staffMaster.staffPhoto=null;
        this.staffService.sendDetails(this.staffAddModel);
        if(this.staffAddModel.staffMaster.salutation!=null){
          this.staffTitle =this.staffAddModel.staffMaster.salutation+" "+ this.staffAddModel.staffMaster.firstGivenName + " " + this.staffAddModel.staffMaster.lastFamilyName;
        }else{
          this.staffTitle =this.staffAddModel.staffMaster.firstGivenName + " " + this.staffAddModel.staffMaster.lastFamilyName;
        }
        this.staffService.setStaffImage(this.responseImage);
      }    
      this.staffService.setStaffDetailsForViewAndEdit(this.staffAddModel);
    });
   

  }




  getAllFieldsCategory() {
    this.fieldsCategoryListView.module = Module.STAFF;
    this.customFieldService.getAllFieldsCategory(this.fieldsCategoryListView).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        this.snackbar.open('Category list failed. ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
      else {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          if (!res.fieldsCategoryList) {
            this.snackbar.open( res._message, '', {
              duration: 10000
            });
          }
        }
        else {
          this.staffAddModel.fieldsCategoryList= this.checkViewPermission(res.fieldsCategoryList);

          this.fieldsCategory = this.staffAddModel.fieldsCategoryList;
          this.staffService.sendDetails(this.staffAddModel);
          this.staffService.setStaffDetailsForViewAndEdit(this.staffAddModel);
        }
      }
    }
    );
  }

  checkViewPermission(category){
    let permittedTabs =this.pageRolePermission.getPermittedSubCategories('/school/staff')
    let filteredCategory: FieldsCategoryModel[] = [];
    for(const item of category){
      for (const permission of permittedTabs) {
        if (
          item.title.toLowerCase() ===
          permission.title.toLowerCase()
        ) {
            item.path= permission.path;
            filteredCategory.push(item)
        }
      }
    }

    this.currentCategory = filteredCategory[0]?.categoryId;
    return filteredCategory;
  }

  ngOnDestroy() {
    this.defaultValuesService.setSchoolID(undefined);
    this.staffService.setStaffDetails(undefined);
    this.staffService.setStaffImage(null);
    this.staffService.setStaffFirstView(true);
    this.staffService.setStaffId(null);
    this.staffService.setStaffCloneImage(null);
    this.staffService.setCategoryTitle(null);
    this.staffService.setCategoryId(null);
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
