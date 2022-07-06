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

import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { AddCommentsComponent } from './add-comments/add-comments.component';
import { AddTeacherCommentsComponent } from './add-teacher-comments/add-teacher-comments.component';
import icPrint from '@iconify/icons-ic/twotone-print';
import { AddReportCardPdf } from 'src/app/models/report-card.model';
import { GetMarkingPeriodByCourseSectionModel, GetMarkingPeriodTitleListModel } from 'src/app/models/marking-period.model';
import { MarkingPeriodService } from 'src/app/services/marking-period.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { StudentService } from 'src/app/services/student.service';
import { ReportCardService } from 'src/app/services/report-card.service';
import { Router } from '@angular/router';
import { Permissions } from '../../../../models/roll-based-access.model';
import { PageRolesPermission } from '../../../../common/page-roles-permissions.service';
import { CommonService } from 'src/app/services/common.service';

import { DefaultValuesService } from '../../../../common/default-values.service';
import { reportCardType } from '../../../../common/static-data';
@Component({
  selector: 'vex-student-report-card',
  templateUrl: './student-report-card.component.html',
  styleUrls: ['./student-report-card.component.scss']
})
export class StudentReportCardComponent implements OnInit {

  icPrint = icPrint;
  addReportCardPdf: AddReportCardPdf =  new AddReportCardPdf();
  getMarkingPeriodByCourseSectionModel: GetMarkingPeriodByCourseSectionModel = new GetMarkingPeriodByCourseSectionModel();
  markingPeriodList = [];
  markingPeriodError: boolean;
  markingPeriods = [];
  studentDetailsForViewAndEdit;
  pdfData;
  studentCreateMode;
  permissions: Permissions;
  reportCardType = JSON.parse(JSON.stringify(reportCardType));
  generatedReportCardData: any;
  halfLengthOfComment:number = 0;
  halfLengthOfGradeList: number = 0;
  constructor(
    private dialog: MatDialog,
    public translateService: TranslateService,
    private markingPeriodService: MarkingPeriodService,
    private snackbar: MatSnackBar,
    private studentService: StudentService,
    private reportCardService: ReportCardService,
    private router: Router,
    private pageRolePermissions: PageRolesPermission,
    private commonService: CommonService,
    private defaultValuesService: DefaultValuesService
    ) {
    // translateService.use("en");
    this.defaultValuesService.checkAcademicYear() && !this.studentService.getStudentId() ? this.studentService.redirectToGeneralInfo() : !this.defaultValuesService.checkAcademicYear() && !this.studentService.getStudentId() ? this.studentService.redirectToStudentList() : '';
    this.addReportCardPdf.templateType='default';
  }

  ngOnInit(): void {
    if (this.defaultValuesService.getTenantName() !== 'fedsis' && this.defaultValuesService.getTenantName() !== 'misis') {
      this.reportCardType.pop();
    }
    this.permissions = this.pageRolePermissions.checkPageRolePermission();
    this.getAllMarkingPeriodList();
    this.studentService.studentDetailsForViewedAndEdited.subscribe((res)=>{
      this.addReportCardPdf.studentsReportCardViewModelList.push({studentId: res.studentMaster.studentId});
    });

    this.studentService.studentCreatedMode.subscribe((res)=>{
      this.studentCreateMode = res;
      if(!res) {
        this.router.navigate(['/school', 'students', 'student-generalinfo']);
      }
    })

  }
  addComments(){
    this.dialog.open(AddCommentsComponent, {
      width: '500px'
    })
  }

