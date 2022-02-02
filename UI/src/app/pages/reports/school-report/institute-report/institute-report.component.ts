import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import icListAlt from '@iconify/icons-ic/twotone-list-alt';
import icAccountBalance from '@iconify/icons-ic/twotone-account-balance';
import { GetAllSchoolModel } from 'src/app/models/get-all-school.model';
import { SchoolService } from 'src/app/services/school.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { CommonService } from 'src/app/services/common.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FormControl } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { LoaderService } from 'src/app/services/loader.service';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { MatPaginator } from '@angular/material/paginator';
import { MatCheckbox } from '@angular/material/checkbox';
import { GetSchoolReportModel } from 'src/app/models/report.model';
import { ReportService } from 'src/app/services/report.service';
import { DomSanitizer } from '@angular/platform-browser';
import icCheckboxChecked from '@iconify/icons-ic/check-box';
import icCheckboxUnchecked from '@iconify/icons-ic/check-box-outline-blank';

@Component({
  selector: 'vex-institute-report',
  templateUrl: './institute-report.component.html',
  styleUrls: ['./institute-report.component.scss']
})
export class InstituteReportComponent implements OnInit, OnDestroy, AfterViewInit {
  icListAlt = icListAlt;
  icAccountBalance = icAccountBalance;
  icCheckboxChecked = icCheckboxChecked;
  icCheckboxUnchecked = icCheckboxUnchecked;
  displayedColumns: string[] = ['schoolCheck', 'schoolName', 'address', 'state', 'principal', 'phone'];
  getAllSchoolModel: GetAllSchoolModel = new GetAllSchoolModel();
  getSchoolReportModel: GetSchoolReportModel = new GetSchoolReportModel();
  today: Date = new Date();
  destroySubject$: Subject<void> = new Subject();
  loading: boolean;
  searchCtrl: FormControl;
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  instituteList: MatTableDataSource<any>;
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild('masterCheckBox') masterCheckBox: MatCheckbox;
  listOfSchools = [];
  selectedSchools = [];
  generatedReportCardData: any;
  studentLogo: any;
  staffLogo: any;
  parentLogo: any;

