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

import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";
import icSearch from '@iconify/icons-ic/search';
import { GetAllGradeLevelsModel } from "../../../models/grade-level.model";
import { GradeLevelService } from "../../../services/grade-level.service";
import { MatSnackBar } from "@angular/material/snack-bar";
import { StudentListModel } from '../../../models/student.model';
import { StudentService } from "../../../services/student.service";
import { MatTableDataSource } from "@angular/material/table";
import { FormControl } from "@angular/forms";
import { MatPaginator, MatPaginatorIntl } from "@angular/material/paginator";
import { MatSort } from "@angular/material/sort";
import { debounceTime, distinctUntilChanged, switchMap, takeUntil } from "rxjs/operators";
import { stagger40ms } from '../../../../@vex/animations/stagger.animation';
import { LoaderService } from '../../../services/loader.service';
import { forkJoin, Subject } from "rxjs";
import { fadeInUp400ms } from "../../../../@vex/animations/fade-in-up.animation";
import { fadeInRight400ms } from "../../../../@vex/animations/fade-in-right.animation";
import { MatCheckbox } from "@angular/material/checkbox";
import { GetStudentTranscriptModel, StudentTranscript } from "../../../models/student-transcript.model";
import { StudentTranscriptService } from "../../../services/student-transcript.service";
import { map } from 'rxjs/operators';
import { Permissions } from "../../../models/roll-based-access.model";
import { PageRolesPermission } from "../../../common/page-roles-permissions.service";
import { CommonService } from "src/app/services/common.service";
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { HistoricalMarkingPeriodService } from "src/app/services/historical-marking-period.service";
import { HistoricalGradeAddViewModel } from "src/app/models/historical-marking-period.model";
import { AdvancedSearchExpansionModel } from "src/app/models/common.model";
import { SharedFunction } from "../../shared/shared-function";

@Component({
  selector: "vex-transcripts",
  templateUrl: "./transcripts.component.html",
  styleUrls: ["./transcripts.component.scss"],
  animations: [
    fadeInUp400ms,
    stagger40ms,
    fadeInRight400ms
  ]
})
export class TranscriptsComponent implements OnInit, OnDestroy {
  icSearch = icSearch;
  columns = [
    { label: '', property: 'selectedStudent', type: 'text', visible: true },
    { label: 'Name', property: 'firstGivenName', type: 'text', visible: true },
    { label: 'Student ID', property: 'studentInternalId', type: 'text', visible: true },
    { label: 'Alternate ID', property: 'alternateId', type: 'text', visible: true },
    { label: 'Grade Level', property: 'gradeLevelTitle', type: 'text', visible: true },
    { label: 'Section', property: 'sectionId', type: 'text', visible: true },
    { label: 'Telephone', property: 'homePhone', type: 'text', visible: true },
    { label: 'Status', property: 'status', type: 'text', visible: false },
  ];

  loading: boolean;
  destroySubject$: Subject<void> = new Subject();
  totalCount = 0;
  pageNumber: number;
  pageSize: number;
  searchValue;
  searchCount;
  searchCtrl: FormControl;
  getAllGradeLevels: GetAllGradeLevelsModel = new GetAllGradeLevelsModel();
  getAllStudent: StudentListModel = new StudentListModel();
  studentList: MatTableDataSource<any>;
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild('masterCheckBox') masterCheckBox: MatCheckbox;
  showAdvanceSearchPanel: boolean = false;
  toggleValues;
  listOfStudents = [];
  selectedStudents = []
  selectedGradeLevels = [];
  studentTranscipt = new StudentTranscript();
  getStudentTranscriptModel: GetStudentTranscriptModel = new GetStudentTranscriptModel();
  gradeLevelError: boolean;
  pdfByteArrayForTranscript: string;
  pdfGenerateLoader: boolean = false;
  permissions: Permissions;
  generatedTranscriptData;
  // historicalGradeAddViewModel: HistoricalGradeAddViewModel = new HistoricalGradeAddViewModel();
  advancedSearchExpansionModel: AdvancedSearchExpansionModel = new AdvancedSearchExpansionModel();
  // historicalGradeList;
  // selectedHistoricalGradeList=[];
  // historicalGradeError:boolean;
  isFromAdvancedSearch: boolean = false;
  filterParameters = [];
  constructor(
    public translateService: TranslateService,
    private gradeLevelService: GradeLevelService,
    private snackbar: MatSnackBar,
    private studentService: StudentService,
    private loaderService: LoaderService,
    private transcriptService: StudentTranscriptService,
    private pageRolePermissions: PageRolesPermission,
    private el: ElementRef,
    private commonService: CommonService,
    private defaultValuesService: DefaultValuesService,
    private paginatorObj: MatPaginatorIntl,
    private historicalMarkingPeriodService:HistoricalMarkingPeriodService,
    private commonFunction: SharedFunction
  ) {
    this.advancedSearchExpansionModel.accessInformation = false;
    this.advancedSearchExpansionModel.enrollmentInformation = true;
    this.advancedSearchExpansionModel.searchAllSchools = false;
    paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
    // translateService.use("en");
    this.getAllStudent.filterParams = null;
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
  }

