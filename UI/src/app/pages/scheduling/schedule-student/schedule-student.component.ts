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

import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AddStudentComponent } from './add-student/add-student.component';
import { AddCourseSectionComponent } from './add-course-section/add-course-section.component';
import { TranslateService } from '@ngx-translate/core';
import { StudentScheduleService } from '../../../services/student-schedule.service';
import { StudentCourseSectionScheduleAddViewModel, StudentScheduleReportViewModel } from '../../../models/student-schedule.model';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { LoaderService } from '../../../services/loader.service';
import { MatTableDataSource } from '@angular/material/table';
import { ExcelService } from '../../../services/excel.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TitleCasePipe } from '@angular/common';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { Permissions } from '../../../models/roll-based-access.model';
import { CommonService } from '../../../services/common.service';
import { DefaultValuesService } from '../../../common/default-values.service';
import { GetAllSectionModel } from '../../../models/section.model';
import { GetAllGradeLevelsModel } from '../../../models/grade-level.model';
import { GradeLevelService } from '../../../services/grade-level.service';
import { LoginService } from '../../../services/login.service';
import { LanguageModel } from '../../../models/language.model';
import { SectionService } from '../../../services/section.service';
import { MarkingPeriodService } from '../../../services/marking-period.service';
import { CourseManagerService } from '../../../services/course-manager.service';
import { GetAllCourseListModel, GetAllProgramModel, GetAllSubjectModel } from '../../../models/course-manager.model';
import { GetMarkingPeriodTitleListModel } from '../../../models/marking-period.model';

