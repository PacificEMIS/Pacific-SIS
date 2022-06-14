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

import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { Component, ElementRef, Input, OnInit, ViewChild } from '@angular/core';
import { MatAutocomplete, MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatChipInputEvent } from '@angular/material/chips';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Observable } from 'rxjs';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { CourseStandardForCourseViewModel } from 'src/app/models/course-manager.model';
import { GradeScaleListView } from 'src/app/models/grades.model';
import { GetMarkingPeriodByCourseSectionModel, GetMarkingPeriodTitleListModel } from 'src/app/models/marking-period.model';
import { GetAllCourseCommentCategoryModel } from 'src/app/models/report-card.model';
import { AddUpdateStudentFinalGradeModel, GetGradebookGradeinFinalGradeModel, StudentFinalGrade } from 'src/app/models/student-final-grade.model';
import { ScheduleStudentForView, ScheduleStudentListViewModel } from 'src/app/models/student-schedule.model';
import { AllScheduledCourseSectionForStaffModel } from 'src/app/models/teacher-schedule.model';
import { CommonService } from 'src/app/services/common.service';
import { CourseManagerService } from 'src/app/services/course-manager.service';
import { FinalGradeService } from 'src/app/services/final-grade.service';
import { GradesService } from 'src/app/services/grades.service';
import { LoaderService } from 'src/app/services/loader.service';
import { MarkingPeriodService } from 'src/app/services/marking-period.service';
import { ReportCardService } from 'src/app/services/report-card.service';
import { StudentScheduleService } from 'src/app/services/student-schedule.service';
import { TeacherScheduleService } from 'src/app/services/teacher-schedule.service';
import { GradebookConfigurationAddViewModel } from 'src/app/models/gradebook-configuration.model';
import { GradeBookConfigurationService } from 'src/app/services/gradebook-configuration.service';

export interface Comment {
  name: string;
}

@Component({
  selector: 'vex-input-final-grades',
  templateUrl: './input-final-grades.component.html',
  styleUrls: ['./input-final-grades.component.scss']
})
export class InputFinalGradesComponent implements OnInit {
  @Input() currentTab: string;
  markingPeriodId: number;
  viewDetailsModal = 0;
  visible = true;
  selectable = true;
  removable = true;
  filteredFruits: Observable<string[]>;
  fruits: string[] = ['Lemon'];
  allFruits: string[] = ['Apple', 'Lemon', 'Lime', 'Orange', 'Strawberry'];
  @ViewChild('fruitInput') fruitInput: ElementRef<HTMLInputElement>;
  showMessage = "pleaseSelectCourseSectionAndMarkingPeriodForInputGrade";
  pageStatus = "Grade Details";
  showComment: boolean = false;
  staffDetails;
  courseSectionData;
  commentDetails = 0;
  selectedStudent: number = 0;
  reportCardComments = [];
  courseList = [];
  courseStandardList = [];
  gradeScaleStandardList = [];
  gradeScaleList = [];
  loading: boolean = false;
  reportCardGrade = [
    { id: 1, value: 'A' },
    { id: 2, value: 'B' },
    { id: 3, value: 'C' },
    { id: 4, value: 'D' },
    { id: 5, value: 'F' }
  ];
  totalCount: number = 0;
  @ViewChild('auto') matAutocomplete: MatAutocomplete;
  courseSectionId: number;
  courseSectionDetails = [];
  markingPeriodList = [];
  studentFinalGradeComments;
  gradeScaleListView: GradeScaleListView = new GradeScaleListView();
  courseStandardForCourseViewModel: CourseStandardForCourseViewModel = new CourseStandardForCourseViewModel();
  reportCardCategoryWithComments: GetAllCourseCommentCategoryModel = new GetAllCourseCommentCategoryModel();
  getAllReportCardCommentsWithCategoryModel: GetAllCourseCommentCategoryModel = new GetAllCourseCommentCategoryModel();
  allScheduledCourseSectionBasedOnTeacher: AllScheduledCourseSectionForStaffModel = new AllScheduledCourseSectionForStaffModel();
  addUpdateStudentFinalGradeModel: AddUpdateStudentFinalGradeModel = new AddUpdateStudentFinalGradeModel();
  scheduleStudentListViewModel: ScheduleStudentListViewModel = new ScheduleStudentListViewModel();
  getMarkingPeriodTitleListModel: GetMarkingPeriodTitleListModel = new GetMarkingPeriodTitleListModel();
  getMarkingPeriodByCourseSectionModel: GetMarkingPeriodByCourseSectionModel = new GetMarkingPeriodByCourseSectionModel();
  isPercent: boolean = false;
  studentMasterList = [];
  addOnBlur = true;
  readonly separatorKeysCodes: number[] = [ENTER, COMMA];
  courseSection: any;
  getGradebookGradeinFinalGradeModel: GetGradebookGradeinFinalGradeModel = new GetGradebookGradeinFinalGradeModel();  
  selectedMarkingPeriod: any;
  creditHours;
  cloneAddUpdateStudentFinalGradeModel;
  gradebookConfigurationAddViewModel: GradebookConfigurationAddViewModel = new GradebookConfigurationAddViewModel();

