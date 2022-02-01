import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import icDeleteForever from "@iconify/icons-ic/twotone-delete-forever";
import { TranslateService } from "@ngx-translate/core";
import { LoginService } from "src/app/services/login.service";
import { DefaultValuesService } from "../../../common/default-values.service";
import { GetAccessLogInfoModel } from "../../../models/get-access-log.model";
import { CommonService } from "../../../services/common.service";
import { MatTableDataSource } from "@angular/material/table";
import { MatSnackBar } from "@angular/material/snack-bar";
import { FormControl} from "@angular/forms";
import { DatePipe } from "@angular/common";
import {
  debounceTime,
  distinctUntilChanged,
  takeUntil,
  filter,
} from "rxjs/operators";
import * as moment from 'moment';
import { LoaderService } from "src/app/services/loader.service";
import { Subject } from "rxjs";
import { ExcelService } from "../../../services/excel.service";
import { MatPaginator } from "@angular/material/paginator";

import { MatDialog } from "@angular/material/dialog";
import { ConfirmDialogComponent } from "../../shared-module/confirm-dialog/confirm-dialog.component";
import { Router } from "@angular/router";
// export interface LogData {
//   loginTime: string;
//   loginEmail: string;
//   name: string;
//   profile: string;
//   failureCount: number;
//   status: string;
//   ipAddress: string;
// }

// export const LogData: LogData[] = [
//   // {loginTime: '2021-06-17 11:26:05', loginEmail: 'johndoe@example.com', name: 'John Doe', profile: 'Super Admin', failureCount: 0, status: 'Success', ipAddress: '123.123.12.123'},
//   // {loginTime: '2021-06-17 11:14:28', loginEmail: 'johndoe@example.com', name: 'Danny Anderson', profile: 'Student', failureCount: 0, status: 'Success', ipAddress: '123.123.12.123'},
//   // {loginTime: '2021-06-17 11:03:16', loginEmail: 'johndoe@example.com', name: 'Roman Loafer', profile: 'Student', failureCount: 0, status: 'Success', ipAddress: '123.123.12.123'},
//   // {loginTime: '2021-06-17 10:19:43', loginEmail: 'johndoe@example.com', name: 'Ella Brown', profile: 'Teacher', failureCount: 0, status: 'Success', ipAddress: '123.123.12.123'},
//   // {loginTime: '2021-06-17 11:26:05', loginEmail: 'johndoe@example.com', name: 'Adriana Garcia', profile: 'Teacher', failureCount: 0, status: 'Success', ipAddress: '123.123.12.123'},
//   // {loginTime: '2021-06-17 10:19:08', loginEmail: 'johndoe@example.com', name: 'Javier Holmes', profile: 'Student', failureCount: 0, status: 'Success', ipAddress: '123.123.12.123'},
//   // {loginTime: '2021-06-17 10:18:24', loginEmail: 'johndoe@example.com', name: 'Olivia Jones', profile: 'Student', failureCount: 0, status: 'Success', ipAddress: '123.123.12.123'},
//   // {loginTime: '2021-06-17 10:10:05', loginEmail: 'johndoe@example.com', name: 'Laura Paiva', profile: 'Student', failureCount: 0, status: 'Success', ipAddress: '123.123.12.123'},
//   // {loginTime: '2021-06-17 11:26:05', loginEmail: 'johndoe@example.com', name: 'John Doe', profile: 'Super Admin', failureCount: 0, status: 'Success', ipAddress: '123.123.12.123'},
//   // {loginTime: '2021-06-17 11:03:16', loginEmail: 'johndoe@example.com', name: 'Alyssa Kimathi', profile: 'Super Admin', failureCount: 0, status: 'Success', ipAddress: '123.123.12.123'},
// ];

