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
  ChangeDetectionStrategy,
  ChangeDetectorRef,
  Component,
  OnDestroy,
  OnInit,
} from "@angular/core";
import { Subject } from "rxjs";
import { fadeInRight400ms } from "../../../../@vex/animations/fade-in-right.animation";
import { ImageCropperService } from "src/app/services/image-cropper.service";
import { MatSnackBar } from "@angular/material/snack-bar";
import { LayoutService } from "src/@vex/services/layout.service";
import { stagger60ms } from "../../../../@vex/animations/stagger.animation";
import { fadeInUp400ms } from "../../../../@vex/animations/fade-in-up.animation";
import icSchool from "@iconify/icons-ic/outline-school";
import icCalendar from "@iconify/icons-ic/outline-calendar-today";
import icAlarm from "@iconify/icons-ic/outline-alarm";
import icPoll from "@iconify/icons-ic/outline-poll";
import icAccessibility from "@iconify/icons-ic/outline-accessibility";
import icHowToReg from "@iconify/icons-ic/outline-how-to-reg";
import icBilling from "@iconify/icons-ic/outline-monetization-on";
import { StudentService } from "../../../services/student.service";
import { OtherStudentTabs, StudentAddModel } from "../../../models/student.model";
import { CustomFieldService } from "../../../services/custom-field.service";
import { FieldsCategoryListView, FieldsCategoryModel } from "../../../models/fields-category.model";
import { SchoolCreate } from "../../../enums/school-create.enum";
import icHospital from "@iconify/icons-ic/baseline-medical-services";
import { filter, map, takeUntil } from "rxjs/operators";
import { LoaderService } from "../../../services/loader.service";
import { ModuleIdentifier } from "../../../enums/module-identifier.enum";
import { RolePermissionListViewModel } from "../../../models/roll-based-access.model";
import { CryptoService } from "../../../services/Crypto.service";
import { TranslateService } from "@ngx-translate/core";
import { DefaultValuesService } from "../../../common/default-values.service";
import { ActivatedRoute, NavigationStart, Router } from "@angular/router";
import { CommonService } from "../../../services/common.service";
import { PageRolesPermission } from "../../../common/page-roles-permissions.service";

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: "vex-add-student",
  templateUrl: "./add-student.component.html",
  styleUrls: ["./add-student.component.scss"],
  animations: [fadeInRight400ms, stagger60ms, fadeInUp400ms],
})
export class AddStudentComponent implements OnInit, OnDestroy {
  studentCreate = SchoolCreate;
  studentCreateMode: SchoolCreate = SchoolCreate.ADD;
  fieldsCategoryListView = new FieldsCategoryListView();
  currentCategory = 3; // because 3 is the id of general info.
  indexOfCategory = 0;
  secondarySidebar = 0;
  icSchool = icSchool;
  icCalendar = icCalendar;
  icAlarm = icAlarm;
  icPoll = icPoll;
  icAccessibility = icAccessibility;
  icHowToReg = icHowToReg;
  icBilling = icBilling;
  icHospital = icHospital;
  studentId: number;
  studentTitle: string;
  pageStatus = "Add Student";
  module = "Student";
  responseImage: string;
  enableCropTool = true;
  studentAddModel: StudentAddModel = new StudentAddModel();
  fieldsCategory = [];
  criticalAlert = false;
  destroySubject$: Subject<void> = new Subject();
  loading: boolean;
  moduleIdentifier = ModuleIdentifier;
  categoryTitle: string;
  studentCriticalInfo: string;
  otherTabs: OtherStudentTabs = new OtherStudentTabs();
  currentRolePermission;

