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

import { Component, ElementRef, OnInit, ViewChild } from "@angular/core";
import { MatDialog } from "@angular/material/dialog";
import { Router } from "@angular/router";
import { TranslateService } from "@ngx-translate/core";
import icSearch from '@iconify/icons-ic/search';
import { StudentDetails } from '../../../models/student-details.model';
import { StudentListModel } from "../../../../app/models/student.model";
import { StudentService } from "../../../../app/services/student.service";
import { MatTableDataSource } from "@angular/material/table";
import { MatSnackBar } from "@angular/material/snack-bar";
import { MarkingPeriodService } from "../../../../app/services/marking-period.service";
import { GetMarkingPeriodByCourseSectionModel, GetMarkingPeriodTitleListModel } from "src/app/models/marking-period.model";
import { AddReportCardPdf } from "../../../../app/models/report-card.model";
import { ReportCardService } from "../../../../app/services/report-card.service";
import { MatCheckbox } from "@angular/material/checkbox";
import { SearchFilter, SearchFilterAddViewModel, SearchFilterListViewModel } from "src/app/models/search-filter.model";
import { CommonService } from "../../../../app/services/common.service";
import { ConfirmDialogComponent } from "../../shared-module/confirm-dialog/confirm-dialog.component";
import { fadeInUp400ms } from "../../../../@vex/animations/fade-in-up.animation";
import { stagger40ms } from "../../../../@vex/animations/stagger.animation";
import { fadeInRight400ms } from "../../../../@vex/animations/fade-in-right.animation";
import { FormControl } from "@angular/forms";
import { MatSort } from "@angular/material/sort";
import { MatPaginator, MatPaginatorIntl } from "@angular/material/paginator";
import { LoaderService } from "../../../../app/services/loader.service";
import { debounceTime, distinctUntilChanged } from "rxjs/operators";
import { DefaultValuesService } from "../../../../app/common/default-values.service";
import { Permissions } from "../../../models/roll-based-access.model";
import { PageRolesPermission } from "../../../common/page-roles-permissions.service";
import { reportCardType } from "../../../common/static-data";
import * as html2pdf from 'html2pdf.js';
import { ScheduleStudentListViewModel } from "src/app/models/student-schedule.model";
import { StudentScheduleService } from "src/app/services/student-schedule.service";

@Component({
  selector: "vex-report-cards",
  templateUrl: "./report-cards.component.html",
  styleUrls: ["./report-cards.component.scss"],
  animations: [
    fadeInUp400ms,
    stagger40ms,
    fadeInRight400ms
  ]
})
export class ReportCardsComponent implements OnInit {

  icSearch = icSearch;
  getAllStudent: StudentListModel = new StudentListModel();
  totalCount: number = 0;
  StudentModelList: MatTableDataSource<any>;
  pageNumber: number;
  pageSize: number;
  getMarkingPeriodTitleListModel: GetMarkingPeriodTitleListModel = new GetMarkingPeriodTitleListModel();
  markingPeriodList = [];
  markingPeriods;
  addReportCardPdf: AddReportCardPdf = new AddReportCardPdf();
  pdfData;
  listOfStudent = [];
  @ViewChild('masterCheckBox') private masterCheckBox: MatCheckbox;
  showAdvanceSearchPanel: boolean = false;
  filterJsonParams: any;
  searchFilter: any;
  showSaveFilter: boolean;
  showLoadFilter: boolean;
  searchCount: any;
  searchFilterAddViewModel: SearchFilterAddViewModel = new SearchFilterAddViewModel();
  searchFilterListViewModel: SearchFilterListViewModel = new SearchFilterListViewModel();
  toggleValues: any;
  searchValue: any;
  searchCtrl = new FormControl();
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  markingPeriodError: boolean;
  loading: boolean;
  displayedColumns: string[] = ['studentSelected', 'studentName', 'studentId', 'alternateId', 'gradeLevelTitle', 'section', 'homePhone'];
  permissions: Permissions;
  reportCardType = reportCardType;
  @ViewChild('printSectionId') printEl: ElementRef;
  generatedReportCardData;
  getMarkingPeriodByCourseSectionModel: GetMarkingPeriodByCourseSectionModel = new GetMarkingPeriodByCourseSectionModel();
  scheduleStudentListViewModel: ScheduleStudentListViewModel = new ScheduleStudentListViewModel();
  teacherSearchInput:any;
  isAdmin:boolean;
  halfLengthOfComment:number = 0;
  constructor(
    private router: Router,
    private dialog: MatDialog,
    public translateService: TranslateService,
    private studentService: StudentService,
    private snackbar: MatSnackBar,
    private markingPeriodService: MarkingPeriodService,
    private reportCardService: ReportCardService,
    private commonService: CommonService,
    private loaderService: LoaderService,
    private pageRolePermissions: PageRolesPermission,
    private studentScheduleService: StudentScheduleService,
    private defaultValuesService: DefaultValuesService,
    private paginatorObj: MatPaginatorIntl,
  ) {
    paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
    // translateService.use("en");
    this.markingPeriods = [];
    this.addReportCardPdf.templateType='default';
    if (this.defaultValuesService.getUserMembershipType() === "Teacher" || this.defaultValuesService.getUserMembershipType() === "Homeroom Teacher")
      this.isAdmin = false
    else
      this.isAdmin = true
  }


