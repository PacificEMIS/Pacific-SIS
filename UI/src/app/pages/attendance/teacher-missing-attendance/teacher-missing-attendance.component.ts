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

import { AfterViewInit, Component, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import icHowToReg from '@iconify/icons-ic/outline-how-to-reg';
import { GetStaffModel } from '../../../models/staff.model';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { StaffPortalService } from '../../../services/staff-portal.service';
import { CommonService } from 'src/app/services/common.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ScheduledCourseSectionViewModel } from '../../../models/dashboard.model';
import { DatePipe } from "@angular/common";
import { Subject } from 'rxjs';
import { LoaderService } from "src/app/services/loader.service";
import {
  debounceTime,
  distinctUntilChanged,
  takeUntil,
  filter,
} from "rxjs/operators";

@Component({
  selector: 'vex-teacher-missing-attendance',
  templateUrl: './teacher-missing-attendance.component.html',
  styleUrls: ['./teacher-missing-attendance.component.scss']
})
export class TeacherMissingAttendanceComponent implements OnInit {

  icHowToReg = icHowToReg;
  getStaffModel: GetStaffModel = new GetStaffModel();
  // @Input() courseSectionClass;
  totalCount: number = 0;
  pageSize: number;
  pageNumber: number;
  fromDateVal: string;
  toDateVal: string;
  dateVal:Date;
  CourseSectionViewList:any;
  scheduledCourseSectionViewModel: ScheduledCourseSectionViewModel = new ScheduledCourseSectionViewModel();
  valueStartingDate;
  today:Date;
  destroySubject$: Subject<void> = new Subject();
  loading:boolean;
  isHasRecord:boolean;

  constructor(public translateService: TranslateService,
    private defaultValuesService: DefaultValuesService,
    private staffPortalService: StaffPortalService,
    private router: Router,
    private commonService: CommonService,
    private snackbar: MatSnackBar,
    private loaderService: LoaderService,
    private datepipe: DatePipe,
  ) {
    // translateService.use("en");
  }
  
  ngOnInit(): void {
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
    this.getAllMissingAttendanceListForStaff();
  }

  //to call the api for getAllMissingAttendanceListForStaff
  getAllMissingAttendanceListForStaff() {
    this.loading=true;
    this.getStaffModel.staffId = this.defaultValuesService.getUserId();
    this.getStaffModel.schoolId=this.defaultValuesService.getSchoolID();
    this.staffPortalService.getAllMissingAttendanceListForStaff(this.getStaffModel).subscribe((res: ScheduledCourseSectionViewModel) => {
      if (res) {
      if(res._failure){
        this.isHasRecord=false;
        this.commonService.checkTokenValidOrNot(res._message);
          // this.courseSectionViewList = new MatTableDataSource([]);
          if (!res.courseSectionViewList) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        }
        else {
          // this.isHasRecord=true;
          this.today = new Date();
          this.CourseSectionViewList=res.courseSectionViewList
          if(this.CourseSectionViewList[0].attendanceDate){
          this.valueStartingDate=this.CourseSectionViewList[0].attendanceDate;
          }else{
            this.valueStartingDate=this.today;
          }
        }
      }
      else {
        this.isHasRecord=false;
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
    // this.loading=false; 
  }

  //to fix the to date not less then from date
  dateCompare(fromDataValue,toDataValue) {
    this.dateVal = new Date(
      this.datepipe.transform(fromDataValue.value, "yyyy,MM,dd")
    );
    this.sortByDate(fromDataValue, toDataValue)
  }

  //to call the api with date sorting
  sortByDate(fromDataValue, toDataValue) {
    // this.searchCtrl = new FormControl();
    this.fromDateVal = this.datepipe.transform( fromDataValue.value,"yyyy-MM-dd" );
    this.toDateVal = this.datepipe.transform( toDataValue.value,"yyyy-MM-dd" );
    if ( fromDataValue.value != "" && toDataValue.value != "" ) {
      if ( this.fromDateVal <= this.toDateVal ) {
        this.getStaffModel.DobStartDate = this.fromDateVal;
        this.getStaffModel.DobEndDate = this.toDateVal;
        this.getAllMissingAttendanceListForStaff();
      }
      if (this.fromDateVal > this.toDateVal) {
        this.snackbar.open(
          "To Date value should be greater than From Date value",
          "",
          {
            duration: 10000,
          }
        );
      }
    }
  }
  onSent(element){
    this.defaultValuesService.setSelectedCourseSection(element)
  }
  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
