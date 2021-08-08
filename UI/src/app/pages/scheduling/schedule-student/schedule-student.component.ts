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

import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
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
import { CommonService } from 'src/app/services/common.service';

@Component({
  selector: 'vex-schedule-student',
  templateUrl: './schedule-student.component.html',
  styleUrls: ['./schedule-student.component.scss'],
  providers: [TitleCasePipe]
})
export class ScheduleStudentComponent implements OnInit, OnDestroy {
  studentList = [];
  studentText: string;
  sectionText: string;
  viewReport: boolean = false;
  failedScheduling: boolean = false
  showReportTable: boolean = false;
  courseSectionList = [];
  showStudentCount: boolean = false;
  showCourseSectionCount: boolean = false;
  destroySubject$: Subject<void> = new Subject();
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
    private snackbar: MatSnackBar,
    private titlecasePipe: TitleCasePipe,
    private pageRolePermissions: PageRolesPermission,
    private commonService: CommonService,
  ) {
    //translateService.use('en');
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
  }

  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission();
  }

  viewScheduledReport() {
    this.studentScheduleService.studentScheduleReport(this.studentScheduleReportViewModel).subscribe(data => {
     if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);

      }
      else {
        if (data.scheduleReport.length > 0) {
          this.showReportTable = true;
        }
        data.scheduleReport.map((item) => {
          for (const key in item) {
            if (item.hasOwnProperty(key)) {
              if (key === 'studentId'){
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
    this.dialog.open(AddStudentComponent, {
      width: '900px'
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

  selectCourseSection() {
    this.dialog.open(AddCourseSectionComponent, {
      width: '900px'
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
    this.studentScheduleService.addStudentCourseSectionSchedule(this.studentCourseSectionScheduleAddViewModel).pipe(takeUntil(this.destroySubject$)).subscribe(data => {
     if(data._failure){
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
     if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);

      }
      else {
        let modifiedReportData = [];
        data.scheduleReport.map((item) => {
          let obj = {};
          for (const key in item) {
            if (item.hasOwnProperty(key)) {
              if (key === 'studentId'){
                delete item[key];
              }else{
                Object.assign(obj, { [key==='studentInternalId'?'Student ID':this.translateKey(key)] : item[key] })
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
