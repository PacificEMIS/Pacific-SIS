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
import { MatSnackBar } from "@angular/material/snack-bar";
import { MatTableDataSource } from "@angular/material/table";
import { FormControl } from "@angular/forms";
import { MatPaginator } from "@angular/material/paginator";
import { MatSort } from "@angular/material/sort";
import { debounceTime, distinctUntilChanged, switchMap, takeUntil } from "rxjs/operators";
import { forkJoin, Subject } from "rxjs";
import { MatCheckbox } from "@angular/material/checkbox";
import { fadeInUp400ms } from "src/@vex/animations/fade-in-up.animation";
import { stagger40ms } from "src/@vex/animations/stagger.animation";
import { fadeInRight400ms } from "src/@vex/animations/fade-in-right.animation";
import { GradeLevelService } from "src/app/services/grade-level.service";
import { StudentService } from "src/app/services/student.service";
import { LoaderService } from "src/app/services/loader.service";
import { StudentTranscriptService } from "src/app/services/student-transcript.service";
import { GetAllGradeLevelsModel } from "src/app/models/grade-level.model";
import { StudentListModel } from "src/app/models/student.model";
import { GetStudentTranscriptModel, StudentTranscript } from "src/app/models/student-transcript.model";
import { Router } from "@angular/router";
import { Permissions } from "../../../../models/roll-based-access.model";
import { PageRolesPermission } from "../../../../common/page-roles-permissions.service";
import { CommonService } from "src/app/services/common.service";
import { DefaultValuesService } from '../../../../common/default-values.service';
import { HistoricalGradeAddViewModel } from "src/app/models/historical-marking-period.model";
import { HistoricalMarkingPeriodService } from "src/app/services/historical-marking-period.service";

@Component({
  selector: "vex-student-transcript",
  templateUrl: "./student-transcript.component.html",
  styleUrls: ["./student-transcript.component.scss"],
  animations: [
    fadeInUp400ms,
    stagger40ms,
    fadeInRight400ms
  ]
})
export class StudentTranscriptComponent implements OnInit {
  icSearch = icSearch;

  loading: boolean;
  destroySubject$: Subject<void> = new Subject();
  constructor(
    public translateService: TranslateService,
    private gradeLevelService: GradeLevelService,
    private snackbar: MatSnackBar,
    private studentService: StudentService,
    private loaderService: LoaderService,
    private transcriptService: StudentTranscriptService,
    private el: ElementRef,
    private pageRolePermissions: PageRolesPermission,
    private router: Router,
    private commonService: CommonService,
    private historicalMarkingPeriodService:HistoricalMarkingPeriodService,
    private defaultValuesService: DefaultValuesService
  ) {
    // translateService.use("en");
    this.getAllStudent.filterParams = null;
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
    this.defaultValuesService.checkAcademicYear() && !this.studentService.getStudentId() ? this.studentService.redirectToGeneralInfo() : !this.defaultValuesService.checkAcademicYear() && !this.studentService.getStudentId() ? this.studentService.redirectToStudentList() : '';
  }
  totalCount = 0;
  pageNumber: number;
  pageSize: number;
  searchValue;
  searchCount;
  searchCtrl: FormControl;
  getAllGradeLevels: GetAllGradeLevelsModel = new GetAllGradeLevelsModel();
  getAllStudent: StudentListModel = new StudentListModel();
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
  // historicalGradeList;
  // selectedHistoricalGradeList=[];
  // historicalGradeError:boolean;
  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission();
    this.getAllGradeLevel();
    this.studentService.studentDetailsForViewedAndEdited.subscribe((res)=>{
      this.selectedStudents.push(res.studentMaster);
    });

    this.studentService.studentCreatedMode.subscribe((res)=>{
      if(!res) {
        this.router.navigate(['/school', 'students', 'student-generalinfo']);
      }
    })
    // this.getAllHistoricalGradeList()
  }


  onGradeLevelChange(event, gradeId) {
    if (event.checked) {
      this.selectedGradeLevels.push(gradeId);
    } else {
      this.selectedGradeLevels = this.selectedGradeLevels.filter(item => item !== gradeId);
    }
    this.selectedGradeLevels?.length > 0 ? this.gradeLevelError = false : this.gradeLevelError = true;
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



  generateTranscript() {
    if (!this.selectedGradeLevels?.length) {
      this.gradeLevelError = true;
      // const invalidGradeLevel: HTMLElement = this.el.nativeElement.querySelector(
      //   '.custom-scroll'
      // );
      // invalidGradeLevel.scrollIntoView({ behavior: 'smooth',block: 'center' });
    } else {
      this.gradeLevelError = false;
    }
    // if (!this.selectedHistoricalGradeList?.length) {
    //   this.historicalGradeError = true;
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
    // this.getStudentTranscriptModel.HistoricalGradeLavels=this.selectedHistoricalGradeList.toString();
    return new Promise((resolve, reject) => {
      this.transcriptService.getTranscriptForStudents(this.getStudentTranscriptModel).subscribe(res => {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          this.snackbar.open(res._message, '', {
            duration: 1000
          });
          // this.pdfGenerateLoader = false;
        } else {
          resolve(res);
          // this.pdfGenerateLoader = false;
        }
      })
    });
  }

  fetchTranscript() {
    // this.pdfGenerateLoader = true;
    this.getTranscriptForStudents().then((res: any) => {
      this.generatedTranscriptData = res;
      setTimeout(() => {
        this.generatePDF();
      }, 100 * this.generatedTranscriptData?.studentsDetailsForTranscripts.length);
    });
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
