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

import { AfterViewInit, Component, EventEmitter, Input, OnDestroy, OnInit, Output } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";
import icSearch from "@iconify/icons-ic/search";
import { MatDialog } from "@angular/material/dialog";
import { AddGradeCommentsComponent } from "./add-grade-comments/add-grade-comments.component";
import { GradeBookConfigurationService } from "src/app/services/gradebook-configuration.service";
import { AddGradebookGradeByAssignmentTypeModel, AddGradebookGradeModel, ViewGradebookGradeByAssignmentTypeModel, ViewGradebookGradeModel } from "src/app/models/gradebook-grades.model";
import { CommonService } from "src/app/services/common.service";
import { DefaultValuesService } from "src/app/common/default-values.service";
import { MatSnackBar } from "@angular/material/snack-bar";
import { FormControl } from "@angular/forms";
import { debounceTime, distinctUntilChanged, takeUntil } from "rxjs/operators";
import { ExcelService } from "src/app/services/excel.service";
import { DatePipe } from "@angular/common";
import { LoaderService } from "src/app/services/loader.service";
import { Subject } from "rxjs";
import { GradebookConfigurationAddViewModel } from '../../../../models/gradebook-configuration.model';

@Component({
  selector: "vex-gradebook-grades",
  templateUrl: "./gradebook-grades.component.html",
  styleUrls: ["./gradebook-grades.component.scss"],
  providers: [DatePipe]
})
export class GradebookGradesComponent implements OnInit, AfterViewInit, OnDestroy {
  icSearch = icSearch;
  currentComponent;
  classGrade = true;
  categoryDetails = false;
  @Input() currentTab:string;

  viewGradebookGradeModel: ViewGradebookGradeModel = new ViewGradebookGradeModel();
  gradeData: any;
  viewGradebookGradeByAssignmentTypeModel: ViewGradebookGradeByAssignmentTypeModel = new ViewGradebookGradeByAssignmentTypeModel();
  assignmentListByAssignmentType;
  addGradebookGradeModel: AddGradebookGradeModel =  new AddGradebookGradeModel();
  addGradebookGradeByAssignmentTypeModel: AddGradebookGradeByAssignmentTypeModel = new AddGradebookGradeByAssignmentTypeModel();
  searchCtrl: FormControl;
  searchCtrlForAssignmentType: FormControl;
  studentListForGenerateExcel: any[];
  loading: boolean;
  destroySubject$: Subject<void> = new Subject();
  assignmentTypeDetails: any;
  gradebookConfigurationAddViewModel: GradebookConfigurationAddViewModel = new GradebookConfigurationAddViewModel();
  selectedCourseSection;
  changeParcentageCalculationValue;
  markingPeriodId;
  isWeightedSection: boolean;
  @Output() isConfigUpdateFlag = new EventEmitter<boolean>()
  constructor(
    public translateService: TranslateService,
    private dialog: MatDialog,
    private gradeBookConfigurationService: GradeBookConfigurationService,
    private commonService: CommonService,
    public defaultValuesService: DefaultValuesService,
    private snackbar: MatSnackBar,
    private excelService: ExcelService,
    private datePipe: DatePipe,
    private loaderService: LoaderService,
  ) {
    // translateService.use("en");
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
    this.gradebookConfigurationAddViewModel.gradebookConfiguration.general = '';
    this.selectedCourseSection = this.defaultValuesService.getSelectedCourseSection();
    this.viewGradebookConfiguration()
    this.markingPeriodId = this.findMarkingPeriodTitleById(this.selectedCourseSection);
  }

  ngOnInit(): void {
    this.searchCtrl = new FormControl();
    this.searchCtrlForAssignmentType = new FormControl();
   
    this.currentTab = "gradebook";
    this.currentComponent = "gradebookGrade";
  }