  constructor(
    public translateService: TranslateService,
    private dialog: MatDialog,
    private finalGradeService: FinalGradeService,
    private teacherReassignmentService: TeacherScheduleService,
    private snackbar: MatSnackBar,
    private studentScheduleService: StudentScheduleService,
    private markingPeriodService: MarkingPeriodService,
    private router: Router,
    private gradesService: GradesService,
    private reportCardService: ReportCardService,
    public defaultValuesService: DefaultValuesService,
    private courseManager: CourseManagerService,
    private gradeBookConfigurationService: GradeBookConfigurationService,
    private loaderService: LoaderService,
    private commonService: CommonService,
  ) {
    this.courseSection = this.defaultValuesService.getSelectedCourseSection();
  }

  ngOnInit(): void {
    if(this.courseSection?.gradeScaleType !=='Ungraded'){
      this.getAllMarkingPeriodList();
    }
    else{
      this.courseSectionData.markingPeriodId='';
      this.currentTab="errorMessage";
    }
    
  }


  getAllMarkingPeriodList() {
    this.getMarkingPeriodByCourseSectionModel.courseSectionId =  this.courseSection.courseSectionId;

    this.markingPeriodService.getMarkingPeriodsByCourseSection(this.getMarkingPeriodByCourseSectionModel).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
        this.getMarkingPeriodTitleListModel.getMarkingPeriodView = [];
        if (!this.getMarkingPeriodTitleListModel?.getMarkingPeriodView) {
          this.snackbar.open(data._message, '', {
            duration: 1000
          });
        }
      } else {
        this.getMarkingPeriodTitleListModel.getMarkingPeriodView = data.getMarkingPeriodView;
        this.courseSectionData = this.findMarkingPeriodTitleById(this.courseSection);
        this.markingPeriodList = data.getMarkingPeriodView;
        // this.markingPeriodList = this.getMarkingPeriodTitleListModel.getMarkingPeriodView.filter(x => x.value === this.courseSectionData.markingPeriodId);
        
      }
    });
  }

  changeMarkingPeriod(event) {
    if(event.value) {
    // this.addUpdateStudentFinalGradeModel.markingPeriodId = event.value;
    const markingPeriodDetails = this.markingPeriodList.find(x=> x.text === event.value);
    
    if(markingPeriodDetails.value === 'Custom') {
      this.addUpdateStudentFinalGradeModel.markingPeriodId = null;
      this.addUpdateStudentFinalGradeModel.isCustomMarkingPeriod = true;
    } else {
    this.addUpdateStudentFinalGradeModel.markingPeriodId = markingPeriodDetails.value;
    }
    this.addUpdateStudentFinalGradeModel.isExamGrade = markingPeriodDetails.doesExam;
    this.addUpdateStudentFinalGradeModel.schoolId = this.defaultValuesService.getSchoolID();
    this.addUpdateStudentFinalGradeModel.tenantId = this.defaultValuesService.getTenantID();
    this.addUpdateStudentFinalGradeModel.courseId = this.courseSectionData.courseId;
    this.addUpdateStudentFinalGradeModel.courseSectionId = this.courseSectionData.courseSectionId;
    this.addUpdateStudentFinalGradeModel.calendarId = this.courseSectionData.calendarId;
    //this.addUpdateStudentFinalGradeModel.studentFinalGradeList= [];
    this.finalGradeService.getAllStudentFinalGradeList(this.addUpdateStudentFinalGradeModel).subscribe((res) => {
      if (res) {
        if (res._failure) {
          
          this.addUpdateStudentFinalGradeModel.courseId = this.courseSectionData.courseId;
          this.addUpdateStudentFinalGradeModel.calendarId = this.courseSectionData.calendarId;
          this.getAllReportCardCommentsWithCategory(this.addUpdateStudentFinalGradeModel.courseId);
          this.getAllCourseStandard(this.addUpdateStudentFinalGradeModel.courseId);
          if (this.courseSectionData.gradeScaleType !== 'Numeric') {
            this.addUpdateStudentFinalGradeModel.isPercent = false;
            this.getAllGradeScaleList(this.courseSectionData.standardGradeScaleId).then(() => {
              this.viewGradebookConfiguration().then(()=>{
                this.searchScheduledStudentForGroupDrop(this.courseSectionData.courseSectionId);
              })
            });
          }
          else {
            this.searchScheduledStudentForGroupDrop(this.courseSectionData.courseSectionId);
          }

        }
        else {
          this.getAllReportCardCommentsWithCategory(this.addUpdateStudentFinalGradeModel.courseId);
          this.getAllCourseStandard(this.addUpdateStudentFinalGradeModel.courseId);
          if (this.courseSectionData.gradeScaleType !== 'Numeric') {
            this.addUpdateStudentFinalGradeModel.isPercent = false;
            this.getAllGradeScaleList(this.courseSectionData.standardGradeScaleId).then(()=>{
              if(this.courseSectionData.gradeScaleType === 'Teacher_Scale'){
                this.viewGradebookConfiguration().then(()=>{
                  this.searchScheduledStudentForGroupDropCall(markingPeriodDetails)
                })
              }
              else 
                this.searchScheduledStudentForGroupDropCall(markingPeriodDetails)
            })
          } else
          this.searchScheduledStudentForGroupDropCall(markingPeriodDetails)
          this.addUpdateStudentFinalGradeModel = res;
        }
      }
      else {
        this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  } else {
    this.studentMasterList = [];
    this.totalCount = 0;
    this.addUpdateStudentFinalGradeModel.markingPeriodId = null;
    this.selectedMarkingPeriod = undefined;
  }
  }

  searchScheduledStudentForGroupDropCall(markingPeriodDetails){
    this.scheduleStudentListViewModel.courseSectionIds = [this.courseSectionData.courseSectionId];
    this.scheduleStudentListViewModel.profilePhoto = true;
    this.scheduleStudentListViewModel.sortingModel = null;
    this.studentScheduleService.searchScheduledStudentForGroupDrop(this.scheduleStudentListViewModel).subscribe((res) => {
      if (res) {
      if(res._failure){
  
          this.showMessage = 'noRecordFound';
        } else {
          this.studentMasterList = res.scheduleStudentForView;
          this.studentMasterList.map((item: any) => {
            item.gradeScaleList = this.getGradeScaleList(item);
          });
          this.totalCount = this.studentMasterList.length;
          this.showCommentDetails(this.studentMasterList.length > 0 ? 0 : null);

          if (this.studentMasterList.length === 0) {
            this.showMessage = 'noRecordFound';
          }
          this.calculateFinalGradeModel(markingPeriodDetails);
        }
      }
    });
  }

  viewGradebookConfiguration() {
    return new Promise((resolve,reject)=>{
      this.gradebookConfigurationAddViewModel.gradebookConfiguration.courseId = this.courseSectionData.courseId;;
      this.gradebookConfigurationAddViewModel.gradebookConfiguration.courseSectionId = this.courseSectionData.courseSectionId;;
      this.gradeBookConfigurationService.viewGradebookConfiguration(this.gradebookConfigurationAddViewModel).subscribe(
        (res: GradebookConfigurationAddViewModel) => {
          if (res) {
            if (res._failure) {
            }
            else {
              this.gradebookConfigurationAddViewModel = res;
        
              this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationGradescale.map((data, i) => {
                this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationGradescale[i].title = this.gradeScaleList[0]?.grade[i].title;
              })
              this.gradeScaleList[0].grade=res.gradebookConfiguration.gradebookConfigurationGradescale;
              resolve('');
            }
          }
          else {
            this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
              duration: 10000
            });
          }
        }
      );
    })
  }

  calculateFinalGradeModel(markingPeriodDetails) {
    if (markingPeriodDetails.value === 'Custom') {
      this.addUpdateStudentFinalGradeModel.markingPeriodId = null;
      this.addUpdateStudentFinalGradeModel.isCustomMarkingPeriod = true;
    } else {
      this.addUpdateStudentFinalGradeModel.markingPeriodId = markingPeriodDetails.value;
    }
    this.addUpdateStudentFinalGradeModel.isExamGrade = markingPeriodDetails.doesExam;

    this.studentMasterList?.map((val:any,index)=>{
      if(!this.matchStudentData(val.studentId)){
        if(val.isDropped)
          this.addUpdateStudentFinalGradeModel.studentFinalGradeList.splice(index,0,new StudentFinalGrade());
        else{
          this.addUpdateStudentFinalGradeModel.studentFinalGradeList.splice(index,0,new StudentFinalGrade())
          this.initializeDefaultValues(val,index);
        }
      }
      if(this.courseSectionDetails[0]?.gradeScaleType === 'Teacher_Scale')
        val.gradeScaleList=this.gradebookConfigurationAddViewModel.gradebookConfiguration.gradebookConfigurationGradescale;
    });

    this.addUpdateStudentFinalGradeModel.studentFinalGradeList.map((item, i) => {
      let commentArray = [];
      item.studentFinalGradeComments?.map((subItem) => {
        const commentData = {
          courseCommentId: subItem.courseCommentCategory.courseCommentId,
          comments: subItem.courseCommentCategory.comments,
        }
        commentArray.push(commentData);
      });
      item.studentFinalGradeComments = commentArray;
      let standardGradeScaleIdData;
      let standardArray = [];
      if (item.studentFinalGradeStandard.length > 0) {
        item.studentFinalGradeStandard.map((subItem) => {
          if (subItem.standardGradeScaleId && subItem.gradeObtained) {
            const standardData = {
              standardGradeScaleId: subItem.standardGradeScaleId,
              gradeObtained: subItem.gradeObtained,
            }
            standardGradeScaleIdData=subItem.standardGradeScaleId;
            standardArray.push(standardData);
          }else{
            const standardData = {
              standardGradeScaleId: standardGradeScaleIdData,
              gradeObtained: 0,
            }
            standardArray.push(standardData);
          }
        });
      } else {
        standardArray = [{ standardGradeScaleId: null, gradeObtained: null }];
      }
      item.studentFinalGradeStandard = standardArray;
    });
  }

  matchStudentData(studentId){
    let isMatch=false;
    this.addUpdateStudentFinalGradeModel.studentFinalGradeList.map(innerVal=> { if(innerVal.studentId === studentId) isMatch=true; });
    return isMatch;
  }

  selectMarkingPeriod(data) {
    this.creditHours = data.creditHours;
  }
  
  getGradeScaleList(grade) {
    let gradeDataSet = [];
    if (this.gradeScaleList) {
      this.gradeScaleList.map(item => {
        if (item.gradeScaleId === grade.gradeScaleId) {
          gradeDataSet = item.grade;
        }
      });
    }
    return gradeDataSet;
  }

  getAllReportCardCommentsWithCategory(courseId) {
    this.reportCardComments = [];
    this.reportCardService.getAllCourseCommentCategory(this.getAllReportCardCommentsWithCategoryModel).subscribe((res) => {
      if (res) {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.reportCardCategoryWithComments.courseCommentCategories = []
          if (!res.courseCommentCategories) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        } else {

          this.reportCardCategoryWithComments = JSON.parse(JSON.stringify(res));
          let commentCategories = this.reportCardCategoryWithComments?.courseCommentCategories.filter(x => x.courseId === +courseId || x.applicableAllCourses);
          for (let commentCategory of commentCategories) {
            this.reportCardComments.push({ courseCommentId: commentCategory.courseCommentId, comments: commentCategory.comments });
          }

        }
      } else {
        this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }


  getAllCourseStandard(courseId) {
    this.courseStandardForCourseViewModel.courseId = +courseId;
    this.courseManager.getAllCourseStandardForCourse(this.courseStandardForCourseViewModel).subscribe(data => {
     if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
        this.courseStandardList=[];
        if(!data.courseStandards){
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
        }
      } else {

        this.courseStandardList = data.courseStandards;

      }
    });
  }


  selectedGrade(grade, index, student) {
    if (this.courseSectionData.gradeScaleType === 'Teacher_Scale')
      this.addUpdateStudentFinalGradeModel.studentFinalGradeList[index].percentMarks = student.filter(x => x.title === grade)[0].breakoffPoints;
    else
      this.addUpdateStudentFinalGradeModel.studentFinalGradeList[index].percentMarks = student.filter(x => x.title === grade)[0].breakoff;
  }

  gradeFromPercent(percent, index, student) {
    let sortedData = [];
    sortedData = student.sort((a, b) => b.breakoff - a.breakoff);
    sortedData.map((item, i) => {
      if (i === 0) {
        if (percent >= item.breakoff) {
          this.addUpdateStudentFinalGradeModel.studentFinalGradeList[index].gradeObtained = item.title;
        }
      } else {
        if (percent >= item.breakoff && percent < sortedData[i - 1].breakoff) {
          this.addUpdateStudentFinalGradeModel.studentFinalGradeList[index].gradeObtained = item.title;
        }
      }
    });
  }

  omitSpecialChar(event) {
    let k = event.charCode;
    return ((k >= 48 && k <= 57) || k === 8 || k === 46);
  }

  selected(event: MatAutocompleteSelectedEvent): void {
    if (this.addUpdateStudentFinalGradeModel.studentFinalGradeList[this.selectedStudent].studentFinalGradeComments.findIndex(item => item.courseCommentId === event.option.value.courseCommentId) === -1) {
      this.addUpdateStudentFinalGradeModel.studentFinalGradeList[this.selectedStudent].studentFinalGradeComments.push(event.option.value);
    }
  }

  percentToGrade(index) {
    if (this.addUpdateStudentFinalGradeModel.studentFinalGradeList[index].percentMarks >= 90 && this.addUpdateStudentFinalGradeModel.studentFinalGradeList[index].percentMarks <= 100) {
      this.addUpdateStudentFinalGradeModel.studentFinalGradeList[index].gradeObtained = 'A';

    }
    else if (this.addUpdateStudentFinalGradeModel.studentFinalGradeList[index].percentMarks >= 80 && this.addUpdateStudentFinalGradeModel.studentFinalGradeList[index].percentMarks < 90) {
      this.addUpdateStudentFinalGradeModel.studentFinalGradeList[index].gradeObtained = 'B';

    }
    else if (this.addUpdateStudentFinalGradeModel.studentFinalGradeList[index].percentMarks >= 70 && this.addUpdateStudentFinalGradeModel.studentFinalGradeList[index].percentMarks < 80) {
      this.addUpdateStudentFinalGradeModel.studentFinalGradeList[index].gradeObtained = 'C';

    }
    else if (this.addUpdateStudentFinalGradeModel.studentFinalGradeList[index].percentMarks >= 60 && this.addUpdateStudentFinalGradeModel.studentFinalGradeList[index].percentMarks < 70) {
      this.addUpdateStudentFinalGradeModel.studentFinalGradeList[index].gradeObtained = 'D';

    }
    else if (this.addUpdateStudentFinalGradeModel.studentFinalGradeList[index].percentMarks >= 0 && this.addUpdateStudentFinalGradeModel.studentFinalGradeList[index].percentMarks < 60) {
      this.addUpdateStudentFinalGradeModel.studentFinalGradeList[index].gradeObtained = 'F';

    }
  }

  addComments(id) {
    this.showCommentDetails(id);
    this.commentDetails = 1;
  }

  showCommentDetails(id) {
    if (id !== null) {
      this.selectedStudent = id;
      this.showComment = true;
    }
  }

  closeCommentDetails() {
    this.commentDetails = 0;
  }

  compareObjects(o1: any, o2: any): boolean {
    return o1.courseCommentId === o2.courseCommentId && o1.comments === o2.comments;
  }

  selectedReportComment(comment) {
  }

  removeComment(reportCommentId) {
    this.addUpdateStudentFinalGradeModel.studentFinalGradeList[this.selectedStudent].studentFinalGradeComments = this.addUpdateStudentFinalGradeModel.studentFinalGradeList[this.selectedStudent].studentFinalGradeComments.filter(item => item.courseCommentId !== +reportCommentId);
  }

  getAllGradeScaleList(standardGradeScaleId) {
    return new Promise((resolve, reject) => {
      this.gradesService.getAllGradeScaleList(this.gradeScaleListView).subscribe(data => {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          if (!data.gradeScaleList) {
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          }
        } else {
          this.gradeScaleStandardList = data.gradeScaleList.filter(x => x.gradeScaleId === +standardGradeScaleId)[0]?.grade;
          this.gradeScaleList = data.gradeScaleList.filter(x => x.useAsStandardGradeScale === false);
          resolve('');
        }
      });
    })

  }

  searchScheduledStudentForGroupDrop(courseSectionId) {
    this.scheduleStudentListViewModel.sortingModel = null;
    this.scheduleStudentListViewModel.courseSectionIds = [courseSectionId];
    this.scheduleStudentListViewModel.profilePhoto = true;
    this.studentScheduleService.searchScheduledStudentForGroupDrop(this.scheduleStudentListViewModel).subscribe((res) => {
      if (res) {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
        } else {
          this.studentMasterList = res.scheduleStudentForView;
          if (this.studentMasterList) {
            this.studentMasterList.map(item => {
              item.gradeScaleList = this.getGradeScaleList(item);
            });
          }
          this.totalCount = this.studentMasterList.length;
          this.addComments(this.studentMasterList.length > 0 ? 0 : null);
          if (this.studentMasterList.length === 0) {
          }
          this.addUpdateStudentFinalGradeModel.studentFinalGradeList = [new StudentFinalGrade()];
          this.studentMasterList.map((item, i) => {
            this.initializeDefaultValues(item, i);
            this.addUpdateStudentFinalGradeModel.studentFinalGradeList.push(new StudentFinalGrade());
          });
          this.scheduleStudentListViewModel = new ScheduleStudentListViewModel();
          this.addUpdateStudentFinalGradeModel.studentFinalGradeList.pop();
        }
      }
      else {
        this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  initializeDefaultValues(item, i) {
    for (let j = 0; j < this.gradeScaleStandardList?.length; j++) {
      this.addUpdateStudentFinalGradeModel.studentFinalGradeList[i].studentFinalGradeStandard[j] = {};
    }
    this.addUpdateStudentFinalGradeModel.studentFinalGradeList[i].studentId = item.studentId;
    this.addUpdateStudentFinalGradeModel.studentFinalGradeList[i].gradeId = item.gradeId;
    this.addUpdateStudentFinalGradeModel.studentFinalGradeList[i].gradeScaleId = item.gradeScaleId;
    this.addUpdateStudentFinalGradeModel.studentFinalGradeList[i].percentMarks = 0;
    this.addUpdateStudentFinalGradeModel.studentFinalGradeList[i].basedOnStandardGrade = true;
    this.addUpdateStudentFinalGradeModel.studentFinalGradeList[i].studentFinalGradeSrlno = 0;
    this.addUpdateStudentFinalGradeModel.studentFinalGradeList[i].gradeObtained = null;
    this.addUpdateStudentFinalGradeModel.studentFinalGradeList[i].teacherComment = null;
    this.addUpdateStudentFinalGradeModel.studentFinalGradeList[i].studentFinalGradeComments = [];

  }

  setValue(standardGradeScaleId, gradeObtained, i) {
    this.addUpdateStudentFinalGradeModel.studentFinalGradeList[this.selectedStudent].studentFinalGradeStandard[i] = { standardGradeScaleId: standardGradeScaleId, gradeObtained: gradeObtained };
  }

  findMarkingPeriodTitleById(courseDetails) {
    if (courseDetails.yrMarkingPeriodId) {
      courseDetails.markingPeriodId = '0_' + courseDetails.yrMarkingPeriodId;
    } else if (courseDetails.smstrMarkingPeriodId) {
      courseDetails.markingPeriodId = '1_' + courseDetails.smstrMarkingPeriodId;
    } else if (courseDetails.qtrMarkingPeriodId) {
      courseDetails.markingPeriodId = '2_' + courseDetails.qtrMarkingPeriodId;
    } else if (courseDetails.prgrsprdMarkingPeriodId) {
      courseDetails.markingPeriodId = '3_' + courseDetails.prgrsprdMarkingPeriodId;
    }
    else {
      courseDetails.markingPeriodId = this.getMarkingPeriodTitleListModel.getMarkingPeriodView[0].value;
    }
    return courseDetails;

  }
  onCheckboxChange(value) {
    if (value.currentTarget.checked) {
      this.addUpdateStudentFinalGradeModel.isPercent = false;
      this.isPercent = false;
    }
    else {
      this.addUpdateStudentFinalGradeModel.isPercent = true;
      this.isPercent = true;
    }
  }


  submitFinalGrade() {
    // this.addUpdateStudentFinalGradeModel.academicYear = this.defaultValuesService.getAcademicYear();
    delete this.addUpdateStudentFinalGradeModel.academicYear;
    this.addUpdateStudentFinalGradeModel.creditHours = this.creditHours;
    let cloneModel=JSON.stringify(this.addUpdateStudentFinalGradeModel);
    this.cloneAddUpdateStudentFinalGradeModel = JSON.parse(cloneModel)
    this.cloneAddUpdateStudentFinalGradeModel.studentFinalGradeList.map((val,index)=>{
      if(val.isDropped)
        this.cloneAddUpdateStudentFinalGradeModel.studentFinalGradeList.splice(index,1);
    });
    this.finalGradeService.addUpdateStudentFinalGrade(this.cloneAddUpdateStudentFinalGradeModel).subscribe((data) => {
      if (data) {
       if(data._failure){
        
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
        } else {
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
        }
      }
      else {
        this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }
  
  getGradeBookGrades() {
    this.getGradebookGradeinFinalGradeModel.courseSectionId = this.addUpdateStudentFinalGradeModel.courseSectionId;
    this.getGradebookGradeinFinalGradeModel.markingPeriodId = this.addUpdateStudentFinalGradeModel.markingPeriodId;

    this.finalGradeService.getGradebookGradeinFinalGrade(this.getGradebookGradeinFinalGradeModel).subscribe((res: any) => {
      if (res) {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        this.snackbar.open(res._message, '', {
          duration: 10000
        });      
        } else {
          res.studentWithGradeBookViewModelList.map((item)=>{
            this.addUpdateStudentFinalGradeModel.studentFinalGradeList.map((subItem)=>{
              if(subItem.studentId === item.studentId) {
                subItem.percentMarks = item.percentage;
                subItem.gradeObtained = item?.grade?.trim()?.length > 0 ? item.grade : null;
              }
            })
          })
        }
      }
      else {
        this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }
}
