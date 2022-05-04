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

import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";
import icSearch from "@iconify/icons-ic/search";
import { LoaderService } from "../../../../services/loader.service";
import { debounceTime, distinctUntilChanged, takeUntil } from "rxjs/operators";
import { Subject } from "rxjs";
import { fadeInUp400ms } from "src/@vex/animations/fade-in-up.animation";
import { stagger40ms } from "src/@vex/animations/stagger.animation";
import { fadeInRight400ms } from "src/@vex/animations/fade-in-right.animation";
import { GetAllStaffModel, StaffMasterModel } from "../../../../models/staff.model";
import { StudentAttendanceService } from "../../../../services/student-attendance.service";
import { MatSnackBar } from "@angular/material/snack-bar";
import { SharedFunction } from "../../../shared/shared-function";
import { MatTableDataSource } from "@angular/material/table";
import { MatPaginator, MatPaginatorIntl } from "@angular/material/paginator";
import { MatSort } from "@angular/material/sort";
import { FormControl } from "@angular/forms";
import { Router } from "@angular/router";
import { ExcelService } from "src/app/services/excel.service";
import { DefaultValuesService } from "src/app/common/default-values.service";
import { CommonService } from "src/app/services/common.service";
import moment from "moment";

@Component({
  selector: "vex-missing-attendance",
  templateUrl: "./missing-attendance.component.html",
  styleUrls: ["./missing-attendance.component.scss"],
  animations: [
    fadeInUp400ms,
    stagger40ms,
    fadeInRight400ms
  ]
})
export class MissingAttendanceComponent implements OnInit, AfterViewInit, OnDestroy {
  icSearch = icSearch;
  getAllStaffModel: GetAllStaffModel = new GetAllStaffModel();
  destroySubject$: Subject<void> = new Subject();
  columns = [
    { label: 'teacher', property: 'teacher', type: 'text', visible: true },
    { label: 'staffId', property: 'staffInternalId', type: 'text', visible: true },
    { label: 'profile', property: 'profile', type: 'text', visible: true },
    { label: 'jobTitle', property: 'jobTitle', type: 'text', visible: true },
    { label: 'schoolEmail', property: 'schoolEmail', type: 'text', visible: true },
    { label: 'mobilePhone', property: 'mobilePhone', type: 'text', visible: true },
    { label: 'status', property: 'status', type: 'text', visible: true },
    // { label: 'action', property: 'action', type: 'text', visible: true },
  ];
  maxDate = new Date();
  minDate:string;
  loading: boolean;
  totalCount: number = 0;
  pageSize: number;
  missingAttendanceDateList = [];
  searchKeyword: string;
  staffList: MatTableDataSource<any>;
  pageNumber: number;
  searchCtrl: FormControl;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;