  ngOnInit(): void {
    this.getAllStudent.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    this.searchCtrl = new FormControl();
    this.callAllStudent();
    this.getAllGradeLevel();
    this.permissions = this.pageRolePermissions.checkPageRolePermission();
    // this.getAllHistoricalGradeList()
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  ngAfterViewInit() {

    //  Sorting
    this.getAllStudent = new StudentListModel();
    this.sort.sortChange.subscribe((res) => {
      this.getAllStudent.pageNumber = this.pageNumber
      this.getAllStudent.pageSize = this.pageSize;
      this.getAllStudent.sortingModel.sortColumn = res.active;
      if (this.searchCtrl.value) {
        let filterParams = [
          {
            columnName: null,
            filterValue: this.searchCtrl.value,
            filterOption: 3
          }
        ]
        Object.assign(this.getAllStudent, { filterParams: filterParams });
      }
      if (res.direction == "") {
        this.getAllStudent.sortingModel = null;
        this.callAllStudent();
        this.getAllStudent = new StudentListModel();
        this.getAllStudent.sortingModel = null;
      } else {
        this.getAllStudent.sortingModel.sortDirection = res.direction;
        this.callAllStudent();
      }
    });
    //  Searching
    this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term) {
        this.callWithSearchParams(term)
      }
      else {
        this.callWithoutSearchParams()
      }
    })
  }
  // getAllHistoricalGradeList() {
  //   this.historicalGradeAddViewModel.schoolId=this.defaultValuesService.getSchoolID();
  //   this.historicalGradeAddViewModel.historicalGradeList=[];
  //   this.historicalMarkingPeriodService.getAllHistoricalGradeList(this.historicalGradeAddViewModel).subscribe((res:any) => {
  //     if (res._failure) {
  //       this.commonService.checkTokenValidOrNot(res._message);
  //       if (!res.gradeEquivalencies) {
  //         this.snackbar.open(res._message, '', {
  //           duration: 10000
  //         });
  //       }
  //     }
  //     else {
  //       this.historicalGradeList = res.gradeEquivalencies;
  //     }
  //   })
  // }
  // onHistoricalGradeLChange(event, equivalencyId) {
  //   if (event.checked) {
  //     this.selectedHistoricalGradeList.push(equivalencyId);
  //   } else {
  //     this.selectedHistoricalGradeList = this.selectedHistoricalGradeList.filter(item => item !== equivalencyId);
  //   }
  //   this.selectedHistoricalGradeList?.length > 0 ? this.historicalGradeError = false : this.historicalGradeError = true;

  // }

  callWithSearchParams(term) {
    let filterParams = [
      {
        columnName: null,
        filterValue: term,
        filterOption: 3
      }
    ]
    if (this.sort.active && this.sort.direction) {
      this.getAllStudent.sortingModel.sortColumn = this.sort.active;
      this.getAllStudent.sortingModel.sortDirection = this.sort.direction;
    }
    Object.assign(this.getAllStudent, { filterParams: filterParams });
    this.getAllStudent.pageNumber = 1;
    this.paginator.pageIndex = 0;
    this.getAllStudent.pageSize = this.pageSize;
    this.callAllStudent();
  }
  callWithoutSearchParams() {
    Object.assign(this.getAllStudent, { filterParams: null });
    this.getAllStudent.pageNumber = this.paginator.pageIndex + 1;
    this.getAllStudent.pageSize = this.pageSize;
    if (this.sort.active != undefined && this.sort.direction != "") {
      this.getAllStudent.sortingModel.sortColumn = this.sort.active;
      this.getAllStudent.sortingModel.sortDirection = this.sort.direction;
    }
    this.callAllStudent();
  }

  resetStudentList() {
    this.getAllStudent = new StudentListModel();
    this.getAllStudent.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    this.searchCount = null;
    this.searchValue = null;
    this.callAllStudent();
  }

  onGradeLevelChange(event, gradeId) {
    if (event.checked) {
      this.selectedGradeLevels.push(gradeId);
    } else {
      this.selectedGradeLevels = this.selectedGradeLevels.filter(item => item !== gradeId);
    }
    this.selectedGradeLevels?.length > 0 ? this.gradeLevelError = false : this.gradeLevelError = true;

  }

  getPageEvent(event) {
    if (this.sort.active && this.sort.direction) {
      this.getAllStudent.sortingModel.sortColumn = this.sort.active;
      this.getAllStudent.sortingModel.sortDirection = this.sort.direction;
    }
    if (this.searchCtrl.value) {
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
    this.getAllStudent.filterParams = this.filterParameters;
    this.callAllStudent();
  }

  callAllStudent() {
    if (this.getAllStudent.sortingModel?.sortColumn == "") {
      this.getAllStudent.sortingModel = null;
    }
    this.studentService.GetAllStudentList(this.getAllStudent).subscribe(res => {
    if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        if (res.studentListViews === null) {
          this.totalCount = this.isFromAdvancedSearch ? 0 : null;
          this.searchCount = this.isFromAdvancedSearch ? 0 : null;
          this.studentList = new MatTableDataSource([]);
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
          this.isFromAdvancedSearch = false;
        } else {
          this.studentList = new MatTableDataSource([]);
          this.totalCount = this.isFromAdvancedSearch ? 0 : null;
          this.searchCount = this.isFromAdvancedSearch ? 0 : null;
          this.isFromAdvancedSearch = false;
        }
      } else {
        this.totalCount = res.totalCount;
        this.searchCount = res.totalCount;
        this.pageNumber = res.pageNumber;
        this.pageSize = res._pageSize;
        res.studentListViews.forEach((student) => {
          student.checked = false;
        });
        this.listOfStudents = res.studentListViews.map((item) => {
          this.selectedStudents.map((selectedUser) => {
            if (item.studentId == selectedUser.studentId) {
              item.checked = true;
              return item;
            }
          });
          return item;
        });

        this.masterCheckBox.checked = this.listOfStudents.every((item) => {
          return item.checked;
        })
        this.studentList = new MatTableDataSource(res.studentListViews);
        this.getAllStudent = new StudentListModel();
        this.isFromAdvancedSearch = false;
      }
    });
  }

  /* This is for get all data from the Advanced Search component and then call the API in this page 
  NOTE: We just get the filterParams Array from Search component
  */
  filterData(res) {
    this.filterParameters = res.filterParams;
    this.isFromAdvancedSearch = true;
    this.getAllStudent = new StudentListModel();
    this.getAllStudent.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    if (res) {
      this.getAllStudent.filterParams = res.filterParams;
      this.getAllStudent.includeInactive = res.inactiveStudents;
      this.getAllStudent.searchAllSchool = res.searchAllSchool;
      this.getAllStudent.dobStartDate = this.commonFunction.formatDateSaveWithoutTime(res.dobStartDate);
      this.getAllStudent.dobEndDate = this.commonFunction.formatDateSaveWithoutTime(res.dobEndDate);
      this.defaultValuesService.sendIncludeInactiveFlag(res.inactiveStudents);
      this.defaultValuesService.sendAllSchoolFlag(res.searchAllSchool);
      this.callAllStudent();
    }
  }

  getSearchResult(res) {
    this.getAllStudent = new StudentListModel();
    if (res?.totalCount){
      this.searchCount = res.totalCount;
      this.totalCount = res.totalCount;
    }
    else{
      this.searchCount = 0;
      this.totalCount = 0;
    }
    this.pageNumber = res.pageNumber;
    this.pageSize = res.pageSize;
    if (res && res.studentListViews) {
      res?.studentListViews?.forEach((student) => {
        student.checked = false;
      });
      this.listOfStudents = res.studentListViews.map((item) => {
        this.selectedStudents.map((selectedUser) => {
          if (item.studentId == selectedUser.studentId) {
            item.checked = true;
            return item;
          }
        });
        return item;
      });

      this.masterCheckBox.checked = this.listOfStudents.every((item) => {
        return item.checked;
      })
    }
    this.studentList = new MatTableDataSource(res?.studentListViews);
    this.getAllStudent = new StudentListModel();
  }

  getToggleValues(event) {
    this.toggleValues = event;
    if (event.inactiveStudents === true) {
      this.columns[7].visible = true;
    }
    else if (event.inactiveStudents === false) {
      this.columns[7].visible = false;
    }
  }

  hideAdvanceSearch(event) {
    this.showAdvanceSearchPanel = false;
  }

  getSearchInput(event) {
    this.searchValue = event;
  }

  getAllGradeLevel() {
    this.gradeLevelService.getAllGradeLevels(this.getAllGradeLevels).pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      if (res) {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          if (res.tableGradelevelList === null) {
            this.getAllGradeLevels.tableGradelevelList = [];
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          } else {
            this.getAllGradeLevels.tableGradelevelList = res.tableGradelevelList;
          }
        }
        else {
          this.getAllGradeLevels.tableGradelevelList = res.tableGradelevelList;
        }
      } else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    })
  }

  someComplete(): boolean {
    let indetermine = false;
    for (let user of this.listOfStudents) {
      for (let selectedUser of this.selectedStudents) {
        if (user.studentId == selectedUser.studentId) {
          indetermine = true;
        }
      }
    }
    if (indetermine) {
      this.masterCheckBox.checked = this.listOfStudents.every((item) => {
        return item.checked;
      })
      if (this.masterCheckBox.checked) {
        return false;
      } else {
        return true;
      }
    }

  }

  setAll(event) {
    this.listOfStudents.forEach(user => { user.checked = event; });
    this.studentList = new MatTableDataSource(this.listOfStudents);
    this.decideCheckUncheck();
  }

  onChangeSelection(eventStatus: boolean, id) {
    for (let item of this.listOfStudents) {
      if (item.studentId == id) {
        item.checked = eventStatus;
        break;
      }
    }
    this.studentList = new MatTableDataSource(this.listOfStudents);
    this.masterCheckBox.checked = this.listOfStudents.every((item) => {
      return item.checked;
    });

    this.decideCheckUncheck();
  }

  decideCheckUncheck() {
    this.listOfStudents.map((item) => {
      let isIdIncludesInSelectedList = false;
      if (item.checked) {
        for (let selectedUser of this.selectedStudents) {
          if (item.studentId == selectedUser.studentId) {
            isIdIncludesInSelectedList = true;
            break;
          }
        }
        if (!isIdIncludesInSelectedList) {
          this.selectedStudents.push(item);
        }
      } else {
        for (let selectedUser of this.selectedStudents) {
          if (item.studentId == selectedUser.studentId) {
            this.selectedStudents = this.selectedStudents.filter((user) => user.studentId != item.studentId);
            break;
          }
        }
      }
      isIdIncludesInSelectedList = false;

    });
    this.selectedStudents = this.selectedStudents.filter((item) => item.checked);
  }

  generateTranscript() {
    if (!this.selectedGradeLevels?.length) {
      this.gradeLevelError = true;
      const invalidGradeLevel: HTMLElement = this.el.nativeElement.querySelector(
        '.custom-scroll'
      );
      invalidGradeLevel.scrollIntoView({ behavior: 'smooth',block: 'center' });
      return;
    } else {
      this.gradeLevelError = false;
    }
    // if (!this.selectedHistoricalGradeList?.length) {
    //   this.historicalGradeError = true;
    //   const invalidGradeLevel: HTMLElement = this.el.nativeElement.querySelector(
    //     '.custom-scroll'
    //   );
    //   invalidGradeLevel.scrollIntoView({ behavior: 'smooth',block: 'center' });
    //   return;
    // } else {
    //   this.historicalGradeError = false;
    // }
    if (!this.selectedStudents?.length) {
      this.snackbar.open('Select at least one student.', '', {
        duration: 3000
      });
      return
    }
    this.fillUpNecessaryValues();
    this.fetchTranscript();
  }

  fillUpNecessaryValues() {
    this.getStudentTranscriptModel.gradeLavels = this.selectedGradeLevels.toString();
    this.getStudentTranscriptModel.studentsDetailsForTranscripts = [];
    this.selectedStudents?.map((item) => {
      this.getStudentTranscriptModel.studentsDetailsForTranscripts.push({
        studentId: item.studentId
        // studentGuid: item.studentGuid,
        // firstGivenName: item.firstGivenName,
        // middleName: item.middleName,
        // lastFamilyName: item.lastFamilyName,
      })
    });
  }

  getTranscriptForStudents() {
    return new Promise((resolve, reject) => {
      // this.getStudentTranscriptModel.HistoricalGradeLavels=this.selectedHistoricalGradeList.toString()
      this.transcriptService.getTranscriptForStudents(this.getStudentTranscriptModel).subscribe(res => {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          this.snackbar.open(res._message, '', {
            duration: 1000
          });
          this.pdfGenerateLoader = false;
        } else {
          resolve(res);
          this.pdfGenerateLoader = false;
        }
      })
    });
  }
  
  
  fetchTranscript() {
    this.pdfGenerateLoader = true;
    this.getTranscriptForStudents().then((res: any) => {
      this.generatedTranscriptData = res;
      setTimeout(() => {
        this.generatePDF();
      }, 100 * this.generatedTranscriptData?.studentsDetailsForTranscripts.length);
    });
  }

  // fetchTranscript() {
  //   this.pdfGenerateLoader = true;
  //   this.transcriptService.addTranscriptForStudent(this.studentTranscipt).pipe(
  //     takeUntil(this.destroySubject$),
  //     switchMap((dataAfterAddingStudentRecords) => {
  //       let generateTranscriptObservable$
  //       if (!dataAfterAddingStudentRecords._failure) {
  //         generateTranscriptObservable$ = this.transcriptService.generateTranscriptForStudent(this.studentTranscipt)
  //       } else {
  //         this.snackbar.open(dataAfterAddingStudentRecords._message, '', {
  //           duration: 3000
  //         });
  //       }
  //       return forkJoin(generateTranscriptObservable$);
  //     })
  //   ).subscribe((res: any) => {
  //     this.pdfGenerateLoader = false;
  //     let response = res[0]
  //     if (response._failure) { this.commonService.checkTokenValidOrNot(response._message);


  //       this.snackbar.open(response._message, '', {
  //         duration: 3000
  //       });
  //     } else {
  //       this.pdfByteArrayForTranscript = response.transcriptPdf;
  //     }
  //   });
  // }

  backToList() {
    this.pdfByteArrayForTranscript = null
  }

  generatePDF() {
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
          h1,
  h2,
  h3,
  h4,
  h5,
  h6,
  p {
    margin: 0;
  }
  body {
    -webkit-print-color-adjust: exact;
  }
  table {
    border-collapse: collapse;
    width: 100%;
    font-family: arial;
  }
  .float-left {
    float: left;
  }
  .float-right {
    float: right;
  }
  .text-center {
    text-align: center;
  }
  .text-right {
    text-align: right;
  }
  .ml-auto {
    margin-left: auto;
  }
  .m-auto {
    margin: auto;
  }
  .report-card {
    width: 900px;
    margin: auto;
    font-family: "Roboto", "Helvetica Neue";
  }
  .report-card-header td {
    padding: 15px 10px;
  }
  .header-left h2 {
    font-weight: 400;
    font-size: 30px;
  }
  .header-left p {
    margin: 5px 0;
    font-size: 15px;
  }
  .header-right {
    color: #040404;
    text-align: center;
  }
  .student-info-header {
    padding: 20px 10px;
  }
  .student-info-header td {
    padding-bottom: 20px;
    vertical-align: top;
  }
  .student-info-header .info-left {
    padding-top: 10px;
    width: calc(100% - 150px);
  }
  .student-info-header .info-left h2 {
    font-size: 20px;
    margin-bottom: 8px;
    font-weight: 600;
  }
  .student-info-header .info-left .title {
    width: 150px;
    display: inline-block;
  }
  .student-info-header .info-left span:not(.title) {
    font-weight: 400;
  }
  .student-info-header .info-left p {
    margin-bottom: 10px;
    color: #333;
  }
  .student-info-header .info-right {
    border: 2px solid #000;
  }
  .semester-table {
    padding: 0 10px 30px;
    vertical-align: top;
  }
  .semester-table th,
  .semester-table td:not(.semester-subtable) {
    border-bottom: 1px solid #000;
    padding: 8px 15px;
  }
  .semester-table th {
    text-align: left;
    background-color: #e5e5e5;
  }
  .semester-table caption {
    margin-bottom: 10px;
    text-align: left;
  }
  
  .semester-table caption h2 {
    font-weight: 400;
    font-size: 24px;
  }
  .semester-table .semester-subtable caption {
    margin-bottom: 8px;
    text-align: left;
  }
  .semester-table .semester-subtable {
    margin-bottom: 20px;
    margin-top: 10px;
  }
  .semester-table .semester-subtable thead, .semester-table .semester-subtable tbody {
    border: 1px solid #000;
  }
  .gpa-table {
    padding: 0 10px 30px;
  }
  .gpa-table table {
    border: 1px solid #000;
  }
  .gpa-table caption h4 {
    text-align: left;
    margin-bottom: 10px;
    font-weight: 500;
  }
  .gpa-table th {
    padding: 8px 15px;
    background-color: #e5e5e5;
    text-align: left;
    border-bottom: 1px solid #000;
  }
  .gpa-table td {
    padding: 8px 15px;
    text-align: left;
  }
  .signature-table {
    padding: 40px 30px;
  }
  .sign {
    padding-bottom: 20px;
  }
  .short-sign {
    padding-top: 60px;
  }
  .long-line {
    width: 90%;
    margin-bottom: 10px;
    border-top: 2px solid #000;
  }
  .small-line {
    display: inline-block;
    width: 150px;
    border-top: 2px solid #000;
  }
  .name {
    margin: 8px 0;
  }
  .text-uppercase {
    text-transform: uppercase;
  }
  .header-middle p {
    font-size: 22px;
  }
  .report-card-header td.header-right {
    vertical-align: top;
    padding-top: 58px;
    font-weight: 500;
  }
  .bevaior-table tr td:first-child {
    width: 20px;
    font-weight: 500;
  }
  .bevaior-table td {
    border-right: 1px solid #333;
  }
  .comments-table h2 {
    text-align: left;
  }
  .semester-table .comments-table caption {
    margin-bottom: 0;
  }
  .semester-table .comments-table {
    border: none;
  }
  .comments-table td {
    border-bottom: 1px dashed #b7b4b4;
    padding: 35px 0 0;
  }
  .report-card-header {
    padding: 20px 0;
    border-bottom: 2px solid #000;
  }

  .logo {
    width: 80px;
    height: 80px;
    border-radius: 50%;
    border: 2px solid #cacaca;
    margin-right: 20px;
    text-align: center;
    overflow: hidden;
  }

  .report-card-header .header-left {
    width: 70%;
  }

  .report-card-header .information {
    width: calc(100% - 110px);
    margin-top: 10px
  }

  .header-right {
    background: #000;
    color: #fff;
    text-align: center;
    padding: 12px 20px;
    border-radius: 3px;
  }

  .student-photo {
    margin-right: 20px;
    border: 1px solid #000;
    border-radius: 3px;
    width: 120px;
    height: 120px;
  }

  .student-photo img {
    width: 100%;
  }

  .student-info-header .info-right td {
    text-align: right;
    padding: 12px 10px;
    color: #333;
  }

  .student-info-header .info-right td:last-child {
    background-color: #040404;
    color: #e2e2e2;
  }

  .logo img {
    width: 100%;
    overflow: hidden;
  }
  .semester-table .semester-subtable table {
    margin-bottom: 20px;
  }
  .word-break {
    word-break: break-word;
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
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}