  ngOnInit(): void {
   

    this.permissions = this.pageRolePermissions.checkPageRolePermission();
    this.getAllStudent.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;

    this.getAllMarkingPeriodList();
    this.getAllStudentList();
    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    });

    this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term.trim().length > 0) {
        let filterParams = [
          {
            columnName: null,
            filterValue: term,
            filterOption: 3
          }
        ]
        if (this.sort.active != undefined && this.sort.direction != "") {
          this.getAllStudent.sortingModel.sortColumn = this.sort.active;
          this.getAllStudent.sortingModel.sortDirection = this.sort.direction;
        }
        Object.assign(this.getAllStudent, { filterParams: filterParams });
        this.getAllStudent.pageNumber = 1;
        this.paginator.pageIndex = 0;
        this.getAllStudent.pageSize = this.pageSize;
        this.getAllStudentList();
      }
      else {
        Object.assign(this.getAllStudent, { filterParams: null });
        this.getAllStudent.pageNumber = this.paginator.pageIndex + 1;
        this.getAllStudent.pageSize = this.pageSize;
        if (this.sort.active != undefined && this.sort.direction != "") {
          this.getAllStudent.sortingModel.sortColumn = this.sort.active;
          this.getAllStudent.sortingModel.sortDirection = this.sort.direction;
        }
        this.getAllStudentList();
      }
    })

  }




  getAllStudentList() {
    if (this.isAdmin) {
      if (this.getAllStudent.sortingModel?.sortColumn == "") {
        this.getAllStudent.sortingModel = null;
      }
      if(this.teacherSearchInput){
        this.getAllStudent.filterParams = this.teacherSearchInput;
      }
      this.studentService.GetAllStudentList(this.getAllStudent).subscribe(data => {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          if (data.studentListViews === null) {
            this.totalCount = null;
            this.listOfStudent = [];
            this.StudentModelList = new MatTableDataSource(this.listOfStudent);
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          } else {
            this.listOfStudent = [];
            this.StudentModelList = new MatTableDataSource(this.listOfStudent);
            this.totalCount = null;
          }
        } else {
          this.totalCount = data.totalCount;
          this.pageNumber = data.pageNumber;
          this.pageSize = data._pageSize;
          this.listOfStudent = data.studentListViews;
          this.StudentModelList = new MatTableDataSource(this.listOfStudent);
          this.getAllStudent = new StudentListModel();
        }
      });
    }else{
      if (this.getAllStudent.sortingModel?.sortColumn == "") {
        this.getAllStudent.sortingModel = null;
      }
      this.scheduleStudentListViewModel.staffId = this.defaultValuesService.getUserId();
      this.scheduleStudentListViewModel.academicYear = this.defaultValuesService.getAcademicYear();
      if(this.teacherSearchInput){
        this.scheduleStudentListViewModel.filterParams = this.teacherSearchInput;
      }
      this.scheduleStudentListViewModel.sortingModel = null;
      this.studentScheduleService.searchScheduledStudentForGroupDrop(this.scheduleStudentListViewModel).subscribe(data => {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          if (data.scheduleStudentForView === null) {
            this.totalCount = null;
            this.listOfStudent = [];
            this.StudentModelList = new MatTableDataSource(this.listOfStudent);
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          } else {
            this.listOfStudent = [];
            this.StudentModelList = new MatTableDataSource(this.listOfStudent);
            this.totalCount = null;
          }
        } else {
          this.totalCount = data.totalCount;
          this.pageNumber = data.pageNumber;
          this.pageSize = data._pageSize;
          data.scheduleStudentForView.map(item => {
            item.gradeLevelTitle = item.gradeLevel;
            item.sectionName = item.section;
          });
          this.listOfStudent = data.scheduleStudentForView;
          this.StudentModelList = new MatTableDataSource(this.listOfStudent);
          this.getAllStudent = new StudentListModel();
        }
      });
    }
  }

  getPageEvent(event) {
    
    if (this.sort.active != undefined && this.sort.direction != "") {
      this.getAllStudent.sortingModel.sortColumn = this.sort.active;
      this.getAllStudent.sortingModel.sortDirection = this.sort.direction;
    }
    if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
      let filterParams = [
        {
          columnName: null,
          filterValue: this.searchCtrl.value,
          filterOption: 3
        }
      ]
      Object.assign(this.getAllStudent, { filterParams: filterParams });
    }
    this.getAllStudent.pageNumber = event.pageIndex + 1;
    this.getAllStudent.pageSize = event.pageSize;
    this.defaultValuesService.setPageSize(event.pageSize);
    this.getAllStudentList();
  }

  modifyMarkingPeriodDataSet(res) {
    let dataSetForMarkingPeriod = [];
    let dataSetForAttendance = [];
    res.markingPeriodDetailsForOtherTemplates.map((item, markingPeriodIndex)=>{
      if(item.courseSectionGradeDetailsForOtherTemplates.length > 0) {

        item.courseSectionGradeDetailsForOtherTemplates.map((subItem)=>{
          const index = dataSetForMarkingPeriod.findIndex(x=> x.courseSectionName === subItem.courseSectionName)
          if(index === -1) {
          const data = {
            courseSectionName: subItem.courseSectionName,
            percentageAndGrade: []
          }
          data.percentageAndGrade[markingPeriodIndex] = {percentage: subItem.percentage, grade: subItem.grade, markingPeriodShortName: subItem.markingPeriodShortName}
          dataSetForMarkingPeriod.push(data)
        } else {
          dataSetForMarkingPeriod[index].percentageAndGrade[markingPeriodIndex] = {percentage: subItem.percentage, grade: subItem.grade, markingPeriodShortName: subItem.markingPeriodShortName}
        }
        })
      }
      if(item.attendanceDetailsForOtherTemplates.length > 0) {
        item.attendanceDetailsForOtherTemplates.map((subItem)=>{
          const index = dataSetForAttendance.findIndex(x=> x.attendanceTitle === subItem.attendanceTitle)
          if(index === -1) {
          const data = {
            attendanceTitle: subItem.attendanceTitle,
            attendance: []
          }
          data.attendance[markingPeriodIndex] = {attendanceCount: subItem.attendanceCount, markingPeriodShortName: subItem.markingPeriodShortName}
          dataSetForAttendance.push(data)
        } else {
          dataSetForAttendance[index].attendance[markingPeriodIndex] = {attendanceCount: subItem.attendanceCount, markingPeriodShortName: subItem.markingPeriodShortName}
        }
        })
      }
    })
    return {courseSectionGradeDetailsForOtherTemplates: dataSetForMarkingPeriod, attendanceDetailsForOtherTemplates: dataSetForAttendance};
  }


  resetStudentList() {
    this.searchCount = null;
    this.searchValue = null;
    this.getAllStudentList();    
  }

  markingPeriodChecked(event, markingPeriod) {
    if (event.checked) {
      this.markingPeriods.push(markingPeriod.value);
    } else {
      this.markingPeriods.splice(this.markingPeriods.findIndex(x => x === markingPeriod.value), 1);
    }
    this.markingPeriodError = this.markingPeriods.length > 0 ? false : true;
  }

  selectedStudent(studentId, event) {
    if (event.checked) {
      this.addReportCardPdf.studentsReportCardViewModelList.push({ studentId });
      this.listOfStudent.map((item) => {
        if (item.studentId === studentId) {
          item.checked = true;
        }
      });
    } else {
      this.addReportCardPdf.studentsReportCardViewModelList.splice(this.addReportCardPdf.studentsReportCardViewModelList.findIndex(x => x.studentId === studentId), 1);
    }
    this.masterCheckBox.checked = this.listOfStudent.every((item) => {
      return item.checked;
    });
  }

  getSearchResult(res) {
    this.getAllStudent = new StudentListModel();
    if (res.totalCount) {
      this.searchCount = res.totalCount;
      this.totalCount = res.totalCount;
    }
    else {
      this.searchCount = 0;
      this.totalCount = 0;
    }
    this.showSaveFilter = true;
    this.pageNumber = res.pageNumber;
    this.pageSize = res._pageSize;
    this.StudentModelList = new MatTableDataSource(res.studentListViews);
    this.getAllStudent = new StudentListModel();
  }

  getAllSearchFilter() {
    this.searchFilterListViewModel.module = 'Student';
    this.commonService.getAllSearchFilter(this.searchFilterListViewModel).subscribe((res) => {
      if (typeof (res) === 'undefined') {
        this.snackbar.open('Filter list failed. ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
      else {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          this.searchFilterListViewModel.searchFilterList = []
          if (!res.searchFilterList) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        }
        else {
          this.searchFilterListViewModel = res;
          let filterData = this.searchFilterListViewModel.searchFilterList.filter(x => x.filterId == this.searchFilter.filterId);
          if (filterData.length > 0) {
            this.searchFilter.jsonList = filterData[0].jsonList;
          }
          if (this.filterJsonParams == null) {
            this.searchFilter = this.searchFilterListViewModel.searchFilterList[this.searchFilterListViewModel.searchFilterList.length - 1];
          }
        }
      }
    }
    );
  }

  hideAdvanceSearch(event) {
    this.showSaveFilter = event.showSaveFilter;
    this.showAdvanceSearchPanel = false;
    if (event.showSaveFilter == false) {
      this.getAllSearchFilter();
    }
  }

  getSearchInput(event) {
    this.searchValue = event;
  }

  editFilter() {
    this.showAdvanceSearchPanel = true;
    this.filterJsonParams = this.searchFilter;
    this.showSaveFilter = false;
    this.showLoadFilter = false;
  }

  deleteFilter() {

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: {
        title: 'Are you sure?',
        message: 'You are about to delete ' + this.searchFilter.filterName + '.'
      }
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult) {
        this.deleteFilterdata(this.searchFilter);
      }
    });
  }

  deleteFilterdata(filterData) {
    this.searchFilterAddViewModel.searchFilter = filterData;
    this.commonService.deleteSearchFilter(this.searchFilterAddViewModel).subscribe(
      (res: SearchFilterAddViewModel) => {
        if (typeof(res) === 'undefined'){
          this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
        else {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open('' + res._message, '', {
              duration: 10000
            });
          }
          else {
            this.getAllSearchFilter();
            this.getAllStudent.filterParams = null;
            this.getAllStudentList();
            this.searchFilter = new SearchFilter();
            this.showLoadFilter = true;
          }
        }
      }
    );
  }
  getTeacherSearchInput(event){
    this.teacherSearchInput=event;
    this.getAllStudentList();    
    this.showAdvanceSearchPanel = false;
  }

  getToggleValues(event) {
    this.toggleValues = event;
    if (event.inactiveStudents === true) {
      // this.columns[6].visible = true;
    }
    else if (event.inactiveStudents === false) {
      // this.columns[6].visible = false;
    }
  }

  someComplete(): boolean {
    let indetermine = false;
    for (let user of this.listOfStudent) {
      for (let selectedUser of this.addReportCardPdf.studentsReportCardViewModelList) {
        if (user.studentId === selectedUser.studentId) {
          indetermine = true;
        }
      }
    }
    if (indetermine) {
      this.masterCheckBox.checked = this.listOfStudent.every((item) => {
        return item.checked;
      })
      if (this.masterCheckBox.checked) {
        return false;
      } else {
        return true;
      }
    }
  }

  decideCheckUncheck() {
    this.listOfStudent.map((item) => {
      let isIdIncludesInSelectedList = false;
      if (item.checked) {
        for (let selectedUser of this.addReportCardPdf.studentsReportCardViewModelList) {
          if (item.studentId == selectedUser.studentId) {
            isIdIncludesInSelectedList = true;
            break;
          }
        }
        if (!isIdIncludesInSelectedList) {
          this.addReportCardPdf.studentsReportCardViewModelList.push({ studentId: item.studentId });
        }
      } else {
        for (let selectedUser of this.addReportCardPdf.studentsReportCardViewModelList) {
          if (item.studentId == selectedUser.studentId) {
            this.addReportCardPdf.studentsReportCardViewModelList = this.addReportCardPdf.studentsReportCardViewModelList.filter((user) => user.studentId != item.studentId);
            break;
          }
        }
      }
      isIdIncludesInSelectedList = false;

    });
  }

  setAll(event) {
    this.listOfStudent.map(user => {
      user.checked = event;
      if (event) {
        this.addReportCardPdf.studentsReportCardViewModelList.push({ studentId: user.studentId });
      } else {
        this.addReportCardPdf.studentsReportCardViewModelList = [];
      }
    });
    this.StudentModelList = new MatTableDataSource(this.listOfStudent);
  }

  doCheck(studentId) {
    return this.addReportCardPdf.studentsReportCardViewModelList.findIndex(x => x.studentId === studentId) === -1 ? false : true;
  }

  getAllMarkingPeriodList() {
    this.addReportCardPdf.academicYear = this.defaultValuesService.getAcademicYear();
    this.getMarkingPeriodByCourseSectionModel.isReportCard = true;
    this.markingPeriodService.getMarkingPeriodsByCourseSection(this.getMarkingPeriodByCourseSectionModel).subscribe((res) => {
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
        this.markingPeriodList = [];
        if (!res.getMarkingPeriodView) {
          this.snackbar.open(res._message, '', {
            duration: 1000
          });
        }
      } else {
        this.markingPeriodList = res.getMarkingPeriodView;
      }
    })
  }

  addAndGenerateReportCard() {
    return new Promise((resolve, reject) => {
      this.addReportCardPdf.markingPeriods = this.markingPeriods.toString();
      this.reportCardService.getReportCardForStudents(this.addReportCardPdf).subscribe((res) => {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          this.snackbar.open(res._message, '', {
            duration: 1000
          });
        } else {
          resolve(res);
        }
      })
    });
  }

  generateReportCard() {
    if (this.markingPeriods.length > 0) {
      this.addAndGenerateReportCard().then((res: any) => {
        this.generatedReportCardData = res;
        if(this.addReportCardPdf?.templateType === 'default') {
          if(this.generatedReportCardData?.studentsReportCardViewModelList[0]?.courseCommentCategories?.length>0) {
            this.halfLengthOfComment = Math.floor(this.generatedReportCardData?.studentsReportCardViewModelList[0]?.courseCommentCategories?.length/2);
          }
          setTimeout(() => {
            this.generatePdfForDefault();
            }, 100*this.generatedReportCardData.studentsReportCardViewModelList.length);
        } else {
          this.generatedReportCardData.studentsReportCardViewModelList.map((res: any)=>{
            Object.assign(res, this.modifyMarkingPeriodDataSet(res))
          });
          setTimeout(() => {
            this.generatePdfForOthers();
            }, 100*this.generatedReportCardData.studentsReportCardViewModelList.length);
        }
      });
    } else {
      this.markingPeriodError = true;
    }
  }

  backToList() {
    this.pdfData = null
  }

  generatePdfForOthers() {
    let printContents, popupWin;
    printContents = document.getElementById('printSectionId').innerHTML;
    document.getElementById('printSectionId').className = 'block';
    popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
    if(popupWin === null || typeof(popupWin)==='undefined'){
      document.getElementById('printSectionId').className = 'hidden';
      this.snackbar.open("User needs to allow the popup from the browser", '', {
        duration: 10000
      });
    } else {
    popupWin.document.open();
    popupWin.document.write(`
      <html>
        <head>
          <title>Print tab</title>
          <style>
          *{
            font-family: \"Arial\", \"Helvetica\", \"sans-serif\";
          }

          body, h1, h2, h3, h4, h5, h6, p { margin: 0; }

          body { -webkit-print-color-adjust: exact; }
          
          table { border-collapse: collapse; width: 100%; }
          
          .float-left { float: left; }
          
          .float-right { float: right; }
          
          .text-center { text-align: center; }
          
          .text-right { text-align: right; }
          
          .ml-auto { margin-left: auto; }
          
          .m-auto { margin: auto; }
          
          .report-card { width: 900px; margin: auto; font-family: \"Arial\", \"Helvetica\", \"sans-serif\"; }
          
          .report-card-header td { padding: 20px 10px; }
          
          .header-left h2 { font-weight: 400; font-size: 30px; }
          
          .header-left p { margin: 5px 0; font-size: 15px; }
          
          .header-right { color: #040404; text-align: center; }
          
          .student-info-header { padding: 0px 30px 20px; }
          
          .student-info-header td { padding-bottom: 20px; vertical-align: top; }
          
          .student-info-header .info-left { padding-top: 20px; width: 100%; }
          
          .student-info-header .info-left h2 { font-size: 16px; margin-bottom: 8px; font-weight: 600; }
          
          .student-info-header .info-left .title { width: 150px; display: inline-block; }
          
          .student-info-header .info-left span:not(.title) { font-weight: 400; }
          
          .student-info-header .info-left p { margin-bottom: 10px; color: #333; }
          
          .student-info-header .info-right { padding-left: 10px; }
          
          .semester-table { padding: 0 30px 30px; vertical-align: top; }
          
          .semester-table table { border: 1px solid #000; }
          
          .semester-table th, .semester-table td { border-bottom: 1px solid #000; padding: 8px 15px; }
          
          .semester-table th { text-align: left; background-color: #e5e5e5; }
          
          .semester-table caption { margin-bottom: 10px; text-align: left; }
          
          .semester-table caption h2 { font-size: 18px; }
          
          .gpa-table { padding: 0 30px 30px; }
          
          .gpa-table table { border: 1px solid #000; }
          
          .gpa-table caption h4 { text-align: left; margin-bottom: 10px; font-weight: 500; }
          
          .gpa-table th { padding: 8px 15px; background-color: #e5e5e5; text-align: left; border-bottom: 1px solid #000; }
          
          .gpa-table td { padding: 8px 15px; text-align: left; }
          
          .signature-table { padding: 40px 30px; }
          
          .sign { padding-bottom: 20px; }
          
          .short-sign { padding-top: 60px; }
          
          .long-line { width: 90%; margin-bottom: 10px; border-top: 2px solid #000; }
          
          .small-line { display: inline-block; width: 150px; border-top: 2px solid #000; }
          
          .name { margin: 8px 0; }
          
          .text-uppercase { text-transform: uppercase; }
          
          .header-middle p { font-size: 22px; }
          
          .report-card-header td.header-right { vertical-align: top; padding-top: 58px; font-weight: 500; }
          
          .bevaior-table tr td:first-child { width: 20px; font-weight: 500; }
          
          .bevaior-table td { border-right: 1px solid #333; }
          
          .comments-table h2 { text-align: left; }
          
          .semester-table .comments-table caption { margin-bottom: 0; }
          
          .semester-table .comments-table { border: none; }
          
          .comments-table td { border-bottom: 1px dashed #b7b4b4; padding: 35px 0 0 } 

          .behavior-subtable th, .behavior-subtable td {
            border-right: 1px solid #000;
          }

          .behavior-subtable th:last-child, .behavior-subtable td:last-child {
            border-right: none;
          }
          
          </style>
        </head>
    <body onload="window.print()">${printContents}</body>
      </html>`
    );
    popupWin.document.close();
    document.getElementById('printSectionId').className = 'hidden';
    return;
    }

    // return new Promise((resolve, reject) => {
    //   const element = document.getElementById('pdfData');
    //   element.style.display = 'block';
    //   const opt = {
    //     margin: 1,
    //     image: { type: 'png', quality: 0.98 },
    //     html2canvas: { scale: 2 },
    //     jsPDF: { unit: 'mm', format: 'A4', orientation: 'landscape' },
    //   };
    //   html2pdf().from(element).set(opt).toContainer().then(() => {
    //     element.style.display = 'none';
    //   }).save().outputPdf('datauristring').then((res) => {
    //     this.pdfData.reportCardPdf = res;
    //     const dataWithTicket = {
    //       // order_id: this.orderDetails.order_id,
    //       // ticket_pdf: new File([this.dataURItoBlob(res)], `Ticket_${this.orderDetails.order_id}.pdf`, { type: 'application/pdf' })
    //     };
    //     resolve(dataWithTicket);
    //   });
    // });
  }

  generatePdfForDefault(){
    let printContents, popupWin;
    printContents = document.getElementById('printReportCardId').innerHTML;
    document.getElementById('printReportCardId').className = 'block';
    popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
    if(popupWin === null || typeof(popupWin)==='undefined'){
      document.getElementById('printReportCardId').className = 'hidden';
      this.snackbar.open("User needs to allow the popup from the browser", '', {
        duration: 10000
      });
    } else {
    popupWin.document.open();
    popupWin.document.write(`
      <html>
        <head>
          <title>Print tab</title>
          <style>
          *{
            font-family: \"Arial\", \"Helvetica\", \"sans-serif\";
          }

          body, h1, h2, h3, h4, h5, h6, p { margin: 0; }

          body { -webkit-print-color-adjust: exact; }
         
          table { border-collapse: collapse; width: 100%; }
         
          .float-left { float: left; }
         
          .float-right { float: right; }
         
          .text-center { text-align: center; }
         
          .text-right { text-align: right; }
         
          .ml-auto { margin-left: auto; }
         
          .m-auto { margin: auto; }
         
          .report-card { width: 900px; margin: auto; font-family: \"Arial\", \"Helvetica\", \"sans-serif\"; }
         
          .report-card-header { border-bottom: 2px solid #000; }
         
          .report-card-header td { padding: 25px 30px 20px; }
         
          .header-left h2 { font-weight: 400; font-size: 30px; }
         
          .header-left p { margin: 5px 0; font-size: 15px; }
         
          .header-right { background: #000; color: #fff; text-align: center; padding: 6px 10px; border-radius: 3px; }
         
          .student-info-header { padding: 20px 30px; }
         
          .student-info-header td { border-bottom: 1px solid #000; padding-bottom: 20px; }
         
          .student-info-header .info-left h1 { font-size: 28px; margin-bottom: 10px; }
         
          .striped-table td { border: 1px solid #000; padding: 8px; }
         
          .striped-table td:last-child { background-color: #E4E4E4; }
         
          .info-left span { padding: 5px 15px; margin-right: 15px; border-radius: 15px; background: #484747; color: #fff; }
         
          .semester-table { padding: 0 30px 30px; }
         
          .semester-table table { border: 1px solid #000; }
         
          .semester-table th, .semester-table td { border-bottom: 1px solid #000; padding: 8px 15px; }
         
          .semester-table th { text-align: left; font-weight: normal; background-color: #e5e5e5; }
         
          .semester-table caption { margin-bottom: 7px; }
         
          .semester-table caption h2 { font-weight: 400; }
         
          .semester-table caption p { margin-top: 8px; }
         
          .signature-table { padding: 30px; }
         
          .sign { padding-bottom: 20px; }
         
          .long-line { width: 90%; border-top: #000; margin-bottom: 10px; border-top: 2px solid #000; }
         
          .small-line { display: inline-block; width: 150px; border-top: 2px solid #000; }
         
          .comments { padding: 20px 30px 10px; }
         
          .comments h4 { margin-bottom: 10px; }
         
          .comments p { margin-bottom: 3px; padding-right: 20px; }

          .inline-block {display: inline-block}

          td.comments table td { vertical-align: top; }
          
    </style>
        </head>
    <body onload="window.print()">${printContents}</body>
      </html>`
    );
    popupWin.document.close();
    document.getElementById('printReportCardId').className = 'hidden';
    return;
    }
  }

  
}