  constructor(
    private layoutService: LayoutService,
    private studentService: StudentService,
    private snackbar: MatSnackBar,
    private customFieldservice: CustomFieldService,
    private imageCropperService: ImageCropperService,
    private loaderService: LoaderService,
    private defaultValuesService: DefaultValuesService,
    private cdr: ChangeDetectorRef,
    private cryptoService: CryptoService,
    public translateService: TranslateService,
    private router: Router,
    private commonService:CommonService,
    private pageRolePermission: PageRolesPermission,
    private activatedRoute: ActivatedRoute,
  ) {
    translateService.use("en");

  //   this.router.events
  //  .pipe(filter(e => e instanceof NavigationStart))
  //  .subscribe((e: NavigationStart) => {
  //   const navigation  = this.router.getCurrentNavigation();
  //   this.orderId = navigation.extras.state ? navigation.extras.state.orderId : 0;
  //  });

  //   this.currentRolePermission = this.activatedRoute.paramMap
  //       .pipe(map(() => console.log(window.history.state.data)));

  this.currentRolePermission =this.router.getCurrentNavigation().extras.state;

    this.layoutService.collapseSidenav();
    this.imageCropperService
      .getCroppedEvent()
      .pipe(takeUntil(this.destroySubject$))
      .subscribe((res) => {
        this.studentService.setStudentImage(res[1]);
      });
    this.studentService.selectedCatgoryTitle
      .pipe(takeUntil(this.destroySubject$))
      .subscribe((res: string) => {
        if(res){
          this.categoryTitle=res;
          let index=0;
          if(this.fieldsCategory.length>0){
            this.fieldsCategory.map((item,i)=>{
              if(item.title===this.categoryTitle){
                this.currentCategory=item.categoryId;
                index=i;
              }
            });
          this.studentService.setCategoryId(index);
          this.checkCurrentCategoryAndRoute();
          }
          
        }
      });
    this.studentService.modeToUpdate
      .pipe(takeUntil(this.destroySubject$))
      .subscribe((res: any) => {
        // this.studentCreateMode = res;
        if (res === this.studentCreate.VIEW) {
          this.pageStatus = "View Student";
        } else {
          this.pageStatus = "Edit Student";
        }
      });
    this.loaderService.isLoading
      .pipe(takeUntil(this.destroySubject$))
      .subscribe((currentState) => {
        this.loading = currentState;
      });
    this.studentService.getStudentDetailsForGeneral
      .pipe(takeUntil(this.destroySubject$))
      .subscribe((res: StudentAddModel) => {
        this.studentAddModel = res;
    this.studentService.setStudentDetailsForViewAndEdit(this.studentAddModel);

      });

      this.studentService.criticalAlertStatus.pipe(takeUntil(this.destroySubject$)).subscribe((res)=>{
        this.checkCriticalAlertFromMedical(res);
      })
  }

  ngOnInit(): void {
    // this.checkCurrentCategoryAndRoute();
    this.commonService.setModuleName(this.module);
    this.studentService.dataAfterSavingGeneralInfoChanged.subscribe((res)=>{
      if(res){
        this.afterSavingGeneralInfo(res);
      }
    })

    this.studentCreateMode = this.studentCreate.ADD;
    this.studentService.setStudentCreateMode(this.studentCreateMode);
    this.studentId = this.studentService.getStudentId();
    if (this.studentId) {
      this.imageCropperService.enableUpload({
        module: this.moduleIdentifier.STUDENT,
        upload: true,
        mode: this.studentCreate.VIEW,
      });
      this.studentCreateMode = this.studentCreate.VIEW;
    this.studentService.setStudentCreateMode(this.studentCreateMode);

      this.getStudentDetailsUsingId();
      this.onViewMode();
    } else if (this.studentCreateMode === this.studentCreate.ADD) {
      this.imageCropperService.enableUpload({
        module: this.moduleIdentifier.STUDENT,
        upload: true,
        mode: this.studentCreate.ADD,
      });
      this.getAllFieldsCategory();
    }
    this.checkOtherTabs();
  }

  checkOtherTabs(){
    this.otherTabs.courseSchedule=this.pageRolePermission.checkPageRolePermission("/school/students/student-course-schedule", this.currentRolePermission)?.view;
    this.otherTabs.attendance=this.pageRolePermission.checkPageRolePermission("/school/students/student-attendance", this.currentRolePermission)?.view
    this.otherTabs.reportCard=this.pageRolePermission.checkPageRolePermission("/school/students/student-report-card", this.currentRolePermission)?.view
    this.otherTabs.transcript=this.pageRolePermission.checkPageRolePermission("/school/students/student-transcript", this.currentRolePermission)?.view
  }

