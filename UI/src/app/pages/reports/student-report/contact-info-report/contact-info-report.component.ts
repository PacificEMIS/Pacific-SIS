import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import icListAlt from '@iconify/icons-ic/twotone-list-alt';
import icSchool from '@iconify/icons-ic/twotone-school';
import { StudentService } from 'src/app/services/student.service';
import { CommonService } from 'src/app/services/common.service';
import { PageRolesPermission } from 'src/app/common/page-roles-permissions.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { LoaderService } from 'src/app/services/loader.service';
import { StudentListByDateRangeModel, StudentListModel } from 'src/app/models/student.model';
import { Permissions } from "../../../../models/roll-based-access.model";
import { debounceTime, distinctUntilChanged, filter } from 'rxjs/operators';
import { FormControl } from '@angular/forms';
import { MatSort } from '@angular/material/sort';
import { MatPaginator, MatPaginatorIntl } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MatSnackBar } from '@angular/material/snack-bar';
import { GetMarkingPeriodTitleListModel } from 'src/app/models/marking-period.model';
import { MarkingPeriodService } from 'src/app/services/marking-period.service';
import { AddReportCardPdf } from "../../../../../app/models/report-card.model";
import { MatCheckbox } from '@angular/material/checkbox';
import { ReportCardService } from 'src/app/services/report-card.service';
import { SearchFilterAddViewModel, SearchFilterListViewModel } from 'src/app/models/search-filter.model';
import { StudentInfoReportModel } from 'src/app/models/student-info-report.model';
import { StudentReportService } from 'src/app/services/student-report.service';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { fadeInRight400ms } from 'src/@vex/animations/fade-in-right.animation';
import { AdvancedSearchExpansionModel } from 'src/app/models/common.model';


@Component({
  selector: 'vex-contact-info-report',
  templateUrl: './contact-info-report.component.html',
  styleUrls: ['./contact-info-report.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms,
    fadeInRight400ms
  ]
})

export class ContactInfoReportComponent implements OnInit, AfterViewInit {

  icListAlt = icListAlt;
  icSchool = icSchool;
  columns = [
    { label: '', property: 'studentCheck', type: 'text', visible: true },
    { label: 'Name', property: 'studentName', type: 'text', visible: true },
    { label: 'Student ID', property: 'studentId', type: 'text', visible: true },
    { label: 'Alternate ID', property: 'alternateId', type: 'text', visible: true },
    { label: 'Grade Level', property: 'gradeLevel', type: 'text', visible: true },
    { label: 'Section', property: 'section', type: 'text', visible: true },
    { label: 'Telephone', property: 'phone', type: 'text', visible: true },
    { label: 'School Name', property: 'schoolName', type: 'text', visible: false },
    { label: 'Status', property: 'status', type: 'text', visible: false }
  ];
  studentInfoReportModel: StudentInfoReportModel = new StudentInfoReportModel();
  displayedColumns: string[] = ['studentCheck', 'studentName', 'studentId', 'alternateId', 'gradeLevel', 'section', 'phone'];
  // studentList = studentListData;
  markingPeriods: any[];
  getAllStudent: StudentListByDateRangeModel = new StudentListByDateRangeModel();
  permissions: Permissions;
  loading: boolean;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  pageSize: number;
  totalCount: number = 0;
  listOfStudents = [];
  selectedStudents = []
  studentModelList: MatTableDataSource<any>;
  pageNumber: number;
  getMarkingPeriodTitleListModel: GetMarkingPeriodTitleListModel = new GetMarkingPeriodTitleListModel();
  advancedSearchExpansionModel: AdvancedSearchExpansionModel = new AdvancedSearchExpansionModel();
  markingPeriodList: any[];
  searchCount: any;
  searchValue: any;
  searchCtrl = new FormControl();
  showAdvanceSearchPanel: boolean = false;
  searchFilter: any;
  toggleValues;
  currentDate = new Date();
  searchFilterAddViewModel: SearchFilterAddViewModel = new SearchFilterAddViewModel();
  searchFilterListViewModel: SearchFilterListViewModel = new SearchFilterListViewModel();
  isFromAdvancedSearch: boolean = false;
  filterParameters = [];



  contacts : any[] = []

  addReportCardPdf: AddReportCardPdf = new AddReportCardPdf();
  @ViewChild('masterCheckBox') private masterCheckBox: MatCheckbox;
  generatedReportCardData: any;



