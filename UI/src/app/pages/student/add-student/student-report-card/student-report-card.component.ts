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
  reportCardType = reportCardType;
  generatedReportCardData: any;
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