  onViewMode() {
    this.studentService.setStudentImage(this.responseImage);
    this.pageStatus = "View Student";
  }

  checkCriticalAlertFromMedical(status) {
    if(status[0]){
      this.criticalAlert=true;
      this.studentCriticalInfo=status[0]?.alertType;
    }else{
      this.criticalAlert=false;
    }
  }

  changeCategory(field, index) {
    // if(this.studentCreateMode === this.studentCreate.ADD) return;

    this.categoryTitle=field.title;
    this.commonService.setModuleName(this.module);
    const studentDetails = this.studentService.getStudentDetails();
    this.studentService.setStudentFirstView(false)
    if (studentDetails) {
      this.studentService.setCategoryTitle(this.categoryTitle);
      this.studentCreateMode = this.studentCreate.EDIT;
      
      this.currentCategory = field.categoryId;
      this.indexOfCategory = index;

      this.studentAddModel = studentDetails;
    this.studentService.setStudentDetailsForViewAndEdit(this.studentAddModel);

    }

    if (this.studentCreateMode === this.studentCreate.VIEW) {
      this.studentService.setCategoryTitle(this.categoryTitle);
      this.currentCategory = field.categoryId;
      this.indexOfCategory = index;

      this.pageStatus = "View Student";
    }
    this.studentService.setCategoryId(this.indexOfCategory);
    this.studentService.setStudentCreateMode(this.studentCreateMode);
    this.secondarySidebar = 0; // Close secondary sidenav in mobile view
    this.checkCurrentCategoryAndRoute();
  }

  checkCurrentCategoryAndRoute() {
    if(this.currentCategory === 3) {
      this.router.navigate(['/school', 'students', 'student-generalinfo']);
    } else if(this.currentCategory === 4 ) {
      this.router.navigate(['/school', 'students', 'student-enrollmentinfo']);
    } else if(this.currentCategory === 5 ) {
        this.router.navigate(['/school', 'students', 'student-address-contact']);
    } else if(this.currentCategory === 6 ) {
      this.router.navigate(['/school', 'students', 'student-familyinfo']);
    } else if(this.currentCategory === 7 ) {
      this.router.navigate(['/school', 'students', 'student-medicalinfo']);
    }
     else if(this.currentCategory === 8 ) {
      this.router.navigate(['/school', 'students', 'student-comments']);
    } else if(this.currentCategory === 9 ) {
      this.router.navigate(['/school', 'students', 'student-documents']);
    } else if(this.currentCategory === 100 ) {
      this.router.navigate(['/school', 'students', 'student-course-schedule']);
    }  else if(this.currentCategory === 101 ) {
      this.router.navigate(['/school', 'students', 'student-attendance']);
    }  else if(this.currentCategory === 102 ) {
      this.router.navigate(['/school', 'students', 'student-transcript']);
    }  else if(this.currentCategory === 103 ) {
      this.router.navigate(['/school', 'students', 'student-report-card']);
    }
    else if(this.currentCategory > 9 ) {
      this.router.navigate(['/school', 'students', 'custom', this.categoryTitle.trim().toLowerCase().split(' ').join('-')]);
    }

  }