  constructor(public translateService: TranslateService,
    private schoolService: SchoolService,
    private defaultValuesService: DefaultValuesService,
    private commonService: CommonService,
    private snackbar: MatSnackBar,
    private loaderService: LoaderService,
    private reportService: ReportService,
    private domSanitizer: DomSanitizer) {
    // translateService.use("en");
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });

    this.studentLogo = this.domSanitizer.bypassSecurityTrustUrl("data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSI1NC4wODgiIGhlaWdodD0iMzkuMjY3IiB2aWV3Qm94PSIwIDAgNTQuMDg4IDM5LjI2NyI+DQogIDxnIGlkPSJpY29uLWdyYWR1YXRpb24tY2FwIiB0cmFuc2Zvcm09InRyYW5zbGF0ZSgwIDApIj4NCiAgICA8cGF0aCBpZD0iUGF0aF8xMjMxIiBkYXRhLW5hbWU9IlBhdGggMTIzMSIgZD0iTTUyLjE0MSwxNi40MmwtMjIuNzYtOS42M2E1LjcsNS43LDAsMCwwLTQuNDE5LDBMMi4yLDE2LjQyYTMuNCwzLjQsMCwwLDAsMCw2LjI2MWw3Ljk3NiwzLjM3M1YzNi45QTUuNjExLDUuNjExLDAsMCwwLDEyLjksNDEuNzQzYTI4LjI4NCwyOC4yODQsMCwwLDAsMjguNTUxLDBBNS42MTEsNS42MTEsMCwwLDAsNDQuMTcsMzYuOVYyNi4wNTRsMy40LTEuNDM5djUuMzczYTIuODE5LDIuODE5LDAsMCwwLS41NSw0Ljg1OCwyLjgyMiwyLjgyMiwwLDAsMC0xLjE1LDIuMjY2djIuODMyQTEuMTMzLDEuMTMzLDAsMCwwLDQ3LDQxLjA3N2gzLjRhMS4xMzMsMS4xMzMsMCwwLDAsMS4xMzMtMS4xMzNWMzcuMTEyYTIuODIyLDIuODIyLDAsMCwwLTEuMTUtMi4yNjYsMi44MTksMi44MTksMCwwLDAtLjU1LTQuODU4VjIzLjY1NmwyLjMwNy0uOTc2YTMuNCwzLjQsMCwwLDAsMC02LjI2MVpNNDkuMjY4LDMyLjU4YS41NjYuNTY2LDAsMSwxLS41NjYtLjU2NkEuNTY2LjU2NiwwLDAsMSw0OS4yNjgsMzIuNThabTAsNi4yMzFINDguMTM1di0xLjdhLjU2Ni41NjYsMCwwLDEsMS4xMzMsMFpNNDEuOSwzNi45YTMuMzM5LDMuMzM5LDAsMCwxLTEuNiwyLjg5LDI2LjAyMiwyNi4wMjIsMCwwLDEtMjYuMjUxLDAsMy4zMzksMy4zMzksMCwwLDEtMS42LTIuODlWMjcuMDEybDEyLjUyMiw1LjNhNS42OTIsNS42OTIsMCwwLDAsNC40MTksMGwxMi41MTctNS4zWm05LjM1NS0xNi4zMDZMMjguNSwzMC4yMjRhMy40MjEsMy40MjEsMCwwLDEtMi42NDksMGwtMjIuNzYtOS42M2ExLjEzMywxLjEzMywwLDAsMSwwLTIuMDg3bDIyLjc2LTkuNjNhMy40MTQsMy40MTQsMCwwLDEsMi42NDksMGwyMi43Niw5LjYzYTEuMTMzLDEuMTMzLDAsMCwxLDAsMi4wODdaIiB0cmFuc2Zvcm09InRyYW5zbGF0ZSgtMC4xMjkgLTYuMzQ0KSIvPg0KICA8L2c+DQo8L3N2Zz4NCg==");
    this.staffLogo = this.domSanitizer.bypassSecurityTrustUrl("data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSI0MS41MTIiIGhlaWdodD0iNDkuODkxIiB2aWV3Qm94PSIwIDAgNDEuNTEyIDQ5Ljg5MSI+DQogIDxnIGlkPSJpY29uLXN0YWZmIiB0cmFuc2Zvcm09InRyYW5zbGF0ZSgwIDApIj4NCiAgICA8cGF0aCBpZD0iUGF0aF8xMjI3IiBkYXRhLW5hbWU9IlBhdGggMTIyNyIgZD0iTTMzLjMzNywyOS45NGExMi40NywxMi40NywwLDEsMC0xMi40Ny0xMi40NywxMi40NywxMi40NywwLDAsMCwxMi40NywxMi40N1ptMC0yMi4xNjlhOS43LDkuNywwLDEsMS05LjcsOS43LDkuNyw5LjcsMCwwLDEsOS43LTkuN1pNMTMuOTM5LDU0Ljg4YTEuMzg2LDEuMzg2LDAsMCwwLDEuMzg2LTEuMzg2VjQ3Ljk1MmExNy45NjIsMTcuOTYyLDAsMCwxLDguMjE5LTE1LjEwOEwzMi4xLDQ5Ljk1OWExLjM5MSwxLjM5MSwwLDAsMCwyLjQ4MywwbDguNTUyLTE3LjEyYTE3LjUsMTcuNSwwLDAsMSwyLjk1NCwyLjM3OEExNy44OSwxNy44OSwwLDAsMSw1MS4zNiw0Ny45NjR2NS41NDJhMS4zODYsMS4zODYsMCwxLDAsMi43NzEsMFY0Ny45NjRBMjAuNzQ1LDIwLjc0NSwwLDAsMCw0My4yMjQsMjkuNjc0bC0uMTExLS4wMzlhMS4zLDEuMywwLDAsMC0uMTMzLS4wNS41NTQuNTU0LDAsMCwwLS4xMzMtLjAyOGgtLjRsLS4xMzMuMDI4aC0uMTIyYTEuMjA4LDEuMjA4LDAsMCwwLS4xMjcuMDVsLS4xMjIuMDU1LS4xMTEuMDcyLS4xMTEuMDc4LS4wOTQuMDk0YS44LjgsMCwwLDAtLjA5NC4xLjg0OC44NDgsMCwwLDAtLjA3OC4xMTYuNjI2LjYyNiwwLDAsMC0uMDY3LjFsLTMuMjcsNi40MzVMMzcuNDc3LDMyLjVhMS4zOCwxLjM4LDAsMCwwLTEuMzY5LTEuMTc1SDMwLjU2NkExLjM4LDEuMzgsMCwwLDAsMjkuMiwzMi41bC0uNjM3LDQuMTg0LTMuMi02LjQxOGgwYS42NTQuNjU0LDAsMCwwLS4wNzItLjEuNzU5Ljc1OSwwLDAsMC0uMDcyLS4xMTFsLS4xMjItLjExNi0uMDY3LS4xMTZhLjYyNi42MjYsMCwwLDAtLjExNi0uMDgzLjY3Ni42NzYsMCwwLDAtLjEwNS0uMDY3bC0uMTI3LS4wNjFjLS4wMzksMC0uMDcyLS4wMzMtLjExNi0uMDQ0YTEuMTY0LDEuMTY0LDAsMCwwLS4xMzMtLjAzM2gtLjUzOGwtLjEyMi4wMjhhMS40ODEsMS40ODEsMCwwLDAtLjE0NC4wNWwtLjEuMDM5YTIwLjc2NywyMC43NjcsMCwwLDAtMTAuOSwxOC4yODl2NS41NDJhMS4zODYsMS4zODYsMCwwLDAsMS4zMTksMS40Wk0zMC43MSw0MC45OCwzMS43NTcsMzQuMWgzLjE1OWwxLjA0Nyw2Ljg4My0yLjYyNyw1LjI2Wk00Ni41LDQzLjhhMS4zODYsMS4zODYsMCwxLDEsMCwyLjc3MUg0Mi4zNDNhMS4zODYsMS4zODYsMCwwLDEsMC0yLjc3MVoiIHRyYW5zZm9ybT0idHJhbnNsYXRlKC0xMi42MiAtNSkiLz4NCiAgPC9nPg0KPC9zdmc+DQo=");
    this.parentLogo = this.domSanitizer.bypassSecurityTrustUrl("data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSI0OC4wODgiIGhlaWdodD0iNjAuMDE0IiB2aWV3Qm94PSIwIDAgNDguMDg4IDYwLjAxNCI+DQogIDxnIGlkPSJpY29uLXBhcmVudCIgdHJhbnNmb3JtPSJ0cmFuc2xhdGUoMCkiPg0KICAgIDxwYXRoIGlkPSJQYXRoXzEyMjgiIGRhdGEtbmFtZT0iUGF0aCAxMjI4IiBkPSJNMjg3LjQ4MywyMTcuMjczbC0uMDA4LS4wMDgtNC41NjYtMy45NWE0LjY0MSw0LjY0MSwwLDAsMS0uOTg4LTEuMjE1LDMyLjM1MiwzMi4zNTIsMCwwLDAtOC4wMTktOS40NTlsLS4wMDYsMC0uMDA2LDBhNS42LDUuNiwwLDAsMC04LjMyNiwxLjY2NSwxNy43NTcsMTcuNzU3LDAsMCwwLTcuMTg3LDIuNiwzLjUxOSwzLjUxOSwwLDAsMCwxLjg2Myw2LjUxMywzLjQ5MSwzLjQ5MSwwLDAsMCwxLjctLjQ0MWMuMDM0LS4wMTguMDY3LS4wMzYuMS0uMDU3YTYuMzM1LDYuMzM1LDAsMCwxLDUuNzY0LS42NTFBMTguMzQ3LDE4LjM0NywwLDAsMSwyNTUuMywyMjMuM2E4LjcsOC43LDAsMCwwLTUuMDMsMTMuMjcsMjIuNTgzLDIyLjU4MywwLDAsMSwzLjg1MiwxMi4wODIsMi44LDIuOCwwLDAsMCwyLjc4MSwyLjcxNmg4Ljk0OWEyLjc4MywyLjc4MywwLDAsMCwyLjc3Ny0yLjk3N2MtLjA1OS0uODM2LS4xMjctMS43MzQtLjIzLTIuNjM4YTM3LjA4MSwzNy4wODEsMCwwLDAtNS42NzMtMTYuNDc5LDI4LjMsMjguMywwLDAsMCwxMC05LjQ1NiwxMy42NjIsMTMuNjYyLDAsMCwxLC4wNzUsNS42NDMuMDE1LjAxNSwwLDAsMCwwLC4wMDZ2MGEyLjgwNiwyLjgwNiwwLDAsMCwuNzIyLDIuNDA3LDQuMDc4LDQuMDc4LDAsMCwwLDIuOTY5LDEuMzI5Yy4wNjcsMCwuMTM1LDAsLjItLjAwNmEzLjcwNywzLjcwNywwLDAsMCwzLjQ5LTMuODE4di0uMDM0bC0uMjgxLTQuODIxLDIuNTc5LDIuNDA3LjAxOS4wMTdhMy43OCwzLjc4LDAsMSwwLDQuOTg5LTUuNjc3Wm0tMjcuMDI2LDEwLjMtLjAxLDBhMS40NjIsMS40NjIsMCwwLDAtLjczOS45MjksMS40ODEsMS40ODEsMCwwLDAsLjIwOCwxLjE3OEEzNC41MzUsMzQuNTM1LDAsMCwxLDI2NS44MzcsMjQ2di4wMTRhLjEuMSwwLDAsMCwwLC4wMTRjLjEuODU1LjE2MSwxLjcyMy4yMiwyLjUzMmEuMjEzLjIxMywwLDAsMS0uMjEzLjIyOUgyNTYuOWEuMjEzLjIxMywwLDAsMS0uMjE0LS4yMDksMjUuMTUzLDI1LjE1MywwLDAsMC00LjI4NS0xMy40NTQsNi4xMzMsNi4xMzMsMCwwLDEsMy41NDktOS4zNDksMjAuOTc2LDIwLjk3NiwwLDAsMCwxNC42NzEtMTMuNzQ5aDB2MGExLjM2NCwxLjM2NCwwLDEsMSwyLjU4Ni44NDhBMjUuNTQ0LDI1LjU0NCwwLDAsMSwyNjAuNDU3LDIyNy41NjlabTI1Ljc0Mi03LjJhMS4yMTIsMS4yMTIsMCwwLDEtMiwuNjc2bC00LjYwNS00LjNhMS40NSwxLjQ1LDAsMCwwLTIuNDMxLDEuMTVsLjQ0MSw3LjU3OGExLjEzMywxLjEzMywwLDAsMS0xLjA3MiwxLjE2OCwxLjUsMS41LDAsMCwxLTEuMS0uNDU4LjM5LjM5LDAsMCwxLS4xMjEtLjI0MSwxNi4xMjksMTYuMTI5LDAsMCwwLS44OTItOS4yMjIsMjguMTg0LDI4LjE4NCwwLDAsMCwxLjIxMy0zLjAzMywzLjkyNiwzLjkyNiwwLDAsMC02Ljc0MS0zLjc1OSw4Ljg1Niw4Ljg1NiwwLDAsMC04LjE5My43ODYuOTU3Ljk1NywwLDAsMS0xLjMtLjMzNi45NzIuOTcyLDAsMCwxLC4zNDMtMS4zMTUsMTUuMSwxNS4xLDAsMCwxLDYuNjctMi4yNzdjLjA0NCwwLC4wODctLjAxLjEyOS0uMDE4YTEuNDQ0LDEuNDQ0LDAsMCwwLDEuMDc4LS44NjUsMy4wMzcsMy4wMzcsMCwwLDEsNC42ODMtMS4yNDcsMjkuODA2LDI5LjgwNiwwLDAsMSw3LjM3MSw4LjcwNiw3LjIwNyw3LjIwNywwLDAsMCwxLjUzMywxLjg4NWw0LjU3LDMuOTU0aDBBMS4yMTMsMS4yMTMsMCwwLDEsMjg2LjIsMjIwLjM2NVoiIHRyYW5zZm9ybT0idHJhbnNsYXRlKC0yNDAuNzY4IC0xOTEuMzUxKSIvPg0KICAgIDxwYXRoIGlkPSJQYXRoXzEyMjkiIGRhdGEtbmFtZT0iUGF0aCAxMjI5IiBkPSJNMTYwLjkxMSwzMzIuMTMxYTYuMDMxLDYuMDMxLDAsMSwwLTYuMDMsNi4wM0E2LjAzMSw2LjAzMSwwLDAsMCwxNjAuOTExLDMzMi4xMzFabS02LjAzMSwzLjQ2MmEzLjQ2MywzLjQ2MywwLDEsMSwzLjQ2My0zLjQ2MkEzLjQ2MywzLjQ2MywwLDAsMSwxNTQuODgxLDMzNS41OTNaIiB0cmFuc2Zvcm09InRyYW5zbGF0ZSgtMTQ4Ljg1IC0zMDYuMDAzKSIvPg0KICAgIDxwYXRoIGlkPSJQYXRoXzEyMzAiIGRhdGEtbmFtZT0iUGF0aCAxMjMwIiBkPSJNMzgyLjU0OSw4NS45NTlhNS4xNDksNS4xNDksMCwxLDAtNS4xNDktNS4xNDlBNS4xNDksNS4xNDksMCwwLDAsMzgyLjU0OSw4NS45NTlabTAtNy43MzFhMi41ODIsMi41ODIsMCwxLDEtMi41ODIsMi41ODIsMi41ODIsMi41ODIsMCwwLDEsMi41ODItMi41ODJaIiB0cmFuc2Zvcm09InRyYW5zbGF0ZSgtMzU5LjA2IC03NS42NikiLz4NCiAgPC9nPg0KPC9zdmc+DQo=");
  }

  ngOnInit(): void {
    this.getAllSchoolModel.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    this.callAllSchool();
    this.searchCtrl = new FormControl();
  }

  ngAfterViewInit() {
    // For searching
    this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term != '') {
        let filterParams = [
          {
            columnName: null,
            filterValue: term,
            filterOption: 3
          }
        ];
        Object.assign(this.getAllSchoolModel, { filterParams: filterParams });
        this.getAllSchoolModel.pageNumber = 1;
        this.paginator.pageIndex = 0;
        this.getAllSchoolModel.pageSize = this.pageSize;
        this.callAllSchool();
      } else {
        Object.assign(this.getAllSchoolModel, { filterParams: null });
        this.getAllSchoolModel.pageNumber = this.paginator.pageIndex + 1;
        this.getAllSchoolModel.pageSize = this.pageSize;
        this.callAllSchool();
      }
    });
  }

  // For get all school list
  callAllSchool() {
    if (this.getAllSchoolModel.sortingModel?.sortColumn == "") {
      this.getAllSchoolModel.sortingModel = null
    }
    this.getAllSchoolModel.emailAddress = this.defaultValuesService.getEmailId();
    this.schoolService.GetAllSchoolList(this.getAllSchoolModel).subscribe(data => {
      if (data) {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          if (data.schoolMaster === null) {
            this.instituteList = new MatTableDataSource([]);
            this.totalCount = null;
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          } else {
            this.instituteList = new MatTableDataSource([]);
            this.totalCount = null;
          }
        } else {
          this.totalCount = data.totalCount;
          this.pageNumber = data.pageNumber;
          this.pageSize = data._pageSize;
          data.schoolMaster.forEach((school) => {
            school.checked = false;
          });
          this.listOfSchools = data.schoolMaster.map((item) => {
            this.selectedSchools.map((selectedSchool) => {
              if (item.schoolId === selectedSchool.schoolId) {
                item.checked = true;
                return item;
              }
            });
            return item;
          });
          this.masterCheckBox.checked = this.listOfSchools.every((item) => {
            return item.checked;
          })
          this.instituteList = new MatTableDataSource(data.schoolMaster);
          this.getAllSchoolModel = new GetAllSchoolModel();
        }
      } else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  // For server side pagination
  getPageEvent(event) {
    if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
      let filterParams = [
        {
          columnName: null,
          filterValue: this.searchCtrl.value,
          filterOption: 3
        }
      ];
      Object.assign(this.getAllSchoolModel, { filterParams: filterParams });
    }
    this.getAllSchoolModel.pageNumber = event.pageIndex + 1;
    this.getAllSchoolModel.pageSize = event.pageSize;
    this.defaultValuesService.setPageSize(event.pageSize);
    this.callAllSchool();
  }

  someComplete(): boolean {
    let indetermine = false;
    for (let school of this.listOfSchools) {
      for (let selectedSchool of this.selectedSchools) {
        if (school.schoolId === selectedSchool.schoolId) {
          indetermine = true;
        }
      }
    }
    if (indetermine) {
      this.masterCheckBox.checked = this.listOfSchools.every((item) => {
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
    this.listOfSchools.forEach(school => { school.checked = event; });
    this.instituteList = new MatTableDataSource(this.listOfSchools);
    this.decideCheckUncheck();
  }

  onChangeSelection(eventStatus: boolean, schoolId) {
    for (let item of this.listOfSchools) {
      if (item.schoolId === schoolId) {
        item.checked = eventStatus;
        break;
      }
    }
    this.instituteList = new MatTableDataSource(this.listOfSchools);
    this.masterCheckBox.checked = this.listOfSchools.every((item) => {
      return item.checked;
    });
    this.decideCheckUncheck();
  }

  decideCheckUncheck() {
    this.listOfSchools.map((item) => {
      let isIdIncludesInSelectedList = false;
      if (item.checked) {
        for (let selectedSchool of this.selectedSchools) {
          if (item.schoolId === selectedSchool.schoolId) {
            isIdIncludesInSelectedList = true;
            break;
          }
        }
        if (!isIdIncludesInSelectedList) {
          this.selectedSchools.push(item);
        }
      } else {
        for (let selectedSchool of this.selectedSchools) {
          if (item.schoolId === selectedSchool.schoolId) {
            this.selectedSchools = this.selectedSchools.filter((school) => school.schoolId !== item.schoolId);
            break;
          }
        }
      }
      isIdIncludesInSelectedList = false;
    });
    this.selectedSchools = this.selectedSchools.filter((item) => item.checked);
  }

  generateInstituteReport() {
    if (!this.getSchoolReportModel.isGeneralInfo && !this.getSchoolReportModel.isAddressInfo && !this.getSchoolReportModel.isContactInfo && !this.getSchoolReportModel.isWashInfo && !this.getSchoolReportModel.isCustomCategory) {
      this.snackbar.open('Please select any option to generate report.', '', {
        duration: 2000
      });
      return;
    }
    if (this.selectedSchools.length === 0) {
      this.snackbar.open('Please select any school to generate report.', '', {
        duration: 2000
      });
      return;
    }
    this.getSchoolReport().then((res: any) => {
      this.generatedReportCardData = res;
      this.generatedReportCardData?.schoolViewForReports?.map((item: any) => {
        item.schoolMaster.fieldsCategoryForPDF = [];
        item?.schoolMaster?.fieldsCategory?.map(subItem => {
          if (!subItem?.isSystemCategory && subItem?.customFields?.length) {
            item.schoolMaster.fieldsCategoryForPDF.push(subItem);
          }
        });
      });
      this.generatedReportCardData?.schoolViewForReports?.map((item: any) => {
        if (item?.schoolMaster?.fieldsCategoryForPDF?.length) {
          item?.schoolMaster?.fieldsCategoryForPDF.map(subItem => {
            subItem?.customFields?.map(subOfSubItem => {
              if (subOfSubItem?.customFieldsValue?.length) {
                subOfSubItem.customFieldsValueForPDF = subOfSubItem?.customFieldsValue[0].customFieldValue;
              } else if (subOfSubItem?.defaultSelection) {
                subOfSubItem.customFieldsValueForPDF = subOfSubItem?.defaultSelection;
              } else {
                subOfSubItem.customFieldsValueForPDF = null;
              }
            });
          });
        }
      });
      setTimeout(() => {
        this.generatePdf();
      }, 100 * this.generatedReportCardData?.schoolViewForReports?.length);
    });
  }

  getSchoolReport() {
    this.getSchoolReportModel.schoolIds = this.selectedSchools.map((item) => {
      return item.schoolId;
    });
    return new Promise((resolve, reject) => {
      this.reportService.getSchoolReport(this.getSchoolReportModel).subscribe((res: any) => {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
        } else {
          resolve(res);
        }
      })
    });
  }

  generatePdf() {
    let printContents, popupWin;
    printContents = document.getElementById('printReportCardId').innerHTML;
    document.getElementById('printReportCardId').className = 'block';
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
              padding: 20px;
              vertical-align: middle;
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
              padding: 20px;
          }
          .student-logo div {
              width: 100%;
              height: 100%;
              border: 1px solid rgb(136, 136, 136);
              border-radius: 3px;
              overflow: hidden;
          }
          .student-logo img {
              width: 100%;
              overflow: hidden;
          }
          .student-details {
              padding: 0 20px 0 10px;
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
              margin-top: 5px;
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
              padding: 10px 20px;
              box-shadow: none;
              display: flex;
          }
          .p-20 {
              padding: 20px;
          }
          .p-t-0 {
              padding-top: 0px;
          }
          .p-b-8 {
              padding-bottom: 8px;
          }
          .width-140 {
              width: 140px;
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
          .p-t-15 {
              padding-top: 15;
          }
          .bg-slate {
              background-color: #E5E5E5;
          }
          .information-table td {
              font-size: 16px;
          }
          .information-table tr:first-child td:first-child {
              border-top-left-radius: 10px;
          }
          .information-table tr:first-child td:last-child {
              border-top-right-radius: 10px;
          }
          table td {
              vertical-align: top;
          }
          .school-details {
              vertical-align: middle;
              padding: 0 20px 0 10px;
          }
          .school-details table td {
              vertical-align: middle;
              width: 33.33%;
              padding-right: 10px;
          }
          .school-details table td:last-child {
              padding-right: 0px;
          }
          .m-t-0 {
              margin-top: 0;
          }
          .student-details table tr:first-child td:first-child {
              border-top-left-radius: 10px;
          }
          .student-details table tr:first-child td:last-child {
              border-top-right-radius: 10px;
          }
          .student-details table tr:last-child td:first-child {
              border-bottom-left-radius: 10px;
          }
          .student-details table tr:last-child td:last-child {
              border-bottom-right-radius: 10px;
          }
    </style>
        </head>
    <body onload="window.print()">${printContents}</body>
      </html>`
    );
    popupWin.document.close();
    document.getElementById('printReportCardId').className = 'hidden';
    return;
  }

  // For destroy the isLoading subject.
  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}