  constructor(
    public translateService: TranslateService,
    private loaderService: LoaderService,
    private studentAttendanceService: StudentAttendanceService,
    private commonFunction: SharedFunction,
    private defaultValuesService: DefaultValuesService,
    private router: Router,
    private excelService: ExcelService,
    private snackbar: MatSnackBar,
    private commonService: CommonService,
    private paginatorObj: MatPaginatorIntl,
  ) {
    paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
    // translateService.use("en");
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
      this.getAllStaffModel.dobEndDate = this.commonFunction.formatDateSaveWithoutTime(new Date());
    });
  }
  ngOnInit(): void {
    this.searchCtrl = new FormControl();
    this.getStaffListByDateRange();

  }

  viewStudentAttendanceDetails(element) {
    if(element.status === "active"){
      const staffFullName = `${element.firstGivenName} ${element.middleName ? element.middleName + ' ' : ''}${element.lastFamilyName}`;
      this.studentAttendanceService.setStaffDetails({ staffId: element.staffId, staffFullName, startDate: this.getAllStaffModel.dobStartDate, endDate: this.getAllStaffModel.dobEndDate });
      this.router.navigate(['/school', 'staff', 'teacher-functions', 'missing-attendance' , 'missing-attendance-details']);
      //this.pageInit = 2;
    }else
    this.snackbar.open('Inactive staff does not have any access to enter the next page', '', {
      duration: 10000
    })
  }

  ngAfterViewInit() {
    //  Sorting
    this.getAllStaffModel = new GetAllStaffModel();
    this.sort.sortChange.subscribe((res) => {
      this.getAllStaffModel.pageNumber = this.pageNumber
      this.getAllStaffModel.pageSize = this.pageSize;
      this.getAllStaffModel.sortingModel.sortColumn = res.active;
      if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
        let filterParams = [
          {
            columnName: null,
            filterValue: this.searchCtrl.value,
            filterOption: 4
          }
        ]
        Object.assign(this.getAllStaffModel, { filterParams: filterParams });
      }
      if (res.direction == "") {
        this.getAllStaffModel.sortingModel = null;
        this.getStaffListByDateRange();
        this.getAllStaffModel = new GetAllStaffModel();
        this.getAllStaffModel.sortingModel = null;
      } else {
        this.getAllStaffModel.sortingModel.sortDirection = res.direction;
        this.getStaffListByDateRange();
      }
    });

    //Searching
    this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term != '') {
        this.callWithFilterValue(term);
      } else {
        this.callWithoutFilterValue()
      }
    });
  }

  getPageEvent(event) {
    if (this.sort.active != undefined && this.sort.direction != "") {
      this.getAllStaffModel.sortingModel.sortColumn = this.sort.active;
      this.getAllStaffModel.sortingModel.sortDirection = this.sort.direction;
    }
    if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
      let filterParams = [
        {
          columnName: null,
          filterValue: this.searchCtrl.value,
          filterOption: 3
        }
      ]
      Object.assign(this.getAllStaffModel, { filterParams: filterParams });
    }
    this.getAllStaffModel.pageNumber = event.pageIndex + 1;
    this.getAllStaffModel.pageSize = event.pageSize;
    this.getStaffListByDateRange();
  }


  callWithFilterValue(term) {
    let filterParams = [
      {
        columnName: null,
        filterValue: term,
        filterOption: 4
      }
    ]
    if (this.sort.active != undefined && this.sort.direction != "") {
      this.getAllStaffModel.sortingModel.sortColumn = this.sort.active;
      this.getAllStaffModel.sortingModel.sortDirection = this.sort.direction;
    }
    Object.assign(this.getAllStaffModel, { filterParams: filterParams });
    this.getAllStaffModel.pageNumber = 1;
    this.paginator.pageIndex = 0;
    this.getAllStaffModel.pageSize = this.pageSize;
    this.getStaffListByDateRange();
  }

  callWithoutFilterValue() {
    Object.assign(this.getAllStaffModel, { filterParams: null });
    this.getAllStaffModel.pageNumber = this.paginator.pageIndex + 1;
    this.getAllStaffModel.pageSize = this.pageSize;
    if (this.sort.active != undefined && this.sort.direction != "") {
      this.getAllStaffModel.sortingModel.sortColumn = this.sort.active;
      this.getAllStaffModel.sortingModel.sortDirection = this.sort.direction;
    }
    this.getStaffListByDateRange();
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  getStaffListByDateRange() {
    if (this.getAllStaffModel.dobStartDate && this.getAllStaffModel.dobEndDate) {
      this.getAllStaffModel.sortingModel = null;
      this.getAllStaffModel.dobStartDate = this.commonFunction.formatDateSaveWithoutTime(this.getAllStaffModel.dobStartDate);
      this.getAllStaffModel.dobEndDate = this.commonFunction.formatDateSaveWithoutTime(this.getAllStaffModel.dobEndDate);
      this.studentAttendanceService.staffListForMissingAttendance(this.getAllStaffModel).subscribe((res) => {
        if (res) {
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.staffList = new MatTableDataSource([]);
            if (!res.staffMaster) {
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
            }
          }
          else {
            this.totalCount = res.totalCount;
            this.pageNumber = res.pageNumber;
            this.pageSize = res._pageSize;
            this.staffList = new MatTableDataSource(res.staffMaster);
            for (let staff of this.staffList.filteredData) {
              staff.staffSchoolInfo.map(schoolList => {
                if (this.defaultValuesService.getSchoolID() == schoolList.schoolAttachedId) {
                  staff.schoolName = schoolList.schoolAttachedName;
                  if (schoolList.endDate === null)
                    staff.status = 'active';
                  else {
                    if (moment(new Date()).isBetween(schoolList.startDate, schoolList.endDate))
                      staff.status = 'active';
                    else
                      staff.status = 'inactive';
                  }
                }
              })
            }
            //this.getAllStaffModel = new GetAllStaffModel();
          }
        }
        else {
          this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      });
    }
    else {
      this.getAllStaffModel.sortingModel = null;
      this.getAllStaffModel.dobStartDate = null;
      this.getAllStaffModel.dobEndDate = null;
      this.studentAttendanceService.staffListForMissingAttendance(this.getAllStaffModel).subscribe((res) => {
        if (res) {
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            if (res.staffMaster == null) {
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
              this.staffList = new MatTableDataSource([]);
            } else {
              this.staffList = new MatTableDataSource([]);
            }
          }
          else {
            this.totalCount = res.totalCount;
            this.pageNumber = res.pageNumber;
            this.pageSize = res._pageSize;
            this.staffList = new MatTableDataSource(res.staffMaster);
            this.missingAttendanceDateList = res.missingAttendanceDateList;
            for (let staff of this.staffList.filteredData) {
              staff.staffSchoolInfo.map(schoolList => {
                if (this.defaultValuesService.getSchoolID() == schoolList.schoolAttachedId) {
                  staff.schoolName = schoolList.schoolAttachedName;
                  if (schoolList.endDate === null)
                    staff.status = 'active';
                  else {
                    if (moment(new Date()).isBetween(schoolList.startDate, schoolList.endDate))
                      staff.status = 'active';
                    else
                      staff.status = 'inactive';
                  }
                }
              })
            }
            this.minDate = null;
            for (var i = 0; i < this.missingAttendanceDateList.length; i++) {
              var current = this.missingAttendanceDateList[i];
              if (this.minDate === null || current < this.minDate) {
                this.minDate = current;
              }
              this.getAllStaffModel.dobStartDate= this.minDate;
              //this.getAllStaffModel= new GetAllStaffModel();
            }
          }
          }
        else {
            this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
              duration: 10000
            });
          }
        });
    }
  }

  exportStaffListToExcel(){
    let getAllStaff: GetAllStaffModel = new GetAllStaffModel();
    getAllStaff.pageNumber=0;
    getAllStaff.pageSize=0;
    getAllStaff.sortingModel=null;
    this.studentAttendanceService.staffListForMissingAttendance(getAllStaff).subscribe(res => {
       if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.snackbar.open('Failed to Export Staff List.'+ res._message, '', {
          duration: 10000
          });
        }else{
          if(res.staffMaster.length>0){
            let staffList = res.staffMaster?.map((x:StaffMasterModel)=>{
              return {
                [this.defaultValuesService.translateKey('teacher')] : x.firstGivenName+" "+x.lastFamilyName,
                [this.defaultValuesService.translateKey('staffId')]: x.staffInternalId,
                [this.defaultValuesService.translateKey('profile')]: x.profile,
                [this.defaultValuesService.translateKey('jobTitle')]: x.jobTitle,
                [this.defaultValuesService.translateKey('schoolEmail')]:x.schoolEmail,
                [this.defaultValuesService.translateKey('mobilePhone')]:x.mobilePhone
             }
            });
            this.excelService.exportAsExcelFile(staffList,'Missing_Attendance_Staffs_List_')
          }else{
            this.snackbar.open('No Records Found. Failed to Export Staff List','', {
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