  ngAfterViewInit() {
    this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term !== '') {
        this.getGradebookGrade(term);
      } else {
        this.getGradebookGrade();
      }
    });

    this.searchCtrlForAssignmentType.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term !== '') {
        this.getGradebookGradeByAssignmentType(term);
      } else {
        this.getGradebookGradeByAssignmentType();
      }
    });
    
  }


  changeComponent(step, data?) {
    this.currentComponent = step;
    this.gradeData = data;
  }

  showCategoryList(data) {
    this.classGrade = false;
    this.categoryDetails = true;
    this.assignmentTypeDetails = data;
    this.getGradebookGradeByAssignmentType();
  }


  getGradebookGradeByAssignmentType(searchValue?, includeInactive?) {
    this.viewGradebookGradeByAssignmentTypeModel.assignmentTpyeId = this.assignmentTypeDetails.assignmentTypeId;
    this.viewGradebookGradeByAssignmentTypeModel.courseSectionId = this.defaultValuesService.getSelectedCourseSection().courseSectionId;
    this.viewGradebookGradeByAssignmentTypeModel.SearchValue = searchValue ? searchValue : null;
    this.viewGradebookGradeByAssignmentTypeModel.includeInactive = includeInactive ? includeInactive.checked : false;
    // this.viewGradebookGradeByAssignmentTypeModel.courseSectionId = this.defaultValuesService.getSelectedCourseSection().courseSectionId;
    // return;
    this.gradeBookConfigurationService.viewGradebookGradeByAssignmentType(this.viewGradebookGradeByAssignmentTypeModel).subscribe((res: any)=>{
      if(res._failure){
        this.addGradebookGradeByAssignmentTypeModel = res;
        this.commonService.checkTokenValidOrNot(res._message);
      } else{
        this.addGradebookGradeByAssignmentTypeModel = res;
        this.assignmentListByAssignmentType = res
      }
    })
  }

  submitGradesBookByAssignmentType() {
    delete this.addGradebookGradeByAssignmentTypeModel.academicYear;
    this.addGradebookGradeByAssignmentTypeModel.markingPeriodId = this.markingPeriodId;
    this.gradeBookConfigurationService.addGradebookGradeByAssignmentType(this.addGradebookGradeByAssignmentTypeModel).subscribe((res)=>{
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
      } else{
        this.getGradebookGradeByAssignmentType();
        this.snackbar.open(res._message, '', {
          duration: 10000
        });
        // this.assignmentList = res
      }
    })
  }

  backTogradeList() {
    this.categoryDetails = false;
    this.classGrade = true;
  }
  
  addGradeComment(grade) {
    this.dialog.open(AddGradeCommentsComponent, {
      width: "500px",
      data : {comment: grade.comment}
    }).afterClosed().subscribe((res)=>{
      if(res) {
        grade.comment = res;
      }
    });
  }

  findMarkingPeriodTitleById(coursesectionDetails) {
    let markingPeriodId;
    if (coursesectionDetails.yrMarkingPeriodId) {
      markingPeriodId = '0_' + coursesectionDetails.yrMarkingPeriodId;
    } else if (coursesectionDetails.smstrMarkingPeriodId) {
      markingPeriodId = '1_' + coursesectionDetails.smstrMarkingPeriodId;
    } else if (coursesectionDetails.qtrMarkingPeriodId) {
      markingPeriodId = '2_' + coursesectionDetails.qtrMarkingPeriodId;
    } else if (coursesectionDetails.prgrsprdMarkingPeriodId) {
      markingPeriodId = '3_' + coursesectionDetails.prgrsprdMarkingPeriodId;
    } else {
      markingPeriodId = null;
    }
    return markingPeriodId;
  }

  getGradebookGrade(SearchValue?, includeInactive?) {
    this.viewGradebookGradeModel.courseSectionId = this.defaultValuesService.getSelectedCourseSection().courseSectionId;
    this.viewGradebookGradeModel.SearchValue = SearchValue ? SearchValue : null;
    this.viewGradebookGradeModel.includeInactive = includeInactive ? includeInactive.checked : false;

    // return;
    this.gradeBookConfigurationService.viewGradebookGrade(this.viewGradebookGradeModel).subscribe((res: any)=>{
      if(res._failure){
        this.addGradebookGradeModel = res;
        this.commonService.checkTokenValidOrNot(res._message);
      } else{
        res.assignmentsListViewModels?.map( data => {
          data.studentsListViewModels.map( value => {
              if( this.changeParcentageCalculationValue === 'up' ){
                value.runningAvg = Math.ceil(value.runningAvg)
                value.percentage = Math.ceil(value.percentage)
              } else if ( this.changeParcentageCalculationValue === 'down'){
                value.runningAvg = Math.floor(value.runningAvg)
                value.percentage = Math.floor(value.percentage)
              } else if ( this.changeParcentageCalculationValue === 'normal' ){
                value.runningAvg = Math.round(value.runningAvg)
                value.percentage = Math.round(value.percentage)
              }
          })
        })
        this.isConfigUpdateFlag.emit(res?.configUpdateFlag)
        this.addGradebookGradeModel = res;
        this.createDataSetForExcel();
        };

      })
  }