@Component({
  selector: 'vex-schedule-student',
  templateUrl: './schedule-student.component.html',
  styleUrls: ['./schedule-student.component.scss'],
  providers: [TitleCasePipe]
})
export class ScheduleStudentComponent implements OnInit, OnDestroy {
  studentList = [];
  languageList;
  sectionList = [];
  programList = [];
  subjectList = [];
  courseList = [];
  markingPeriodList = [];
  gradeLevelList = [];
  studentText: string;
  sectionText: string;
  viewReport: boolean = false;
  failedScheduling: boolean = false
  showReportTable: boolean = false;
  courseSectionList = [];
  showStudentCount: boolean = false;
  showCourseSectionCount: boolean = false;
  destroySubject$: Subject<void> = new Subject();
  languages: LanguageModel = new LanguageModel();
  getAllSubjectModel: GetAllSubjectModel = new GetAllSubjectModel();
  getAllCourseListModel: GetAllCourseListModel = new GetAllCourseListModel();
  getAllProgramModel: GetAllProgramModel = new GetAllProgramModel();
  getAllGradeLevelsModel: GetAllGradeLevelsModel = new GetAllGradeLevelsModel();
  getMarkingPeriodTitleListModel: GetMarkingPeriodTitleListModel = new GetMarkingPeriodTitleListModel();
  studentCourseSectionScheduleAddViewModel: StudentCourseSectionScheduleAddViewModel = new StudentCourseSectionScheduleAddViewModel();
  studentScheduleReportViewModel: StudentScheduleReportViewModel = new StudentScheduleReportViewModel();
  loading: boolean;
  showCard: boolean = false;
  scheduleReport: MatTableDataSource<any>;
  displayedColumns: string[];
  permissions: Permissions;
  constructor(private dialog: MatDialog, public translateService: TranslateService,
    private studentScheduleService: StudentScheduleService,
    private loaderService: LoaderService,
    private excelService: ExcelService,
    private gradeLevelService: GradeLevelService,
    private loginService: LoginService,
    private sectionService: SectionService,
    private snackbar: MatSnackBar,
    private markingPeriodService: MarkingPeriodService,
    private courseManagerService: CourseManagerService,
    private titlecasePipe: TitleCasePipe,
    private pageRolePermissions: PageRolesPermission,
    private commonService: CommonService,
    private defaultValuesService: DefaultValuesService
  ) {
    //translateService.use('en');
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
  }

  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission();
    this.getAllLanguage();
    this.getAllSection();
    this.getAllGradeLevelList();
    this.getAllCourse();
    this.getAllSubjectList();
    this.getAllProgramList();
    this.getAllMarkingPeriodList();
  }

  getAllLanguage() {
    this.languages._tenantName = this.defaultValuesService.getTenantName();
    this.loginService.getAllLanguage(this.languages).pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        this.languageList = [];
      }
      else {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          if (res.tableLanguage) {
            this.languageList = []
          } else {
            this.languageList = []
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        } else {
          this.languageList = res.tableLanguage?.sort((a, b) => { return a.locale < b.locale ? -1 : 1; })
        }
      }
    })
  }

  getAllSection() {
    let section: GetAllSectionModel = new GetAllSectionModel();
    this.sectionService.GetAllSection(section).pipe(takeUntil(this.destroySubject$)).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
        if (!data.tableSectionsList) {
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
        }
      }
      else {
        this.sectionList = data.tableSectionsList;
      }

    });
  }


  getAllGradeLevelList() {
    this.gradeLevelService.getAllGradeLevels(this.getAllGradeLevelsModel).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
      }
      else {
        this.gradeLevelList = data.tableGradelevelList;
      }

    });
  }

  getAllProgramList() {
    this.courseManagerService.GetAllProgramsList(this.getAllProgramModel).subscribe(data => {
      if(data){
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          this.programList=[];
          if(!data.programList){
            this.snackbar.open(data._message, '', {
              duration: 1000
            }); 
          }
        }else{
          this.programList=data.programList;
        }
      }else{
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 1000
        }); 
      }  
    });
  }
  getAllSubjectList() {
    this.courseManagerService.GetAllSubjectList(this.getAllSubjectModel).subscribe(data => {
      if(data){
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          this.subjectList=[];
          if(!data.subjectList){
            this.snackbar.open(data._message, '', {
              duration: 1000
            }); 
          }
        }else{
          this.subjectList=data.subjectList;
        }
      }else{
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 1000
        }); 
      } 

    });
  }

  getAllMarkingPeriodList() {
    this.getMarkingPeriodTitleListModel.schoolId = this.defaultValuesService.getSchoolID();
    this.getMarkingPeriodTitleListModel.academicYear = this.defaultValuesService.getAcademicYear();
    this.markingPeriodService.getAllMarkingPeriodList(this.getMarkingPeriodTitleListModel).subscribe(data => {
     if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
        this.getMarkingPeriodTitleListModel.getMarkingPeriodView = [];
        if(!this.getMarkingPeriodTitleListModel?.getMarkingPeriodView){
          this.snackbar.open(data._message, '', {
            duration: 1000
          }); 
        }
      } else {
        this.getMarkingPeriodTitleListModel.getMarkingPeriodView = data.getMarkingPeriodView;
      }
    });
  }

  getAllCourse() {
    this.courseManagerService.GetAllCourseList(this.getAllCourseListModel).subscribe(data => {
      if (data) {
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          this.courseList = [];
          if (!data.courseViewModelList) {
            this.snackbar.open(data._message, '', {
              duration: 1000
            });
          }
        } else {
          this.courseList = data.courseViewModelList;
        }
      } else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    })
  }

  viewScheduledReport() {
    this.studentScheduleService.studentScheduleReport(this.studentScheduleReportViewModel).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);

      }
      else {
        if (data.scheduleReport.length > 0) {
          this.showReportTable = true;
        }
        data.scheduleReport.map((item) => {
          for (const key in item) {
            if (item.hasOwnProperty(key)) {
              if (key === 'studentId') {
                delete item[key];
              }
            }
          }
        });
        this.displayedColumns = Object.keys(data.scheduleReport[0]);
        this.scheduleReport = new MatTableDataSource(data.scheduleReport);
      }

    });
  }

  selectStudent() {
    if(this.defaultValuesService.checkAcademicYear()){
    this.dialog.open(AddStudentComponent, {
      width: '900px',
      data:{
        sectionList: this.sectionList,
        languageList: this.languageList,
        gradeLevelList: this.gradeLevelList
      }
    }).afterClosed().subscribe((data) => {
      this.studentList = data;
      if (this.studentList?.length > 0) {
        if (this.studentList?.length > 1) {
          this.studentText = 's';
        }
        else {
          this.studentText = '';
        }
        this.showStudentCount = true;
      }
      else {
        this.showStudentCount = false;
      }
      this.showCard = false;
      this.viewReport = false;
      this.showReportTable = false;
    });
  }
  }

  selectCourseSection() {
    if(this.defaultValuesService.checkAcademicYear()){
    this.dialog.open(AddCourseSectionComponent, {
      width: '900px',
      data:{
        courseList: this.courseList,
        subjectList: this.subjectList,
        programList: this.programList,
        markingPeriodList: this.getMarkingPeriodTitleListModel.getMarkingPeriodView
      }
    }).afterClosed().subscribe((data) => {
      this.courseSectionList = data;
      if (this.courseSectionList?.length > 0) {
        if (this.studentList?.length > 1) {
          this.sectionText = 's';
        }
        else {
          this.sectionText = '';
        }
        this.showCourseSectionCount = true;
      }
      else {
        this.showCourseSectionCount = false;
      }
      this.showCard = false;
      this.viewReport = false;
      this.showReportTable = false;
    });
    }
  }

  translateKey(key) {
    let convertedKey;
    this.translateService.get(key).subscribe((res: string) => {
      convertedKey = res;
    });
    return this.titlecasePipe.transform(convertedKey);
  }

  scheduleStudent() {
    this.showCard = true;
    this.studentCourseSectionScheduleAddViewModel.courseSectionList = this.courseSectionList;
    this.studentCourseSectionScheduleAddViewModel.studentMasterList = this.studentList;
    this.studentCourseSectionScheduleAddViewModel.createdBy = this.defaultValuesService.getUserName();
    this.studentScheduleService.addStudentCourseSectionSchedule(this.studentCourseSectionScheduleAddViewModel).pipe(takeUntil(this.destroySubject$)).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
        this.studentCourseSectionScheduleAddViewModel.conflictMessage = 'Failed to schedule student(s) to course section(s)';
        this.failedScheduling = true;
      }
      else {
        this.studentCourseSectionScheduleAddViewModel = data;
        this.studentCourseSectionScheduleAddViewModel.conflictMessage = data.conflictMessage;
        this.viewReport = true;
      }
    });
  }

  refreshAll() {
    this.studentList = [];
    this.courseSectionList = [];
    this.showStudentCount = false;
    this.showCourseSectionCount = false;
    this.showCard = false;
    this.viewReport = false;
    this.showReportTable = false;
  }

  viewExcelReport() {

    this.studentScheduleService.studentScheduleReport(this.studentScheduleReportViewModel).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);

      }
      else {
        let modifiedReportData = [];
        data.scheduleReport.map((item) => {
          let obj = {};
          for (const key in item) {
            if (item.hasOwnProperty(key)) {
              if (key === 'studentId') {
                delete item[key];
              } else {
                Object.assign(obj, { [key === 'studentInternalId' ? 'Student ID' : this.translateKey(key)]: item[key] })
              }
            }
          }
          modifiedReportData.push(obj);
        })
        for (let report of modifiedReportData) {
          for (let key in report) {
            if (report.hasOwnProperty(key)) {
              if (report[key]?.split('|')[0].includes('False')) {
                report[key] = report[key]?.split('|')[1].trim()
              } else if (report[key]?.split('|')[0].includes('True')) {
                report[key] = ''
              }
            }
          }
        }

        if (modifiedReportData.length > 0) {
          this.excelService.exportAsExcelFile(modifiedReportData, 'Schedule_Report_List_')
        }
        else {
          this.snackbar.open('No Records Found. Failed to Export Schedule Report', '', {
            duration: 5000
          });
        }
      }

    });
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