  getAllFieldsCategory() {
    this.fieldsCategoryListView.module = "Student";
    this.customFieldservice
      .getAllFieldsCategory(this.fieldsCategoryListView)
      .subscribe((res) => {
        if (res) {
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            if (!res.fieldsCategoryList) {
              this.snackbar.open(res._message, "", {
                duration: 10000,
              });
            }
          } else {
            this.fieldsCategory = this.checkViewPermission(
              res.fieldsCategoryList
            );
            // this.studentAddModel.fieldsCategoryList = this.checkViewPermission(
            //   res.fieldsCategoryList
            // );
    // this.studentService.setStudentDetailsForViewAndEdit(this.studentAddModel);

            this.studentService.sendDetails(this.studentAddModel);
          }
        } else {
          this.snackbar.open(
            this.defaultValuesService.translateKey("categoryListFailed") +
            sessionStorage.getItem("httpError"),
            "",
            {
              duration: 10000,
            }
          );
        }
      });
  }
  checkViewPermission(category) {
    let permittedTabs = this.pageRolePermission.getPermittedSubCategories('/school/students', this.currentRolePermission);
    let filteredCategory: FieldsCategoryModel[] = [];
    for(const item of category){
      for (const permission of permittedTabs) {
        if (
          item.title.toLowerCase() ===
          permission.title.toLowerCase()
        ) {
            filteredCategory.push(item)
        }
      }
    }
    this.currentCategory = filteredCategory[0]?.categoryId;
    return filteredCategory;
  }

  changeTempCategory(step: number = 3) {
    if(this.pageStatus === "View Student" || this.pageStatus==="Edit Student") {
      this.currentCategory = step;
      this.secondarySidebar = 0; // Close secondary sidenav in mobile view
      this.checkCurrentCategoryAndRoute();
    };
  }

  getStudentDetailsUsingId() {
    this.studentAddModel.studentMaster.studentId = this.studentId;
    this.studentService.viewStudent(this.studentAddModel).subscribe((data) => {
      if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
      }
      this.studentAddModel = data;
      this.responseImage = this.studentAddModel.studentMaster.studentPhoto;
      this.studentAddModel.fieldsCategoryList = this.checkViewPermission(data.fieldsCategoryList);
      this.fieldsCategory = this.studentAddModel.fieldsCategoryList

      this.studentAddModel.studentMaster.studentPhoto = null;
      this.checkCriticalAlertFromMedical(data.studentMaster.studentMedicalAlert)
      this.studentService.sendDetails(this.studentAddModel);
      this.studentTitle =
        this.studentAddModel.studentMaster.firstGivenName +
        " " +
        this.studentAddModel.studentMaster.lastFamilyName;
      let studentName = this.studentAddModel.studentMaster.firstGivenName + '|' + this.studentAddModel.studentMaster.middleName + '|' + this.studentAddModel.studentMaster.lastFamilyName;
      this.studentService.setStudentName(studentName)
      this.studentService.setStudentImage(this.responseImage);
      this.studentService.setStudentCloneImage(this.responseImage);
    });
    this.studentService.setStudentDetailsForViewAndEdit(this.studentAddModel);

  }

  toggleSecondarySidebar() {
    if(this.secondarySidebar === 0){
      this.secondarySidebar = 1;
    } else {
      this.secondarySidebar = 0;
    }
  }

  ngAfterViewChecked() {
    this.cdr.detectChanges();
  }

  afterSavingGeneralInfo(data) {
    if(data) {
      this.studentTitle =
      data.studentMaster.firstGivenName +
      " " +
      data.studentMaster.lastFamilyName;

    let studentName = data.studentMaster.firstGivenName + '|' + null + '|' + data.studentMaster.lastFamilyName;
    this.studentService.setStudentName(studentName)
    }
  }

  navigateToMedicalInfo(){
    if(this.criticalAlert){
      this.currentCategory = 7;
      this.indexOfCategory = 6; // This needs to be dynamic.We have to fix it.
      this.studentService.setCategoryId(this.currentCategory);
      this.router.navigate(['/school', 'students', 'student-medicalinfo']);
    }
  }

  ngOnDestroy() {
    this.defaultValuesService.setSchoolID(undefined);
    this.studentService.setStudentDetails(undefined);
    this.studentService.setStudentImage(null);
    this.studentService.setStudentId(null);
    this.studentService.setStudentFirstView(true);
    this.studentService.sendDetails(undefined);
    this.studentService.setCategoryTitle(null);
    this.studentService.setStudentCloneImage(null);
    this.studentService.setDataAfterSavingGeneralInfo(null)
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