// this api is called for getting the gradebookConfiguration.scoreRounding value
  viewGradebookConfiguration() {
    this.gradebookConfigurationAddViewModel.gradebookConfiguration.courseId = this.selectedCourseSection.courseId;
    this.gradebookConfigurationAddViewModel.gradebookConfiguration.courseSectionId = +this.selectedCourseSection.courseSectionId;
    this.gradeBookConfigurationService.viewGradebookConfiguration(this.gradebookConfigurationAddViewModel).subscribe(
      (res: GradebookConfigurationAddViewModel) => {
        if(res._failure){
          this.commonService.checkTokenValidOrNot(res._message);
        }else{
          this.changeParcentageCalculationValue=res.gradebookConfiguration.scoreRounding;
          this.isWeightedSection = res?.gradebookConfiguration?.general?.includes('weightGrades') ? true : false;
          this.getGradebookGrade();
        }
      }
    );
  }

  submitGradesBook() {
    delete this.addGradebookGradeModel.academicYear;
    this.addGradebookGradeModel.markingPeriodId = this.markingPeriodId;
    this.gradeBookConfigurationService.addGradebookGrade(this.addGradebookGradeModel).subscribe((res)=>{
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
      } else{
        this.getGradebookGrade();
        this.snackbar.open(res._message, '', {
          duration: 10000
        });
        // this.assignmentList = res
      }
    })
  }

  createDataSetForExcel() {
    this.studentListForGenerateExcel = [];
    this.addGradebookGradeModel.assignmentsListViewModels?.map((item)=>{
      item.studentsListViewModels.map((subItem)=>{
      let middleName = subItem.middleName==null ? ' ' : ' ' + subItem.middleName + ' ';

      this.studentListForGenerateExcel.push(
          {
            [this.defaultValuesService.translateKey('name')]: subItem.firstGivenName + middleName + subItem.lastFamilyName,
            [this.defaultValuesService.translateKey('studentInternalId')]: subItem.studentInternalId,
            [this.defaultValuesService.translateKey('avgAndLetterGrade')]: `${subItem.runningAvg ? subItem.runningAvg + '%' : ''} ${subItem.runningAvgGrade ? '[ '+ subItem.runningAvgGrade + ' ]' : '' }`,
            [this.defaultValuesService.translateKey('assignmentType')]: item.title,
            [this.defaultValuesService.translateKey('assignments')]: item.assignmentTitle,
            [this.defaultValuesService.translateKey('weightage')]: item.weightage,
            [this.defaultValuesService.translateKey('dueDate')]: this.datePipe.transform(item.dueDate),
            [this.defaultValuesService.translateKey('marksGiven')]: subItem.allowedMarks ? subItem.allowedMarks : 0,
            [this.defaultValuesService.translateKey('totalMarks')]: subItem.points,            
          }
        )
      });
    });
  }

  generateGradebookExcel() {
    this.excelService.exportAsExcelFile(this.studentListForGenerateExcel,'Gradebook_grades_');
  }

  showHideUngraded(status, item) {
    if(item.assignmentsListViewModels) {
      item.assignmentsListViewModels?.map((item)=>{
        item.studentsListViewModels.map((subItem)=>{
          subItem.isShowUngraded = subItem.allowedMarks && status.checked ? true : false;
        });
      });
      }

  }

  ngOnDestroy(){
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
