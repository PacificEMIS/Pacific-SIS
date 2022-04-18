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

import { Component, OnInit, Input } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { fadeInUp400ms } from '../../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../../@vex/animations/stagger.animation';
import { fadeInRight400ms } from '../../../../../@vex/animations/fade-in-right.animation';
import { TranslateService } from '@ngx-translate/core';
import icEdit from '@iconify/icons-ic/twotone-edit';
import icDelete from '@iconify/icons-ic/twotone-delete';
import icAdd from '@iconify/icons-ic/baseline-add';
import icPrint from '@iconify/icons-ic/baseline-print';
import icComment from '@iconify/icons-ic/twotone-comment';
import { MatDialog } from '@angular/material/dialog';
import { EditCommentComponent } from './edit-comment/edit-comment.component';
import {StudentService} from '../../../../services/student.service';
import {ExcelService} from '../../../../services/excel.service';
import {StudentCommentsListViewModel, StudentCommentsAddView} from '../../../../models/student-comments.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ConfirmDialogComponent } from '../../../../pages/shared-module/confirm-dialog/confirm-dialog.component';
import { SchoolCreate } from '../../../../enums/school-create.enum';
import { SharedFunction } from '../../../../pages/shared/shared-function';
import { DatePipe } from '@angular/common';
import { Permissions, RolePermissionListViewModel, RolePermissionViewModel } from '../../../../models/roll-based-access.model';
import { CryptoService } from '../../../../services/Crypto.service';
import { DefaultValuesService } from '../../../../common/default-values.service';
import { PageRolesPermission } from '../../../../common/page-roles-permissions.service';
import { CommonService } from 'src/app/services/common.service';
import { GetStudentProgressReportModel } from 'src/app/models/report.model';
import { ProfilesTypes } from 'src/app/enums/profiles.enum';
import { ReportService } from 'src/app/services/report.service';


@Component({
  selector: 'vex-student-comments',
  templateUrl: './student-comments.component.html',
  styleUrls: ['./student-comments.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms,
    fadeInRight400ms
  ],
  providers: [DatePipe]
})
export class StudentCommentsComponent implements OnInit {

  icEdit = icEdit;
  icDelete = icDelete;
  icAdd = icAdd;
  icComment = icComment;
  icPrint = icPrint;
  listCount;
  StudentCreate = SchoolCreate;
  studentCreateMode: SchoolCreate;
  studentDetailsForViewAndEdit;
  membershipType;
  studentCommentsListViewModel: StudentCommentsListViewModel = new StudentCommentsListViewModel();
  studentCommentsAddView: StudentCommentsAddView = new StudentCommentsAddView();
  permissions: Permissions;
  getStudentProgressReportModel: GetStudentProgressReportModel = new GetStudentProgressReportModel();
  profiles = ProfilesTypes;
  schoolDetails: any;
  today: Date = new Date();
  generatedReportCardData: any;
  defaultStudentPhoto = "/9j/4QAYRXhpZgAASUkqAAgAAAAAAAAAAAAAAP/sABFEdWNreQABAAQAAABkAAD/4QMtaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wLwA8P3hwYWNrZXQgYmVnaW49Iu+7vyIgaWQ9Ilc1TTBNcENlaGlIenJlU3pOVGN6a2M5ZCI/PiA8eDp4bXBtZXRhIHhtbG5zOng9ImFkb2JlOm5zOm1ldGEvIiB4OnhtcHRrPSJBZG9iZSBYTVAgQ29yZSA3LjAtYzAwMCA3OS5kYWJhY2JiLCAyMDIxLzA0LzE0LTAwOjM5OjQ0ICAgICAgICAiPiA8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPiA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtbG5zOnhtcE1NPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvbW0vIiB4bWxuczpzdFJlZj0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL3NUeXBlL1Jlc291cmNlUmVmIyIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgMjIuNSAoV2luZG93cykiIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6QUE3MTBDNDQwNDk2MTFFQzg4Q0Y5N0JCOEU0Q0FGNTkiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6QUE3MTBDNDUwNDk2MTFFQzg4Q0Y5N0JCOEU0Q0FGNTkiPiA8eG1wTU06RGVyaXZlZEZyb20gc3RSZWY6aW5zdGFuY2VJRD0ieG1wLmlpZDpBQTcxMEM0MjA0OTYxMUVDODhDRjk3QkI4RTRDQUY1OSIgc3RSZWY6ZG9jdW1lbnRJRD0ieG1wLmRpZDpBQTcxMEM0MzA0OTYxMUVDODhDRjk3QkI4RTRDQUY1OSIvPiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gPD94cGFja2V0IGVuZD0iciI/Pv/uAA5BZG9iZQBkwAAAAAH/2wCEAAEBAQEBAQEBAQEBAQEBAQEB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";
  constructor(
    private fb: FormBuilder,
    private dialog: MatDialog,
    public translateService: TranslateService,
    private snackbar: MatSnackBar,
    private studentService: StudentService,
    private commonFunction: SharedFunction,
    private excelService: ExcelService,
    private datePipe: DatePipe,
    public defaultValuesService: DefaultValuesService,
    private pageRolePermissions: PageRolesPermission,
    private cryptoService: CryptoService,
    private commonService: CommonService,
    private reportService: ReportService,
    ) {
    //translateService.use('en');
    this.defaultValuesService.checkAcademicYear() && !this.studentService.getStudentId() ? this.studentService.redirectToGeneralInfo() : !this.defaultValuesService.checkAcademicYear() && !this.studentService.getStudentId() ? this.studentService.redirectToStudentList() : '';
  }

