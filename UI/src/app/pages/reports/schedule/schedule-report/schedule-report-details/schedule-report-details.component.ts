import { Component, OnDestroy, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import icPrint from '@iconify/icons-ic/twotone-print';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { GetAllCourseSectionModel } from 'src/app/models/course-section.model';
import { CommonService } from 'src/app/services/common.service';
import { CourseSectionService } from 'src/app/services/course-section.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ScheduleStudentListViewModel } from 'src/app/models/student-schedule.model';
import { StudentScheduleService } from 'src/app/services/student-schedule.service';
import { Router } from '@angular/router';
import { takeUntil } from 'rxjs/operators';
import { LoaderService } from 'src/app/services/loader.service';
import { Subject } from 'rxjs';
import { ExcelService } from 'src/app/services/excel.service';
import { GradeScaleModel } from 'src/app/models/grades.model';

@Component({
  selector: 'vex-schedule-report-details',
  templateUrl: './schedule-report-details.component.html',
  styleUrls: ['./schedule-report-details.component.scss']
})
export class ScheduleReportDetailsComponent implements OnInit, OnDestroy {
  icPrint = icPrint;
  getAllCourseSectionModel: GetAllCourseSectionModel = new GetAllCourseSectionModel();
  GetCourseId;
  courseSectionForView;
  getAllStudent: ScheduleStudentListViewModel = new ScheduleStudentListViewModel();
  selectedCourseSection = 0;
  studentListView;
  loading: boolean;
  selectedCourseName;
  destroySubject$: Subject<void> = new Subject();
  studentCount = 0;
  courseListCount = 0;
  today = new Date();
  selectedStaffName;
  parentData;
  allCourseSectionData;
  constructor(public translateService: TranslateService,
    private courseSectionService: CourseSectionService,
    private commonService: CommonService,
    private excelService: ExcelService,
    private snackbar: MatSnackBar,
    private loaderService: LoaderService,
    private studentScheduleService: StudentScheduleService,
    private defaultValuesService: DefaultValuesService,
    private router: Router
  ) {
    this.parentData=this.router.getCurrentNavigation().extras.state;
    if(!this.parentData)
      this.router.navigate(['/school', 'reports', 'schedule', 'schedule-report'])
  }

  ngOnInit(): void {
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
    this.getAllCourseSection()
  }

  getAllCourseSection() {
    this.getAllCourseSectionModel.courseId = this.parentData.courseId;
    this.getAllCourseSectionModel.schoolDetails=true;
    this.getAllCourseSectionModel.academicYear = this.defaultValuesService.getAcademicYear();
    this.courseSectionService.getAllCourseSection(this.getAllCourseSectionModel).subscribe(
      (res: GetAllCourseSectionModel) => {
        if (res) {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            this.courseListCount = res.getCourseSectionForView.length;
            if (!res.getCourseSectionForView) {
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
            }
          } else {
            this.allCourseSectionData=res;
            res.getCourseSectionForView?.map((item)=>{
              if(item.courseSection.durationBasedOnPeriod){
                if(item.courseSection.quarters!=null){
                  item.courseSection.mpTitle=item.courseSection.quarters.title;
                  item.courseSection.mpStartDate=item.courseSection.quarters.startDate;
                  item.courseSection.mpEndDate=item.courseSection.quarters.endDate;
                }else if(item.courseSection.schoolYears!=null){
                  item.courseSection.mpTitle=item.courseSection.schoolYears.title;
                  item.courseSection.mpStartDate=item.courseSection.schoolYears.startDate;
                  item.courseSection.mpEndDate=item.courseSection.schoolYears.endDate;
                }else if(item.courseSection.progressPeriods!=null){
                  item.courseSection.mpTitle=item.courseSection.progressPeriods.title;
                  item.courseSection.mpStartDate=item.courseSection.progressPeriods.startDate;
                  item.courseSection.mpEndDate=item.courseSection.progressPeriods.endDate;
                } else{
                  item.courseSection.mpTitle=item.courseSection.semesters.title;
                  item.courseSection.mpStartDate=item.courseSection.semesters.startDate;
                  item.courseSection.mpEndDate=item.courseSection.semesters.endDate;
                }
              }
            })
            this.courseSectionForView=res.getCourseSectionForView;
            this.courseListCount = res.getCourseSectionForView.length;
            this.getAllStudent.courseSectionIds = [this.courseSectionForView[this.selectedCourseSection].courseSection.courseSectionId];
            this.selectedCourseName = this.courseSectionForView[this.selectedCourseSection].courseSection.courseSectionName;
            this.selectedStaffName = this.courseSectionForView[this.selectedCourseSection].staffName;
            this.searchScheduledStudentForGroupDrop();
          }
        }
        else {
          this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      }
    );
  }

  onChangeCourseSection(index) {
    this.selectedCourseSection = index
    this.getAllStudent.courseSectionIds = [this.courseSectionForView[this.selectedCourseSection].courseSection.courseSectionId];
    this.selectedCourseName = this.courseSectionForView[this.selectedCourseSection].courseSection.courseSectionName;
    this.selectedStaffName = this.courseSectionForView[this.selectedCourseSection].staffName;
    this.searchScheduledStudentForGroupDrop();
  }

  searchScheduledStudentForGroupDrop() {
    this.getAllStudent.includeInactive = true;
    this.getAllStudent.profilePhoto = true;
    this.studentScheduleService.searchScheduledStudentForGroupDrop(this.getAllStudent).subscribe(
      (res: ScheduleStudentListViewModel) => {
        if (res) {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            this.studentListView = res.scheduleStudentForView;
            this.studentCount = this.studentListView.length;
            if (!res.scheduleStudentForView) {
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
            }
          } else {
            this.studentListView = res.scheduleStudentForView;
            this.studentCount = this.studentListView.length;
          }
        }
        else {
        }
      }
    );
  }
  exportScheduleStudentsListToExcel() {
    if (this.studentCount) {
      let studentList;
      studentList = this.studentListView?.map(x => {
        return {
          [this.defaultValuesService.translateKey("studentName")]: `${x.firstGivenName} ${x.lastFamilyName}`,
          [this.defaultValuesService.translateKey("studentId")]: `${x.studentId ? x.studentId : '-'}`,
          [this.defaultValuesService.translateKey("gradeLevel")]: `${x.gradeLevel ? x.gradeLevel : '-'}`,
          [this.defaultValuesService.translateKey("alternateId")]: `${x.alternateId ? x.alternateId : '-'}`,
          [this.defaultValuesService.translateKey("section")]: `${x.section ? x.section : '-'}`,
          [this.defaultValuesService.translateKey("mobilePhone")]: `${x.mobilePhone ? x.mobilePhone : '-'}`,
        };
      });
      this.excelService.exportAsExcelFile(studentList, "Student_Schedule_Report");
    } else {
      this.snackbar.open('No Student Found. Failed to Export Schedule Students List', '', {
        duration: 5000
      });
    }
  }

  printStudentsList() {
    if (!this.studentCount) {
      this.snackbar.open('No Student Found. Failed to Print Schedule Students List', '', {
        duration: 5000
      });
      return;
    }
    this.loading = true;
    setTimeout(() => {
      this.generatePDF();
      this.loading = false;
    }, 100 * this.studentCount);
  }

  generatePDF() {
    let printContents, popupWin;
    printContents = document.getElementById('printSectionId').innerHTML;
    document.getElementById('printSectionId').className = 'block';
    popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
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
              padding: 20px;
              padding-bottom: 10px;
          }
  
          .report-header td.generate-date {
              padding: 0;
          }
  
          .report-header .information h4 {
              font-size: 20px;
              font-weight: 600;
              padding: 10px 0;
          }
  
          .report-header .information p,
          .header-right p {
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
              padding: 20px;
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
  
          .p-b-8 {
              padding-bottom: 8px;
          }
  
          .p-20 {
              padding:20px;
          }
  
          .width-160 {
              width: 160px;
          }
  
          .m-b-15 {
              margin-bottom: 15px;
          }
  
          .bg-black {
              background-color: #000;
          }
  
          .bg-slate {
              background-color: #E5E5E5;
          }
  
          .information-table {
              border: 1px solid #000;
              border-collapse: separate;
              border-spacing: 0;
              border-radius: 10px;
          }
          .information-table th {
              border-bottom: 1px solid #000;
              padding: 10 12px;
              text-align: left;
          }
  
          .information-table tr:first-child th:first-child {
              border-top-left-radius: 10px;
          }
  
          .information-table tr:first-child th:last-child {
              border-top-right-radius: 10px;
          }
          .information-table tr:last-child td {
              border-bottom: none;
          }
  
          .information-table td {
              padding: 10px 12px;
              border-bottom: 1px solid #000;
          }
  
          table td {
              vertical-align: middle;
          }
  
          .report-header .header-left {
              width: 65%;
          }
  
          .report-header .header-right {
              width: 35%;
          }
  
          .report-header .information {
              width: calc(100% - 110px);
          }
          .course-details {
              padding: 20px 20px 0;
          }
          .course-details table td {
              width: 33.33%;
          }
          .course-details table td label {
              font-size: 14px;
              font-weight: 600;
              margin-bottom: 5px;
              display: block;
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

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