  constructor(
    public translateService: TranslateService,
    private studentService: StudentService,
    private reportCardService: ReportCardService,
    private commonService: CommonService,
    private loaderService: LoaderService,
    private pageRolePermissions: PageRolesPermission,
    private defaultValuesService: DefaultValuesService,
    private snackbar: MatSnackBar,
    private markingPeriodService: MarkingPeriodService,
    private studentReportService: StudentReportService,
    private paginatorObj: MatPaginatorIntl,
    ) { 
    this.advancedSearchExpansionModel.accessInformation = false;
    this.advancedSearchExpansionModel.enrollmentInformation = false;
    this.advancedSearchExpansionModel.searchAllSchools = false;
    this.defaultValuesService.setReportCompoentTitle.next("Student Contact Info")
    paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
    // translateService.use("en");
    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    });

    this.markingPeriods = [];

  }

  ngOnInit(): void {

    this.permissions = this.pageRolePermissions.checkPageRolePermission();
    this.getAllStudent.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;

    // this.getAllMarkingPeriodList();
    this.getAllStudentList();
    this.searchCtrl = new FormControl();


    // this.loaderService.isLoading.subscribe((val) => {
      
    //   this.loading = val;
    // });

  }

  ngAfterViewInit(): void {
    this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term.trim().length > 0) {
        let filterParams = [
          {
            columnName: null,
            filterValue: term,
            filterOption: 3
          }
        ];
        Object.assign(this.getAllStudent, { filterParams: filterParams });
        this.getAllStudent.pageNumber = 1;
        this.paginator.pageIndex = 0;
        this.getAllStudent.pageSize = this.pageSize;
        this.getAllStudentList();
      }
      else {
        Object.assign(this.getAllStudent, { filterParams: null });
        this.getAllStudent.pageNumber = this.paginator.pageIndex + 1;
        this.getAllStudent.pageSize = this.pageSize;
        this.getAllStudentList();
      }
    });
  }

  getToggleValues(event) {
    this.toggleValues = event;
    if (event.inactiveStudents === true) {
      this.columns[8].visible = true;
    } else if (event.inactiveStudents === false) {
      this.columns[8].visible = false;
    }
    if (event.searchAllSchool === true) {
      this.columns[7].visible = true;
    } else if (event.searchAllSchool === false) {
      this.columns[7].visible = false;
    }
  }

  getSearchInput(event) {
    this.searchValue = event;
  }

  /* This is for get all data from the Advanced Search component and then call the API in this page 
  NOTE: We just get the filterParams Array from Search component
  */
  filterData(res) {
    this.filterParameters = res.filterParams;
    this.isFromAdvancedSearch = true;
    this.getAllStudent = new StudentListByDateRangeModel();
    this.getAllStudent.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    if (res) {
      this.getAllStudent.filterParams = res.filterParams;
      this.getAllStudent.includeInactive = res.inactiveStudents;
      this.getAllStudent.searchAllSchool = res.searchAllSchool;
      this.defaultValuesService.sendIncludeInactiveFlag(res.inactiveStudents);
      this.defaultValuesService.sendAllSchoolFlag(res.searchAllSchool);
      this.getAllStudentList();
    }
  }

  getSearchResult(res) {
    this.getAllStudent = new StudentListByDateRangeModel();
    if (res?.totalCount) {
      this.searchCount = res.totalCount;
      this.totalCount = res.totalCount;
    }
    else {
      this.searchCount = 0;
      this.totalCount = 0;
    }
    this.pageNumber = res.pageNumber;
    this.pageSize = res.pageSize;
    if (res && res.studentListViews) {
      res?.studentListViews?.forEach((student) => {
        student.checked = false;
      });
      this.listOfStudents = res.studentListViews.map((item) => {
        this.addReportCardPdf.studentsReportCardViewModelList.map((selectedUser) => {
          if (item.studentId == selectedUser.studentId) {
            item.checked = true;
            return item;
          }
        });
        return item;
      });
      this.masterCheckBox.checked = this.listOfStudents.every((item) => {
        return item.checked;
      })
    }
    this.studentModelList = new MatTableDataSource(res?.studentListViews);
    this.getAllStudent = new StudentListByDateRangeModel();
  }

  getAllStudentList() {
    if (this.getAllStudent.sortingModel?.sortColumn == "") {
      this.getAllStudent.sortingModel = null;
    }
    this.studentService.getAllStudentListByDateRange(this.getAllStudent).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
        if (data.studentListViews === null) {
          this.totalCount = this.isFromAdvancedSearch ? 0 : null;
          this.listOfStudents = [];
          this.studentModelList = new MatTableDataSource(this.listOfStudents);
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
          this.isFromAdvancedSearch = false;
        } else {
          this.listOfStudents = [];
          this.studentModelList = new MatTableDataSource(this.listOfStudents);
          this.totalCount = this.isFromAdvancedSearch ? 0 : null;
          this.isFromAdvancedSearch = false;
        }
      } else {
        this.totalCount = data.totalCount;
        this.pageNumber = data.pageNumber;
        this.pageSize = data._pageSize;
        data.studentListViews.forEach((student) => {
          student.checked = false;
        });

        this.listOfStudents = data.studentListViews.map((item) => {
          this.addReportCardPdf.studentsReportCardViewModelList.map((selectedUser) => {
            if (item.studentId == selectedUser.studentId) {
              item.checked = true;
              return item;
            }
          });
          return item;
        });

        this.masterCheckBox.checked = this.listOfStudents.every((item) => {
          return item.checked;
        })
        this.listOfStudents = data.studentListViews;
        
        this.studentModelList = new MatTableDataSource(this.listOfStudents);
        this.getAllStudent = new StudentListByDateRangeModel();
        this.isFromAdvancedSearch = false;
      }
    });
  }

  // resetStudentList() {
  //   this.searchCount = null;
  //   this.searchValue = null;
  //   this.getAllStudentList();
  // }

  getPageEvent(event) {
    if (this.sort?.active && this.sort.direction) {
      this.getAllStudent.sortingModel.sortColumn = this.sort.active;
      this.getAllStudent.sortingModel.sortDirection = this.sort.direction;
    }

    if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
      let filterParams = [
        {
          columnName: null,
          filterValue: this.searchCtrl.value,
          filterOption: 3
        }
      ]
      Object.assign(this.getAllStudent, { filterParams: filterParams });
    }
    this.getAllStudent.pageNumber = event.pageIndex + 1;
    this.getAllStudent.pageSize = event.pageSize;
    this.getAllStudent.filterParams = this.filterParameters;
    this.defaultValuesService.setPageSize(event.pageSize);
    this.getAllStudentList();
  }

  someComplete(): boolean {
    let indetermine = false;
    for (let user of this.listOfStudents) {
      for (let selectedUser of this.addReportCardPdf.studentsReportCardViewModelList) {
        if (user.studentId === selectedUser.studentId) {
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

  setAll(event) {  
    this.contacts = []
    this.listOfStudents.map(user => {
      user.checked = event;
      if (event) {
        this.addReportCardPdf.studentsReportCardViewModelList.push({ studentId: user.studentId });
        this.contacts.push(user.studentGuid)
      } else {
        this.addReportCardPdf.studentsReportCardViewModelList = [];
        this.contacts = []
      }
    });
    this.studentModelList = new MatTableDataSource(this.listOfStudents);
    this.decideCheckUncheck();
  }

  doCheck(studentId) {
    return this.addReportCardPdf.studentsReportCardViewModelList.findIndex(x => x.studentId === studentId) === -1 ? false : true;
  }

  hideAdvanceSearch(event) {
    this.showAdvanceSearchPanel = false;
  }

  onChangeSelection(eventStatus: boolean, studentId) {
    if (eventStatus) {
      this.addReportCardPdf.studentsReportCardViewModelList.push({ studentId });
      this.listOfStudents.map(item => {
        if (item.studentId === studentId) {
          item.checked = eventStatus;
          this.contacts.push(item.studentGuid)
        }
      });
      


    } else {
      this.addReportCardPdf.studentsReportCardViewModelList.splice(this.addReportCardPdf.studentsReportCardViewModelList.findIndex(x => x.studentId === studentId), 1);
      this.listOfStudents.map(item => {
        if (item.studentId === studentId) {
          
          this.contacts.splice(this.contacts.indexOf(item.studentGuid),1)
        }
      });
    }
    for (let item of this.listOfStudents) {
      if (item.studentId == studentId) {
        item.checked = eventStatus;
        break;
      }
    }
    this.studentModelList = new MatTableDataSource(this.listOfStudents);
    this.masterCheckBox.checked = this.listOfStudents.every((item) => {
      return item.checked;
    });
    this.decideCheckUncheck();
  }

  decideCheckUncheck() {
    this.listOfStudents.map((item) => {
      let isIdIncludesInSelectedList = false;
      if (item.checked) {
        for (let selectedUser of this.addReportCardPdf.studentsReportCardViewModelList) {
          if (item.studentId == selectedUser.studentId) {
            isIdIncludesInSelectedList = true;
            break;
          }
        }
        if (!isIdIncludesInSelectedList) {
          this.addReportCardPdf.studentsReportCardViewModelList.push(item.studentId);
          this.contacts.push(item.studentGuid)
        }
      } else {
        for (let selectedUser of this.addReportCardPdf.studentsReportCardViewModelList) {
          if (item.studentId == selectedUser.studentId) {
            this.addReportCardPdf.studentsReportCardViewModelList = this.addReportCardPdf.studentsReportCardViewModelList.filter((user) => user.studentId != item.studentId);
            break;
          }
        }
      }
      isIdIncludesInSelectedList = false;
    });
    this.selectedStudents = this.selectedStudents.filter((item) => item.checked);
  }

  generateContactInfo() {

    // if (!this.studentInfoReportModel.isGeneralInfo && !this.studentInfoReportModel.isEnrollmentInfo && !this.studentInfoReportModel.isAddressInfo && !this.studentInfoReportModel.isFamilyInfo && !this.studentInfoReportModel.isMedicalInfo && !this.studentInfoReportModel.isComments) {
    //   this.snackbar.open('Please select any option to generate report.', '', {
    //     duration: 2000
    //   });
    //   return;
    // }
    // else if (this.selectedStudents.length === 0) {
    //   this.snackbar.open('Please select any student to generate report.', '', {
    //     duration: 2000
    //   });
    //   return;
    // }

    this.addAndGenerateStudentContactInfo().then((res: any) => {
      this.generatedReportCardData = res;      
      setTimeout(() => {
        this.generatePdf();
      }, 100 * this.generatedReportCardData.schoolMasterData.length);
    });
  }


  addAndGenerateStudentContactInfo() {
    this.studentInfoReportModel.studentGuids = this.contacts
    this.studentInfoReportModel.isGeneralInfo = true
    this.studentInfoReportModel.isEnrollmentInfo = true
    this.studentInfoReportModel.isAddressInfo = true
    this.studentInfoReportModel.isMedicalInfo = true
    this.studentInfoReportModel.isComments = true
    this.studentInfoReportModel.isFamilyInfo = true

    return new Promise((resolve, reject) => {
      this.studentReportService.getStudentInfoReport(this.studentInfoReportModel).subscribe((res) => {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
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
    let printContents, popupWin;
    printContents = document.getElementById('printContactInfo').innerHTML;
    document.getElementById('printContactInfo').className = 'block';
    popupWin = window.open('', '_blank', 'top=0,left=0,height=100%,width=auto');
    if(popupWin === null || typeof(popupWin)==='undefined'){
      document.getElementById('printContactInfo').className = 'hidden';
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
      font-size: 14px;
    }
    .student-report {
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
    .p-y-20 {
        padding-top: 20px;
        padding-bottom: 20px;
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
    }
    .information-table th {
        border-bottom: 1px solid #000;
        padding: 8px 5px;
        text-align: left;
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
    .information-table tr:last-child td {
        border-bottom: none;
    }
    table td {
        vertical-align: top;
    }

    .report-header .header-left {
      width: 68%;
    }
    .report-header .header-right {
      width: 32%;
    }
    .report-header .information {
      width: calc(100% - 110px);
    }
    .information-table tr:last-child td:first-child {
      border-bottom-left-radius: 10px;
    }
    .information-table tr:last-child td:last-child {
        border-bottom-right-radius: 10px;
    }
    .bg-gray {
      background-color: #EAEAEA;
    }
    .radius-5 {
      border-radius: 5px;
    }
    .p-x-8 {
      padding-left: 8px;
      padding-right: 8px;
    }

    </style>
        </head>
    <body onload="window.print()">${printContents}</body>
      </html>`
    );
    popupWin.document.close();
    document.getElementById('printContactInfo').className = 'hidden';
    return;
    }
  }





  // getAllMarkingPeriodList() {
  //   this.getMarkingPeriodTitleListModel.academicYear = this.defaultValuesService.getAcademicYear();
  //   // this.addReportCardPdf.academicYear = this.defaultValuesService.getAcademicYear();

  //   this.markingPeriodService.getAllMarkingPeriodList(this.getMarkingPeriodTitleListModel).subscribe((res) => {
  //     if (res._failure) {
  //       this.commonService.checkTokenValidOrNot(res._message);
  //       this.markingPeriodList = [];
  //       if (!res.getMarkingPeriodView) {
  //         this.snackbar.open(res._message, '', {
  //           duration: 1000
  //         });
  //       }
  //     } else {
  //       this.markingPeriodList = res.getMarkingPeriodView;
  //     }
  //   })
  // }


  
  

}