@Component({
  selector: "vex-access-log",
  templateUrl: "./access-log.component.html",
  styleUrls: ["./access-log.component.scss"],
})
export class AccessLogComponent implements OnInit, OnDestroy {
  columns = [
    { label: "Login Time", property: "loginTime", type: "text", visible: true },
    {label: "Login Email",property: "loginEmail",type: "text",visible: true,},
    { label: "Name", property: "name", type: "text", visible: true },
    { label: "Profile", property: "profile", type: "text", visible: true },
    {label: "Failure Count",property: "failureCount",type: "text",visible: true,},
    { label: "Status", property: "status", type: "text", visible: true },
    { label: "IP Address", property: "ipAddress", type: "text", visible: true },
  ];
  accessLogModel: GetAccessLogInfoModel = new GetAccessLogInfoModel();
  icDeleteForever = icDeleteForever;
  totalCount = 0;
  pageNumber: number;
  pageSize: number;
  accessLog: MatTableDataSource<any>;
  searchCtrl: FormControl;
  filterParams: any;
  // displayedColumns: string[] = [
  //   "loginTime",
  //   "loginEmail",
  //   "name",
  //   "profile",
  //   "failureCount",
  //   "status",
  //   "ipAddress",
  // ];
  // logData = LogData;
  dateVal: Date;
  fromDataValue: string;
  toDataValue: string;
  isVisible: boolean = false;
  isSuccessOrNot: string;
  loading: boolean;
  recordNotFound:string="No record found";
  noRecordFound:boolean=false;
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  destroySubject$: Subject<void> = new Subject();
  constructor(
    public translateService: TranslateService,
    private defaultValueService: DefaultValuesService,
    public loginService: LoginService,
    private commonService: CommonService,
    private snackbar: MatSnackBar,
    private datepipe: DatePipe,
    private excelService: ExcelService,
    private defaultValuesService: DefaultValuesService,
    private loaderService: LoaderService,
    private dialog: MatDialog,
    private router:Router
  ) {
    this.loaderService.isLoading
      .pipe(takeUntil(this.destroySubject$))
      .subscribe((currentState) => {
        this.loading = currentState;
      });
    // translateService.use("en");
  }

  ngOnInit(): void {
    this.searchCtrl = new FormControl();
  }

  //from date should not be greater than to date
  dateCompare(fromDataValue) {
    this.dateVal = new Date(
      this.datepipe.transform(fromDataValue.value, "yyyy,MM,dd")
    );
  }

  //get range value
  onSearch(fromDataValue, toDataValue) {
    // this.searchCtrl = new FormControl();
    this.fromDataValue = this.datepipe.transform( fromDataValue.value,"yyyy-MM-dd" );
    this.toDataValue = this.datepipe.transform( toDataValue.value,"yyyy-MM-dd" );
    if ( fromDataValue.value != "" && toDataValue.value != "" ) {
      if ( this.fromDataValue <= this.toDataValue ) {
        this.accessLogModel.dobStartDate = this.fromDataValue;
        this.accessLogModel.dobEndDate = this.toDataValue;
        this.accessLogModel.filterParams = null;
        this.getAccessLog();
      }
      if (this.fromDataValue > this.toDataValue) {
        this.snackbar.open(
          "To Date value should be greater than From Date value",
          "",
          {
            duration: 10000,
          }
        );
      }
    } else if (fromDataValue.value == "" && toDataValue.value == "") {
      this.snackbar.open("Choose From date and To date", "", {
        duration: 10000,
      });
    } else if ((fromDataValue.value != "" && toDataValue.value == "") || (fromDataValue.value == "" && toDataValue.value != "")) {
      this.snackbar.open("Choose both From date and To date", "", {
        duration: 10000,
      });
    }
  }
  formatDate(date) {
    return moment.utc(date).local().format("YYYY-MM-DD HH:mm:ss");
  }