  ngOnInit(): void {
    this.membershipType = this.defaultValuesService.getUserMembershipType();
    this.studentService.studentCreatedMode.subscribe((res)=>{
      this.studentCreateMode = res;
    })
    this.studentService.studentDetailsForViewedAndEdited.subscribe((res)=>{
      this.studentDetailsForViewAndEdit = res;
    })
    this.permissions = this.pageRolePermissions.checkPageRolePermission();
    this.getAllComments();
    this.getSchoolDetails();
  }

  openAddNew() {
    this.dialog.open(EditCommentComponent, {
      data: {studentId: this.studentDetailsForViewAndEdit.studentMaster.studentId},
      width: '800px'
    }).afterClosed().subscribe((data) => {
      if (data === 'submited'){
        this.getAllComments();
      }
    });
  }
  getAllComments(){
    this.studentCommentsListViewModel.studentId = this.studentDetailsForViewAndEdit.studentMaster.studentId;
    this.studentService.getAllStudentCommentsList(this.studentCommentsListViewModel).subscribe(
      (res: StudentCommentsListViewModel) => {
        if (res){
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.listCount = 0;
            this.studentCommentsListViewModel.studentCommentsList = [] ;
            if (!res.studentCommentsList){
              this.snackbar.open( res._message, '', {
                duration: 10000
              });
            }
        }
        else {
          this.studentCommentsListViewModel.studentCommentsList = res.studentCommentsList;
          this.listCount = res.studentCommentsList.length;
          this.studentCommentsListViewModel.studentCommentsList.map( n => {
            n.updatedOn = this.commonFunction.serverToLocalDateAndTime(n.updatedOn);
            n.createdOn = this.commonFunction.serverToLocalDateAndTime(n.createdOn);
          });
        }
        }else{
          this.snackbar.open(this.defaultValuesService.translateKey('studentCommentsNotFound') +
           this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      }
    );
  }

  exportCommentsToExcel(){
    if (this.studentCommentsListViewModel.studentCommentsList?.length > 0 || this.studentCommentsListViewModel.studentCommentsList!=null){
      const commentList = this.studentCommentsListViewModel.studentCommentsList?.map((item) => {
        return{
                   Comment: this.stripHtml(item.comment),
                   UpdatedBy: item.updatedBy,
                   UpdatedOn: this.datePipe.transform(item.updatedOn, 'MMM d, y, h:mm a')
        };
      });
      this.excelService.exportAsExcelFile(commentList, 'Comments_');
     }else{
       this.snackbar.open(this.defaultValuesService.translateKey('noRecordsFoundFailedtoExportComments'), '', {
         duration: 5000
       });
     }
  }

  stripHtml(html){
    // Create a new div element
    const temporalDivElement = document.createElement('div');
    // Set the HTML content with the providen
    temporalDivElement.innerHTML = html;
    // Retrieve the text property of the element (cross-browser support)
    return temporalDivElement.textContent || temporalDivElement.innerText || '';
}

  confirmDelete(element){
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: {
          title: this.defaultValuesService.translateKey('areYouSure'),
          message: this.defaultValuesService.translateKey('youAreAboutToDeleteThisComment') + '.'}
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult){
        this.deleteStudentComment(element);
      }
   });
  }
  deleteStudentComment(element){
    this.studentCommentsAddView.studentComments = element;
    this.studentService.deleteStudentComment(this.studentCommentsAddView).subscribe(
      (res: StudentCommentsAddView) => {
        if (res){
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open( res._message, '', {
              duration: 10000
            });
          }
          else {
            this.getAllComments();
          }
        }
        else{
          this.snackbar.open( this.defaultValuesService.translateKey('studentCommentsNotFound') + 
          this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      }
    );
  }
  editComment(element){
    this.dialog.open(EditCommentComponent, {
      data: {studentId: this.studentDetailsForViewAndEdit.studentMaster.studentId, information: element},
      width: '800px'
    }).afterClosed().subscribe((data) => {
      if (data === 'submited'){
        this.getAllComments();
      }
    });
  }

  getSchoolDetails() {
    this.getSchoolDetailsForStudentReport().then((res: any) => {
      this.generatedReportCardData = res.schoolMasterListData[0].studentMasterListData[0];
      this.schoolDetails = res.schoolMasterListData[0];
    });
  }

  getSchoolDetailsForStudentReport() {
    if(this.defaultValuesService.getUserMembershipType() === this.profiles.SuperAdmin || this.defaultValuesService.getUserMembershipType() === this.profiles.SchoolAdmin || this.defaultValuesService.getUserMembershipType() === this.profiles.AdminAssitant) {
      this.getStudentProgressReportModel.studentGuids = [this.studentService.getStudentGuid()]
    } else {
      this.getStudentProgressReportModel.studentGuids = [this.defaultValuesService.getUserGuidId()]
    }
    return new Promise((resolve, reject) => {
      this.reportService.getStudentProgressReport(this.getStudentProgressReportModel).subscribe((res) => {
        if (res._failure) {
          
          this.snackbar.open(res._message, '', {
            duration: 1000
          });
        } else {

          resolve(res);
        }
      })
    });
  }

  generatePdf() {
    setTimeout(() => {
        this.generatePdfForStudentComments();
      }, 500);
  }

  generatePdfForStudentComments() {
    let printContents, popupWin;
    printContents = document.getElementById('printComments').innerHTML;
    document.getElementById('printComments').className = 'block';
    popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
    if(popupWin === null || typeof(popupWin)==='undefined'){
      document.getElementById('printComments').className = 'hidden';
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
            font-family: Arial;
            background-color: #fff;
            margin: 0;
          }
          table {
            border-collapse: collapse;
            width: 100%;
          }
          .student-information-report {
              width: 1024px;
              margin: auto;
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
          .text-left {
              text-align: left;
          }
          .ml-auto {
            margin-left: auto;
          }
          .m-auto {
            margin: auto;
          }
          .inline-block {
              display: inline-block;
          }
          .border-table {
              border: 1px solid #000;
          }
          .clearfix::after {
              display: block;
              clear: both;
              content: "";
            }
          .report-header {
              padding: 20px 0;
              border-bottom: 2px solid #000;
          }
          .school-logo {
              width: 80px;
              height: 80px;
              border-radius: 50%;
              border: 2px solid #cacaca;
              margin-right: 20px;
              text-align: center;
              overflow: hidden;
          }
          .school-logo img {
              width: 100%;
              overflow: hidden;
          }
          .report-header td {
              padding: 20px 8px 0;
          }
          .report-header td.generate-date {
              padding: 0;
          }
          .report-header .information h4 {
              font-size: 20px;
              font-weight: 600;
              padding: 10px 0;
          }
          .report-header .information p, .header-right p {
              font-size: 16px;
          }
          .header-right div {
              background-color: #000;
              color: #fff;
              font-size: 20px;
              padding: 5px 20px;
              font-weight: 600;
              margin-bottom: 8px;
          }
          .student-logo {
              padding: 20px 10px;
          }
          .student-logo div {
              width: 100%;
              height: 100%;
              border: 1px solid rgb(136, 136, 136);
              border-radius: 3px;
          }
          .student-logo img {
              width: 100%;
          }
          .student-details {
              padding: 20px 10px;
              vertical-align: top;
          }
          .student-details h4 {
              font-size: 22px;
              font-weight: 600;
              margin-bottom: 10px;
          }
          .student-details span {
              color: #817e7e;
              padding: 0 15px;
              font-size: 20px;
          }
          .student-details p {
              color: #121212;
              font-size: 16px;
          }
          .student-details table {
              border-collapse: separate;
              border-spacing:0;
              border-radius: 10px;
          }
          .student-details table td {
              border-left: 1px solid #000;
              border-bottom: 1px solid #000;
              padding: 8px 10px;
              width: 33.33%;
          }
          .student-details table td b, .student-details table span {
              color: #000;
              font-size: 16px;
          }
          .student-details table td b {
              font-weight: 600;
          }
          .student-details table td:first-child {
              border-left: none;
          }
          .student-details table tr:last-child td {
              border-bottom: none;
          }
          .card {
              background-color: #EAEAEA;
              border-radius: 5px;
              padding: 20px;
              box-shadow: none;
              display: flex;
          }
          .p-20 {
              padding: 20px;
          }
          .p-y-20 {
              padding-top: 20px;
              padding-bottom: 20px;
          }
          .p-x-10 {
              padding-left: 10px;
              padding-right: 10px;
          }
          .p-t-0 {
              padding-top: 0px;
          }
          .p-b-8 {
              padding-bottom: 8px;
          }
          .width-160 {
              width: 160px;
          }
          .m-r-10 {
              margin-right: 10px;
          }
          .m-r-20 {
              margin-right: 20px;
          }
          .m-b-5 {
              margin-bottom: 5px;
          }
          .m-b-8 {
              margin-bottom: 8px;
          }
          .m-b-20 {
              margin-bottom: 20px;
          }
          .m-b-15 {
              margin-bottom: 15px;
          }
          .m-b-10 {
              margin-bottom: 10px;
          }
          .m-t-20 {
              margin-top: 20px;
          }
          .m-b-15 {
              margin-bottom: 15px;
          }
          .font-bold {
              font-weight: 600;
          }
          .font-medium {
              font-weight: 500;
          }
          .f-s-20 {
              font-size: 20px;
          }
          .f-s-18 {
              font-size: 18px;
          }
          .f-s-16 {
              font-size: 16px;
          }
          .bg-black {
              background-color: #000;
          }
          .rounded-3 {
              border-radius: 3px;
          }
          .text-white {
              color: #fff;
          }
          .p-y-5 {
              padding-top: 5px;
              padding-bottom: 5px;
          }
          .p-x-10 {
              padding-left: 10px;
              padding-right: 10px;
          }
          .bg-slate {
              background-color: #E5E5E5;
          }
          .information-table {
              border: 1px solid #000;
              border-collapse: separate;
              border-spacing: 0;
              border-radius: 10px;
              margin-bottom: 30px;
          }
          .information-table th {
              border-bottom: 1px solid #000;
              padding: 8px 5px;
              text-align: left;
              vertical-align: top;
          }
          .information-table td {
              padding: 8px 5px;
              border-bottom: 1px solid #000;
          }
          .information-table tr:first-child th:first-child {
              border-top-left-radius: 10px;
          }
          .information-table tr:first-child th:last-child {
              border-top-right-radius: 10px;
          }
          .information-table tr:last-child td:first-child {
              border-bottom-left-radius: 10px;
          }
          .information-table tr:last-child td:last-child {
              border-bottom-right-radius: 10px;
          }
          .information-table tr:last-child td {
              border-bottom: none;
          }
          table td {
              vertical-align: top;
          }
          .bullet {
              width: 5px;
              height: 5px;
              border-radius: 100%;
              background-color: #000;
              margin: 0 20px;
              vertical-align: middle;
          }
          .report-header .header-left {
              width: 68%;
          }
          .report-header .information {
              width: calc(100% - 110px);
          }
          </style>
        </head>
    <body onload="window.print()">${printContents}</body>
      </html>`
    );
    popupWin.document.close();
    document.getElementById('printComments').className = 'hidden';
    return;      
    }
  }
}