  getAllMarkingPeriodList() {
    this.addReportCardPdf.academicYear=this.defaultValuesService.getAcademicYear();
    this.getMarkingPeriodByCourseSectionModel.isReportCard = true;

    this.markingPeriodService.getMarkingPeriodsByCourseSection(this.getMarkingPeriodByCourseSectionModel).subscribe((res)=>{
    if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.markingPeriodList = [];
          if(!res.getMarkingPeriodView){
            this.snackbar.open(res._message, '', {
              duration: 1000
            });
          }
      } else {
        this.markingPeriodList = res.getMarkingPeriodView;
      }
    })
  }

  markingPeriodChecked(event, markingPeriod) {
    if(event.checked) {
      this.markingPeriods.push(markingPeriod.value);
    } else {
      this.markingPeriods.splice(this.markingPeriods.findIndex(x => x === markingPeriod.value), 1);
    }
    this.markingPeriodError = this.markingPeriods.length > 0 ? false : true;
  }

  addAndGenerateReportCard() {
    return new Promise((resolve, reject) => {
      this.addReportCardPdf.markingPeriods = this.markingPeriods.toString();
      this.addReportCardPdf.standardGrade = null;
      this.addReportCardPdf.effortGrade = null;
      // this.reportCardService.addReportCard(this.addReportCardPdf).subscribe((res) => {
      this.reportCardService.getReportCardForStudents(this.addReportCardPdf).subscribe((res) => {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          this.snackbar.open(res._message, '', {
            duration: 1000
          });
        } else {
          resolve(res);
          // this.markingPeriods = [];
          // this.snackbar.open(res._message, '', {
          //   duration: 1000
          // });
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
        } else if (this.addReportCardPdf?.templateType === 'RMI') {
          this.generatedReportCardData.studentsReportCardViewModelList.map((res: any) => {
            this.modifyRMIDataSet(res);
          });
          setTimeout(() => {
            this.generatePdfForRMI();
          }, 100 * this.generatedReportCardData.studentsReportCardViewModelList.length);
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

  modifyRMIDataSet(res) {
    if (res.subjectDetailsForRMITemplates.length) {
      res.subjectDetailsForRMITemplates.map(item => {
        if (item.subjectName === "Math") {
          res.mathSubjectDataSet = item;
        } else if (item.subjectName === "Science") {
          res.scienceSubjectDataSet = item;
        } else if (item.subjectName === "Social Studies") {
          res.socialStudiesSubjectDataSet = item;
        } else if (item.subjectName === "Health") {
          res.healthSubjectDataSet = item;
        } else if (item.subjectName === "Marshallese") {
          res.MarshalleseSubjectDataSet = item;
        } else if (item.subjectName === "English") {
          res.englishSubjectDataSet = item;
        }
      });
    }

    if (res.gradeList.length) {
      let sortedData = [];
      const highestValue = 100;
      sortedData = res.gradeList.sort((a, b) => b.breakoff - a.breakoff);
      sortedData.map((item, index) => {
        if (index === 0) {
          res.gradeList[index].breakoffData = `${item.breakoff}-${highestValue}`;
        } else {
          res.gradeList[index].breakoffData = `${item.breakoff}-${sortedData[index - 1].breakoff - 1}`;
        }
      });
    }

    if (res.gradeList.length) {
      this.halfLengthOfGradeList = Math.floor(res.gradeList.length / 2);
    }

    if (res.attendanceDetailsViewforRMIReports.length) {
      res.attendanceDetailsViewforRMIReportsDataSet = [{}, {}, {}, {}, {}, {}, {}];
      res.attendanceDetailsViewforRMIReports.map(item => {
        if (item.markingPeriodName.trim().toLowerCase() === 'q1') {
          res.attendanceDetailsViewforRMIReportsDataSet[0] = item;
        } else if (item.markingPeriodName.trim().toLowerCase() === 'q2') {
          res.attendanceDetailsViewforRMIReportsDataSet[1] = item;
        } else if (item.markingPeriodName.trim().toLowerCase() === 's1') {
          res.attendanceDetailsViewforRMIReportsDataSet[2] = item;
        } else if (item.markingPeriodName.trim().toLowerCase() === 'q3') {
          res.attendanceDetailsViewforRMIReportsDataSet[3] = item;
        } else if (item.markingPeriodName.trim().toLowerCase() === 'q4') {
          res.attendanceDetailsViewforRMIReportsDataSet[4] = item;
        } else if (item.markingPeriodName.trim().toLowerCase() === 's2') {
          res.attendanceDetailsViewforRMIReportsDataSet[5] = item;
        }
      });

      // For Present Count
      res.attendanceDetailsViewforRMIReportsDataSet[2].presentCount = res.attendanceDetailsViewforRMIReportsDataSet[0].presentCount + res.attendanceDetailsViewforRMIReportsDataSet[1].presentCount;
      res.attendanceDetailsViewforRMIReportsDataSet[5].presentCount = res.attendanceDetailsViewforRMIReportsDataSet[3].presentCount + res.attendanceDetailsViewforRMIReportsDataSet[4].presentCount;
      res.attendanceDetailsViewforRMIReportsDataSet[6].presentCount = res.attendanceDetailsViewforRMIReportsDataSet[2].presentCount + res.attendanceDetailsViewforRMIReportsDataSet[5].presentCount;

      // For Absent Count
      res.attendanceDetailsViewforRMIReportsDataSet[2].absencesCount = res.attendanceDetailsViewforRMIReportsDataSet[0].absencesCount + res.attendanceDetailsViewforRMIReportsDataSet[1].absencesCount;
      res.attendanceDetailsViewforRMIReportsDataSet[5].absencesCount = res.attendanceDetailsViewforRMIReportsDataSet[3].absencesCount + res.attendanceDetailsViewforRMIReportsDataSet[4].absencesCount;
      res.attendanceDetailsViewforRMIReportsDataSet[6].absencesCount = res.attendanceDetailsViewforRMIReportsDataSet[2].absencesCount + res.attendanceDetailsViewforRMIReportsDataSet[5].absencesCount;
    }
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

  generatePdfForRMI() {
    let printContents, popupWin;
    printContents = document.getElementById('reportCardIdForRMI').innerHTML;
    document.getElementById('reportCardIdForRMI').className = 'block';
    popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
    if (popupWin === null || typeof (popupWin) === 'undefined') {
      document.getElementById('reportCardIdForRMI').className = 'hidden';
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

        .RMI-report-card {
            width: 1024px;
            margin: 20px auto;
        }

        table {
            border-collapse: collapse;
            width: 100%;
        }

        table td {
            vertical-align: top;
        }

        table h1 {
            font-size:16px;
            text-transform: uppercase;
        }

        .text-center {
            text-align: center;
        }

        .text-right {
            text-align: right;
        }

        .inline-block {
            display: inline-block;
        }

        .border-table {
            border: 1px solid #000;
            border-top: none;
        }

        .clearfix::after {
            display: block;
            clear: both;
            content: "";
        }

        .mt-20 {
            margin-top: 20px;
        }

        .bg-slate {
            background-color: #E5E5E5;
        }

        .w-33 {
            width: 33.33%;
        }

        .font-black {
            font-weight: 800;
        }

        .text-uppercase {
            text-transform: uppercase;
        }

        .font-italic {
            font-style: italic;
        }

        .px-10 {
            padding-left: 10px;
            padding-right: 10px;
        }

        .mt-100 {
            margin-top: 100px;
        }

        .f-s-28 {
            font-size: 28px;
        }

        .mb-5 {
            margin-bottom: 5px;
        }

        .report-card {
            margin-top: 20px;
            table-layout: fixed;
        }

        .report-card thead td:first-child {
            /* width: 100px; */
            border: none;
        }

        .report-card td {
            padding: 8px 3px;
            font-size: 14px;
            vertical-align: bottom;
        }

        .report-card thead td {
            padding: 10px 5px;
            border: 1px solid #000;
            border-top-width: 3px;
            border-bottom-width: 3px;
        }

        .report-card thead td:last-child {
            border-right-width: 3px;
        }

        .report-card thead td:nth-child(2) {
            border-left-width: 3px;
        }

        .report-card tbody {
            border: 2px solid #000;
        }

        .report-card tbody td {
            border-bottom: 1px solid #000;
            border-right: 1px solid #000;
            height: 30px;
            min-width: 20px;
        }

        .report-card tbody td:last-child {
            border-right: none;
        }

        .teacher-comment-table {
            table-layout: unset;
        }

        .teacher-comment-table tbody, .attendance-table tbody, .gpa-table tbody {
            border-width: 6px;
        }

        .teacher-comment-table tbody td.bg-slate {
            padding-left: 5px;
        }

        .teacher-comment-table tbody td {
            padding-left: 5px;
            border-right: none;
        }

        .attendance-table {
            margin-top: 5px;
            table-layout: unset;
        }

        .attendance-table tbody td {
            height: 40px;
            vertical-align: middle;
            font-size: 16px;
        }

        .gpa-table tbody td {
            vertical-align: middle;
        }

        .gpa-table tbody p {
            margin-bottom: 8px;
            display: flex;
            justify-content: center;
            white-space: nowrap;
        }

        .gpa-table tbody p span {
            flex: 1;
        }
        .gpa-table tbody p span:first-child {
            padding-left: 20px;
        }

        .gpa-table tbody p span:nth-child(2){
            margin-left: -15px;
        }

        .gpa-table tbody p span:last-child {
          margin-left: 10px;
        }

        .text-truncate {
          overflow: hidden;
          text-overflow: ellipsis;
          white-space: nowrap;
          display : block;
        }

        .main-table {
            margin-bottom: 60px;
        }

        .main-table td.w-33:nth-child(2) {
            padding: 0 15px;
        }

        .underline {
            text-decoration: underline;
        }

        .border-1 {
            border: 1px solid #000;
        }
        .p-5 {
            padding: 5px;
        }

        .rounded-ruler {
            width: 100%;
            height: 4px;
            background-color: #000;
            border-radius: 2px;
            margin: 20px 0;
        }

        .font-normal {
            font-weight: 400;
        }
        
        .spacing-3 {
            letter-spacing: 3px;
        }

        .mb-40 {
            margin-bottom: 40px;
        }

        .mb-60 {
            margin-bottom: 60px;
        }
        .style-border {
            border: 1px solid #000;
            margin: -15px 0;
            padding: 20px 10px;
        }

        .bg-black {
            background-color: #000;
        }

        .text-white {
            color: #fff;
        }

        .heading {
            font-size: 20px;
            background-color: #000;
            color: #fff;
            margin-bottom: 10px;
            padding: 10px 8px;
            letter-spacing: 1px;
        }

        .grades-details h4 {
            margin-bottom: 180px;
        }

        .grades-details h4:last-child {
            margin-bottom: 80px;
        }

        .grades-details .rounded-ruler {
            margin-bottom: 10px;
        }

        .student-details .style-border {
            height: 820px;
        }

        .student-details .style-border > div {
            margin-top: 575px;
        }

        .student-info img {
            width: 170px;
            height: 170px;
            display: block;
            text-align: center;
            border-radius: 50%;
            margin: 40px auto 20px;
            overflow: hidden;
            border: 5px solid #000;
        }

        .student-details.student-info .style-border > div {
            margin-top: 0px;
        }

        .student-details.student-info .heading {
            margin: 0 -18px;
        }

        .student-details.student-info h1 {
            font-size: 28px;
            font-weight: normal;
            margin: 40px 0 20px;
        }

        .student-details.student-info h4, .student-details.student-info p {
            margin-bottom: 5px;
        }

        .student-details.student-info .style-border .rounded-ruler {
            margin-top: 140px;
            margin-bottom: 30px;
        }

        .student-details.student-info p.spacing-3 {
            margin-bottom: 25px;
        }
    </style>
        </head>
    <body onload="window.print()">${printContents}</body>
      </html>`
      );
      popupWin.document.close();
      document.getElementById('reportCardIdForRMI').className = 'hidden';
      return;
    }
  }

// Old Method
//   generateReportCard() {
//     if(this.markingPeriods.length > 0) {
//     this.addAndGenerateReportCard().then(()=>{
//     this.reportCardService.generateReportCard(this.addReportCardPdf).subscribe((res)=>{
//     if(res._failure){
//         this.commonService.checkTokenValidOrNot(res._message);
//         this.snackbar.open(res._message, '', {
//           duration: 1000
//         });
//     } else {
//       // this.addReportCardPdf =  new AddReportCardPdf();
//      this.pdfData = res;
//     }
//     });
//   });
// } else {
//   this.markingPeriodError = true;
// }
//   }

  addTeacherComments(){
    this.dialog.open(AddTeacherCommentsComponent, {
      width: '500px'
    })
  }

}