  //get all data from api
  getAccessLog() {
    this.accessLogModel.tenantId = this.defaultValueService.getTenantID();
    this.loginService
      .getAllUserAccessLog(this.accessLogModel)
      .subscribe((res) => {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          if (res.userAccessLogList === null) {
            this.totalCount = null;
            this.accessLog = new MatTableDataSource([]);
            this.snackbar.open(res._message, "", {
              duration: 10000,
            });
          } else {
            this.accessLog = new MatTableDataSource([]);
            this.totalCount = null;
          }
        }
        else {
          this.isVisible = true;
          if(res.totalCount>0)
          {res.userAccessLogList.map((item) => {
            item.createdOn = this.formatDate(item.createdOn);
          });}
          this.accessLog = new MatTableDataSource(res.userAccessLogList);
          this.totalCount = res.totalCount;
        }
      });
  }

  //Page
  getPageEvent(event) {
    this.accessLogModel.pageNumber = event.pageIndex + 1;
    this.accessLogModel.dobStartDate = this.fromDataValue;
    this.accessLogModel.dobEndDate = this.toDataValue;
    this.accessLogModel.pageSize = event.pageSize;
    this.defaultValuesService.setPageSize(event.pageSize);
    this.getAccessLog();
  }
  get visibleColumns() {
    return this.columns
      .filter((column) => column.visible)
      .map((column) => column.property);
  }
  exportAccessLogListToExcel() {
    this.loading=true;
    const accessLogModel: GetAccessLogInfoModel = new GetAccessLogInfoModel();
    accessLogModel.filterParams = this.filterParams;
    accessLogModel.pageNumber = 0;
    accessLogModel.pageSize = 0;
    accessLogModel.dobStartDate = this.fromDataValue;
    accessLogModel.dobEndDate = this.toDataValue;
    this.loginService.getAllUserAccessLog(accessLogModel).subscribe((res) => {
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
        this.snackbar.open(
          "Failed to export access log list." + res._message,
          "",
          {
            duration: 10000,
          }
        );
      } else {
        if (res.userAccessLogList.length > 0) {
          let accessLogData;
          accessLogData = res.userAccessLogList?.map((x) => {
            return {
              [this.defaultValuesService.translateKey("loginTime")]:
                this.formatDate(x.loginAttemptDate),
              [this.defaultValuesService.translateKey("loginemail")]:
                x.emailaddress,
              [this.defaultValuesService.translateKey("name")]: x.userName,
              [this.defaultValuesService.translateKey("profile")]: x.profile,
              [this.defaultValuesService.translateKey("failureCount")]:
                x.loginFailureCount,
              [this.defaultValuesService.translateKey("status")]: this.changeExcelData(x.loginStatus),
              [this.defaultValuesService.translateKey("ipAddress")]:
                x.ipaddress,
            };
          });
          this.excelService.exportAsExcelFile(accessLogData, "Access_Log_List");
        } else {
          this.snackbar.open(
            "No records found. failed to export access log list",
            "",
            {
              duration: 5000,
            }
          );
        }
      }
    });
  }
  changeExcelData(value){
    if(value===true){
      return "Success";
    }else{
      return "Failure";
    }
  }
  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  ngAfterViewInit() {
    
    this.searchCtrl.valueChanges
      .pipe(debounceTime(500), distinctUntilChanged())
      .subscribe((term) => {
        if (term != "") {
          this.filterParams = [
            {
              columnName: null,
              filterValue: this.searchCtrl.value,
              filterOption: 3,
            },
          ];
          Object.assign(this.accessLogModel, {
            filterParams: this.filterParams,
          });
          this.accessLogModel.pageNumber = 1;
          this.paginator.pageIndex = 0;
          this.accessLogModel.pageSize = this.pageSize;
          this.getAccessLog();
        } else {
          Object.assign(this.accessLogModel, { filterParams: null });
          this.accessLogModel.pageNumber = this.paginator.pageIndex + 1;
          this.accessLogModel.pageSize = this.pageSize;
          this.getAccessLog();
        }
      });
  }
// for Delete
onDelete(){
  this.dialog.open(ConfirmDialogComponent, {
    data: {
      title: "Are you sure?",
      message: "You are about to delete the records."
    }, width: '500px'
  }).afterClosed().subscribe((res) => {
    if(res){
          this.accessLogModel.isDelete=true;
          this.getAccessLog();
          this.snackbar.open(
            "Access logs delete successfully",
            "",
            {
              duration: 10000,
            }
          );
          this.accessLogModel.isDelete=false;
          this.searchCtrl.setValue("");
          // Object.assign(this.accessLogModel, { filterParams: null });
          // this.paginator.pageIndex = 0;
          // this.getAccessLog();
          
        }else{
        }
  });
}
  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
