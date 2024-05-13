import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatPaginator, MatPaginatorIntl } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap, takeUntil } from 'rxjs/operators';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { GetStudentAbsenceReport, GetStudentAbsenceReportForSearch, StudentListForAbsenceSummary } from 'src/app/models/absence-summary.model';
import { GetAllAttendanceCodeModel, GetStudentAttendanceReport } from 'src/app/models/attendance-code.model';
import { CourseManagerService } from 'src/app/services/course-manager.service';
import { GetAllCourseListModel } from 'src/app/models/course-manager.model';
import { ScheduledCourseSectionsForStaffModel } from 'src/app/models/staff.model';
import { GetAllGradeLevelsModel } from 'src/app/models/grade-level.model';
import { MarkingPeriodListModel } from 'src/app/models/marking-period.model';
import { LoaderService } from 'src/app/services/loader.service';
import { MarkingPeriodService } from 'src/app/services/marking-period.service';
import { SharedFunction } from 'src/app/pages/shared/shared-function';
import { CommonService } from 'src/app/services/common.service';
import { SchoolPeriodService } from 'src/app/services/school-period.service';
import { StaffService } from 'src/app/services/staff.service';
import { BlockListViewModel } from 'src/app/models/school-period.model';
import { ReportService } from 'src/app/services/report.service';
import { ExcelService } from 'src/app/services/excel.service';
import { AdvancedSearchExpansionModel } from 'src/app/models/common.model';
import {MomentDateAdapter, MAT_MOMENT_DATE_ADAPTER_OPTIONS} from '@angular/material-moment-adapter';
import {DateAdapter, MAT_DATE_FORMATS, MAT_DATE_LOCALE} from '@angular/material/core';
import * as _moment from 'moment';
import {default as _rollupMoment} from 'moment';
import { MY_FORMATS } from 'src/app/pages/shared/format-datepicker';
import { ProfilesTypes } from 'src/app/enums/profiles.enum';

const moment = _rollupMoment || _moment;


@Component({
  selector: 'vex-absence-summary',
  templateUrl: './absence-summary.component.html',
  styleUrls: ['./absence-summary.component.scss'],
  providers: [
    {
      provide: DateAdapter,
      useClass: MomentDateAdapter,
      deps: [MAT_DATE_LOCALE, MAT_MOMENT_DATE_ADAPTER_OPTIONS],
    },
    
    {provide: MAT_DATE_FORMATS, useValue: MY_FORMATS},
  ]
})
export class AbsenceSummaryComponent implements OnInit, AfterViewInit, OnDestroy {
  getStudentAbsenceReportModel: GetStudentAbsenceReportForSearch = new GetStudentAbsenceReportForSearch();
  displayedColumns: string[] = [];
  columns: string[] = ['studentName', 'studentId', 'alternateId', 'grade', 'phone'];
  studentLists = [];
  parentData :any ={
    markingPeriodStartDate: null,
    markingPeriodEndDate: null,
    periodId: null
  };
  getAllCourseListModel: GetAllCourseListModel = new GetAllCourseListModel();
  blockListViewModel: BlockListViewModel = new BlockListViewModel();
  scheduledCourseSectionModel: ScheduledCourseSectionsForStaffModel = new ScheduledCourseSectionsForStaffModel();
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  searchCtrl: FormControl;
  totalCount: number = 0;
  pageSize: number;
  allAttendence: MatTableDataSource<any>;
  pageNumber: number;
  profiles=ProfilesTypes;
  studentListForAbsenceSummary: StudentListForAbsenceSummary = new StudentListForAbsenceSummary();
  getAllAttendanceCodeModel: GetAllAttendanceCodeModel = new GetAllAttendanceCodeModel();
  advancedSearchExpansionModel: AdvancedSearchExpansionModel = new AdvancedSearchExpansionModel();
  isFromAdvancedSearch: boolean = false;
  isVisible: boolean = false;
  destroySubject$: Subject<void> = new Subject();
  loading: boolean;
  selectedReportBy: any;
  course: any;
  courseSection: any;
  globalMarkingPeriodEndDate;
  globalMarkingPeriodStartDate;
  attendanceListForColumn = [];
  filterJsonParams;
  showAdvanceSearchPanel: boolean = false;
  allStudentlist: MatTableDataSource<any> = new MatTableDataSource<any>();
  searchValue: any;
  toggleValues: any;
  searchCount: number;
  disabledAdvancedSearch: boolean = false;
  selectOptions: any = [
    {
      title: 'This School Year',
      subTitle: 'this_school_year'
    },
    {
      title: 'This Month',
      subTitle: 'this_school_month'
    },
  ];
  tempPeriod = '';
  gradeLevelList = [];
  periodList = [];
  courses = [];
  originalCourseSectionList = [];
  courseSections = [];
  markingPeriodListModel: MarkingPeriodListModel = new MarkingPeriodListModel();
  studentAbsenceListSubject$: Subject<void> = new Subject();
  incomingCustomFieldFilter=[];
  backlinkUsed: boolean = false;
  membershipType = this.defaultValuesService.getUserMembershipType();
  constructor(
    public translateService: TranslateService,
    private router: Router,
    private commonFunction: SharedFunction,
    private commonService: CommonService,
    private paginatorObj: MatPaginatorIntl,
    private reportService: ReportService,
    private excelService: ExcelService,
    private schoolPeriodService: SchoolPeriodService,
    private snackbar: MatSnackBar,
    public defaultValuesService: DefaultValuesService,
    private loaderService: LoaderService,
    private markingPeriodService: MarkingPeriodService,
    private staffService: StaffService,
    private courseManager: CourseManagerService,
  ) {
    this.advancedSearchExpansionModel.accessInformation = false;
    this.advancedSearchExpansionModel.searchBirthdays = false;
    this.advancedSearchExpansionModel.enrollmentInformation = false;
    this.advancedSearchExpansionModel.includeInactiveStudents = false;
    this.advancedSearchExpansionModel.searchAllSchools = false;
    paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
    this.defaultValuesService.setReportCompoentTitle.next("Absence Summary");
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
    this.selectOptions[0].startDate = this.defaultValuesService.getFullYearStartDate();
    this.selectOptions[0].endDate = this.defaultValuesService.getFullYearEndDate();

    this.selectOptions[1].startDate = moment().startOf('month').format('YYYY-MM-DD');
    this.selectOptions[1].endDate = moment().endOf('month').format('YYYY-MM-DD');
    this.selectedReportBy = this.defaultValuesService.getMarkingPeriodTitle();    
    
    if(this.router.getCurrentNavigation()?.extras?.state){
      this.backlinkUsed = true;
      this.selectedReportBy = this.router.getCurrentNavigation()?.extras?.state?.selectedReportBy;
      // this.getStudentAbsenceReportModel.periodId = this.router.getCurrentNavigation()?.extras?.state?.dropdownValues.periodId;
      this.getStudentAbsenceReportModel.markingPeriodStartDate = this.globalMarkingPeriodStartDate = this.router.getCurrentNavigation()?.extras?.state?.dropdownValues.markingPeriodStartDate;
      this.getStudentAbsenceReportModel.markingPeriodEndDate = this.globalMarkingPeriodEndDate = this.router.getCurrentNavigation()?.extras?.state?.dropdownValues.markingPeriodEndDate;
      this.originalCourseSectionList = this.router.getCurrentNavigation()?.extras?.state?.originalCourseSectionList;
      this.course = this.router.getCurrentNavigation()?.extras?.state?.course;
      this.courseSection = this.router.getCurrentNavigation()?.extras?.state?.courseSection;
      let courseId = { value: this.course };
      this.courseChanged(courseId, this.courseSection);
    }
  }

  ngOnInit(): void {
    // this.getStudentAbsenceReportModel.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    this.getStudentAbsenceReportModel.pageSize = 10;
    this.searchCtrl = new FormControl();

    
    if(this.membershipType === this.profiles.Teacher || this.membershipType === this.profiles.HomeroomTeacher){
      this.getScheduledCourseSectionsForTeacher();
    }
    if(this.membershipType === this.profiles.SuperAdmin || this.membershipType === this.profiles.SchoolAdmin || this.membershipType === this.profiles.AdminAssitant) {
      this.getScheduledCourseSectionsForAdmin();
    }
    this.getMarkingPeriod().then(data => {
      if(this.backlinkUsed){
        this.getStudentAbsenceReport();
      }
    });
    this.getDropDownData();
  }

  ngAfterViewInit() {
    this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term != '') {
        this.callWithFilterValue(term);
      } else {
        this.callWithoutFilterValue()
      }
    });
    this.getStudentAbsenceReportBySearch();    
  }

  // For get all marking period
 getMarkingPeriod() {
  return new Promise((resolve, reject) => {
    this.markingPeriodService.GetMarkingPeriod(this.markingPeriodListModel).subscribe((data: any) => {
      if (data._failure) {
      } else {
        for (let i = 0; i < data.schoolYearsView.length; i++) {
          this.selectOptions.push({
            title: data.schoolYearsView[i]?.title,
            startDate: data.schoolYearsView[i]?.startDate,
            endDate: data.schoolYearsView[i]?.endDate,
            subTitle: data.schoolYearsView[i]?.title
          });
          if (data.schoolYearsView[i].children.length > 0) {
            data.schoolYearsView[i].children.map((item: any) => {
              this.selectOptions.push({
                title: item?.title,
                startDate: item?.startDate,
                endDate: item?.endDate,
                subTitle: item?.title
              });
              if (item.children.length > 0) {
                item.children.map((subItem: any) => {
                  this.selectOptions.push({
                    title: subItem?.title,
                    startDate: subItem?.startDate,
                    endDate: subItem?.endDate,
                    subTitle: subItem?.title
                  });
                  if (subItem.children.length > 0) {
                    subItem.children.map((subOfSubItem: any) => {
                      this.selectOptions.push({
                        title: subOfSubItem?.title,
                        startDate: subOfSubItem?.startDate,
                        endDate: subOfSubItem?.endDate,
                        subTitle: subOfSubItem?.title
                      });
                    });
                  }
                });
              }
            });
          }
        }
        resolve(this.selectOptions);
      }
    });
  });
}

getReportBy(event) {
  if (event.value) {
    const selectedOption = this.selectOptions.filter(x => x.subTitle === event.value);
    this.getStudentAbsenceReportModel.markingPeriodStartDate = this.globalMarkingPeriodStartDate = this.commonFunction.formatDateSaveWithoutTime(selectedOption[0].startDate);
    this.getStudentAbsenceReportModel.markingPeriodEndDate = this.globalMarkingPeriodEndDate = this.commonFunction.formatDateSaveWithoutTime(selectedOption[0].endDate);
  } else {
    this.getStudentAbsenceReportModel.markingPeriodStartDate = this.globalMarkingPeriodStartDate = null;
    this.getStudentAbsenceReportModel.markingPeriodEndDate = this.globalMarkingPeriodEndDate = null;
  }
}

  selectedPeriod(event) {
    this.tempPeriod = event.value;
  }

  callWithoutFilterValue() {
    Object.assign(this.getStudentAbsenceReportModel, { filterParams: null });
    this.getStudentAbsenceReportModel.pageNumber = this.paginator.pageIndex + 1;
    this.getStudentAbsenceReportModel.pageSize = this.pageSize;
    this.studentAbsenceListSubject$.next();
  }

  callWithFilterValue(term) {
    let filterParams = [
      {
        columnName: null,
        filterValue: term,
        filterOption: 4
      }
    ]
    Object.assign(this.getStudentAbsenceReportModel, { filterParams: filterParams });
    this.getStudentAbsenceReportModel.pageNumber = 1;
    this.paginator.pageIndex = 0;
    this.getStudentAbsenceReportModel.pageSize = this.pageSize;
    this.studentAbsenceListSubject$.next();
  }

  getPageEvent(event) {
    if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
      let filterParams = [
        {
          columnName: null,
          filterValue: this.searchCtrl.value,
          filterOption: 3
        }
      ]
      Object.assign(this.getStudentAbsenceReportModel, { filterParams: filterParams });
    }
    this.getStudentAbsenceReportModel.pageNumber = event.pageIndex + 1;
    this.getStudentAbsenceReportModel.pageSize = event.pageSize;
    this.getStudentAbsenceReport();
  }

  onSearch() {
    this.parentData.markingPeriodStartDate = this.globalMarkingPeriodStartDate = this.getStudentAbsenceReportModel.markingPeriodStartDate ? this.commonFunction.formatDateSaveWithoutTime(this.getStudentAbsenceReportModel.markingPeriodStartDate) : null;
    this.parentData.markingPeriodEndDate = this.globalMarkingPeriodEndDate = this.getStudentAbsenceReportModel.markingPeriodEndDate ? this.commonFunction.formatDateSaveWithoutTime(this.getStudentAbsenceReportModel.markingPeriodEndDate) : null;
    this.disabledAdvancedSearch = true;
    // if(this.getStudentAbsenceReportModel.periodId ==='daily'){
    //   this.getStudentAbsenceReportModel.periodId= null;
    // }
    this.parentData.periodId = this.getStudentAbsenceReportModel.periodId;
    if (!this.selectedReportBy) {
      
      this.getStudentAbsenceReportModel.markingPeriodStartDate = this.getStudentAbsenceReportModel.markingPeriodStartDate ? this.commonFunction.formatDateSaveWithoutTime(this.getStudentAbsenceReportModel.markingPeriodStartDate) : null;
      this.getStudentAbsenceReportModel.markingPeriodEndDate = this.getStudentAbsenceReportModel.markingPeriodEndDate ? this.commonFunction.formatDateSaveWithoutTime(this.getStudentAbsenceReportModel.markingPeriodEndDate) : null;
      if (this.getStudentAbsenceReportModel.markingPeriodStartDate && this.getStudentAbsenceReportModel.markingPeriodEndDate) {
        if (this.getStudentAbsenceReportModel.markingPeriodStartDate <= this.getStudentAbsenceReportModel.markingPeriodEndDate) {
          this.getStudentAbsenceReport();
        } else {
          this.snackbar.open("To date value should be greater than from date value", "", {
            duration: 10000
          });
        }
      } else if (!this.getStudentAbsenceReportModel.markingPeriodStartDate && !this.getStudentAbsenceReportModel.markingPeriodEndDate) {
        this.snackbar.open("Choose from date and to date", "", {
          duration: 10000
        });
      } else if ((this.getStudentAbsenceReportModel.markingPeriodStartDate && !this.getStudentAbsenceReportModel.markingPeriodEndDate) || (!this.getStudentAbsenceReportModel.markingPeriodStartDate && this.getStudentAbsenceReportModel.markingPeriodEndDate)) {
        this.snackbar.open("Choose both from date and to date", "", {
          duration: 10000,
        });
      }
    } else {
      if (!this.getStudentAbsenceReportModel.markingPeriodStartDate && !this.getStudentAbsenceReportModel.markingPeriodEndDate) {
        this.selectOptions.map(x => {
          if (x.title === this.selectedReportBy) {
            this.getStudentAbsenceReportModel.markingPeriodStartDate = this.globalMarkingPeriodStartDate = x.startDate
            this.getStudentAbsenceReportModel.markingPeriodEndDate = this.globalMarkingPeriodEndDate = x.endDate
          }
        })
      }
      this.getStudentAbsenceReport();
    }
  }

  getToggleValues(event) {
    this.toggleValues = event;
    if (event.inactiveStudents === true) {
      //this.columns[6].visible = true;
    }
    else if (event.inactiveStudents === false) {
      //this.columns[6].visible = false;
    }
  }
  getSearchInput(event) {
    this.searchValue = event;
  }

  /* This is for get all data from the Advanced Search component and then call the API in this page 
  NOTE: We just get the filterParams Array from Search component
  */
  filterData(res) {
    this.isFromAdvancedSearch = true;
    this.getStudentAbsenceReportModel = new GetStudentAbsenceReportForSearch();
    if (res) {
      if(res.customFieldFilter)
        this.incomingCustomFieldFilter = res.customFieldFilter;
      this.getStudentAbsenceReportModel.customFieldFilter = this.incomingCustomFieldFilter;      
      this.getStudentAbsenceReportModel.filterParams = res.filterParams;
      // this.getStudentAbsenceReportModel.periodId = this.tempPeriod;
      this.getStudentAbsenceReport();
    }
  }

  getSearchResult(res) {
    if (res.totalCount) {
      this.searchCount = res.totalCount;
      this.totalCount = res.totalCount;
    }
    else {
      this.searchCount = 0;
      this.totalCount = 0;
    }
    this.studentLists = res.studendAttendanceList;
    this.pageNumber = res.pageNumber;
    this.pageSize = res._pageSize;
  }

  getDropDownData() {
    this.schoolPeriodService.getAllBlockList(this.blockListViewModel).subscribe((res: BlockListViewModel) => {
      res.getBlockListForView.forEach(element => {
        element.blockPeriod.forEach(index => {
          this.periodList.push({ periodId: index.periodId, periodTitle: index.periodTitle, periodShortName: index.periodShortName, calculateAttendance: index.calculateAttendance })
        })
      })
    })

  }

  exportToExcel(){
    if (!this.totalCount) {
      this.snackbar.open('No record found. Failed to student absence sumary', '', {
        duration: 5000
      });
      return;
    }
      this.getStudentAbsenceReportModel.pageNumber = 0;
      this.getStudentAbsenceReportModel.pageSize = 0;
      this.getStudentAbsenceReportModel.courseSectionId = this.courseSection;
      this.getStudentAbsenceReportModel.membershipType = this.defaultValuesService.getUserMembershipType().toLowerCase();
      this.getStudentAbsenceReportModel.academicYear = this.defaultValuesService.getAcademicYear();
      this.getStudentAbsenceReportModel.markingPeriodStartDate = this.globalMarkingPeriodStartDate;
      this.getStudentAbsenceReportModel.markingPeriodEndDate = this.globalMarkingPeriodEndDate;
      this.reportService.getAllStudentAbsenceList(this.getStudentAbsenceReportModel).subscribe((res: any) => {
      if(res._failure){
          this.snackbar.open('Failed to export students absence summary list.' + res._message, '', {
            duration: 10000
          });
        } else {
          if (res.studendAttendanceList.length > 0) {
            let studentList;
            studentList = res.studendAttendanceList?.map((x) => {
              let a = [];
              res.attendanceCodeList.forEach(element => {
                let findAtt = x.attendanceDetailsViewModels.find(x => x.attendanceCodeId === element.attendanceCode1);
                a = {...a, [element.shortName]: findAtt?.attendanceCount};
              })

              return {
                [this.defaultValuesService.translateKey('studentName')]: x.firstGivenName +" "+ x.lastFamilyName,
                [this.defaultValuesService.translateKey('studentId')]: x.studentInternalId,
                [this.defaultValuesService.translateKey('alternateId')]: x.studentAlternetId,
                [this.defaultValuesService.translateKey('grade')]: x.gradeLevelTitle,
                [this.defaultValuesService.translateKey('phone')]: x.homePhone,
                ...a
              };
            });
            this.excelService.exportAsExcelFile(studentList, 'Students_absence_summary_list')
          } else {
            this.snackbar.open('No records found. Failed to export students absence summary list', '', {
              duration: 5000
            });
          }
        }
      });
  }

  getStudentAbsenceReportBySearch() {
    this.getStudentAbsenceReportModel.courseSectionId = this.courseSection;
    this.getStudentAbsenceReportModel.markingPeriodStartDate = this.globalMarkingPeriodStartDate;
    this.getStudentAbsenceReportModel.markingPeriodEndDate = this.globalMarkingPeriodEndDate;
    this.getStudentAbsenceReportModel.academicYear = this.defaultValuesService.getAcademicYear();
    this.getStudentAbsenceReportModel.membershipType = this.defaultValuesService.getUserMembershipType().toLowerCase();
    this.studentAbsenceListSubject$.pipe(switchMap(() => this.reportService.getAllStudentAbsenceList(this.getStudentAbsenceReportModel))).subscribe((data: any) => {
      if (data) {
        if (data._failure) {
          this.studentLists = data.studendAttendanceList;
          this.totalCount = this.isFromAdvancedSearch ? 0 : null;
          this.isFromAdvancedSearch = false;
        } else {
          this.isVisible = true;
          this.studentListForAbsenceSummary = data;
          this.studentLists = data.studendAttendanceList.map((item) => {
            if(data.attendanceCodeList && data.attendanceCodeList.length>0){
              let attendanceCodeList = data.attendanceCodeList.map((att) => {
                let findAtt = item.attendanceDetailsViewModels.find(x => x.attendanceCodeId === att.attendanceCode1);
                return {
                  attendanceCodeId: att.attendanceCode1,
                  attendanceTitle: att.title,
                  attendanceStateCode: att.stateCode,
                  attendanceShortName: att.shortName,
                  sortOrder: att.sortOrder,
                  attendanceCount: findAtt?findAtt.attendanceCount:0
                }
              })

              return {
                ...item,
                attendanceCodes: attendanceCodeList
              }
            }
          });

          this.attendanceListForColumn = data.attendanceCodeList;
          this.displayedColumns = [];
          let newCols: string[] = [];
          if(data.attendanceCodeList && data.attendanceCodeList.length>0){
            data.attendanceCodeList.forEach(element => {
              newCols.push(element.shortName);
            });
            this.displayedColumns = [...this.columns, ...newCols];
          } else {
            this.displayedColumns = [...this.columns];
          }
          
          this.totalCount = data.totalCount;
          this.pageNumber = data.pageNumber;
          this.pageSize = data._pageSize;
          this.isFromAdvancedSearch = false;
        }
      }
      else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  getStudentAbsenceReport() {
    this.getStudentAbsenceReportModel.courseSectionId = this.courseSection;
    if(this.selectedReportBy){      
      const selectedOption = this.selectOptions.filter(x => x.subTitle === this.selectedReportBy);
      this.getStudentAbsenceReportModel.markingPeriodStartDate = this.globalMarkingPeriodStartDate = this.commonFunction.formatDateSaveWithoutTime(selectedOption[0].startDate);
      this.getStudentAbsenceReportModel.markingPeriodEndDate = this.globalMarkingPeriodEndDate = this.commonFunction.formatDateSaveWithoutTime(selectedOption[0].endDate);
    }
    this.getStudentAbsenceReportModel.academicYear = this.defaultValuesService.getAcademicYear();
    this.getStudentAbsenceReportModel.membershipType = this.defaultValuesService.getUserMembershipType().toLowerCase();
    this.reportService.getAllStudentAbsenceList(this.getStudentAbsenceReportModel).subscribe((data: any) => {
      if (data) {
        this.isVisible = true;
        if (data._failure) {
          this.studentLists = data.studendAttendanceList;
          this.totalCount = this.isFromAdvancedSearch ? 0 : null;
          this.isFromAdvancedSearch = false;
        } else {
          this.studentListForAbsenceSummary = data;
          this.studentLists = data.studendAttendanceList.map((item) => {
            if(data.attendanceCodeList && data.attendanceCodeList.length>0){
              let attendanceCodeList = data.attendanceCodeList.map((att) => {
                let findAtt = item.attendanceDetailsViewModels.find(x => x.attendanceCodeId === att.attendanceCode1);
                return {
                  attendanceCodeId: att.attendanceCode1,
                  attendanceTitle: att.title,
                  attendanceStateCode: att.stateCode,
                  attendanceShortName: att.shortName,
                  sortOrder: att.sortOrder,
                  attendanceCount: findAtt?findAtt.attendanceCount:0
                }
              })

              return {
                ...item,
                attendanceCodes: attendanceCodeList
              }
            }
          });

          console.log('this.studentLists', this.studentLists);
          

          this.attendanceListForColumn = data.attendanceCodeList;
          this.displayedColumns = [];
          let newCols: string[] = [];
          if(data.attendanceCodeList && data.attendanceCodeList.length>0){
            data.attendanceCodeList.forEach(element => {
              newCols.push(element.shortName);
            });
            this.displayedColumns = [...this.columns, ...newCols];
          } else {
            this.displayedColumns = [...this.columns];
          }

          this.totalCount = data.totalCount;
          this.pageNumber = data.pageNumber;
          this.pageSize = data._pageSize;
          this.isFromAdvancedSearch = false;
        }
      }
      else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  getScheduledCourseSectionsForTeacher() {
    this.scheduledCourseSectionModel.staffId = this.defaultValuesService.getUserId();
    this.staffService
      .getScheduledCourseSectionsForStaff(this.scheduledCourseSectionModel)
      .subscribe((res) => {
        if (res) {
        if(res._failure){
        
            this.scheduledCourseSectionModel.courseSectionViewList = [];
            if (!this.scheduledCourseSectionModel.courseSectionViewList) {
              this.snackbar.open(res._message, "", {
                duration: 1000,
              });
            }
            this.courseSections = [];
            this.originalCourseSectionList = [];
            this.courses = [];
          } else {
            this.originalCourseSectionList = res.courseSectionViewList;
            this.courses = this.originalCourseSectionList.map((item) => {
              return {
                courseId: item.courseId,
                courseTitle: item.courseTitle
              }
            });
            this.courses = this.courses.filter((item, i, arr) => arr.findIndex(x => x.courseTitle === item.courseTitle) === i);            
            if(!this.course && this.courses.length > 0){
              this.course = this.courses[0]?.courseId;
              this.courseSections = this.originalCourseSectionList.filter(x => x.courseId === this.courses[0]?.courseId);
            }
            if(!this.courseSection){
              this.courseSection = this.courseSections[0]?.courseSectionId;
            }
          }
        }
      });
  }

  getScheduledCourseSectionsForAdmin(){
    this.courseManager.GetAllCourseList(this.getAllCourseListModel).subscribe(res => {
      if(res._failure){
        this.snackbar.open(res._message, "", {
          duration: 1000,
        });
        this.courseSections = [];
        this.originalCourseSectionList = [];
        this.courses = [];
      } else {
        this.originalCourseSectionList = res.courseViewModelList;
        this.courses = this.originalCourseSectionList.map((item) => {
          return {
            courseId: item.course.courseId,
            courseTitle: item.course.courseTitle
          }
        });
        this.courses = this.courses.filter((item, i, arr) => arr.findIndex(x => x.courseTitle === item.courseTitle) === i);      
        this.courses.unshift({courseId: '', courseTitle: 'All'});
        this.courseSections.unshift({ courseSectionId: '', courseSectionName: 'All' });
        if(!this.course){
          this.course = this.courses?.[0]?.courseId;
        }
        if(!this.courseSection){
          this.courseSection = this.courseSections?.[0]?.courseSectionId;
        }

        if (!this.course && this.courses.length > 0) {
          this.course = this.courses?.[0]?.courseId;
          let filterCourseSectionBySelectedCourse = this.originalCourseSectionList?.find(x => x.course.courseId === this.courses[0]?.courseId);
          this.courseSections = filterCourseSectionBySelectedCourse?.course?.courseSection.map(y => {
            return {
              courseId: y.courseId,
              courseSectionId: y.courseSectionId,
              courseSectionName: y.courseSectionName
            }
          });
          this.courseSection = this.courseSections?.[0]?.courseSectionId;
        }
      }
    })
  }

  courseChanged(event: any, courseSectionId?: any){
    
    if(this.membershipType === this.profiles.Teacher || this.membershipType === this.profiles.HomeroomTeacher){
      this.courseSections = this.originalCourseSectionList?.filter(x => x.courseId === event.value);      
      if(!courseSectionId){
        this.courseSection = this.courseSections?.[0]?.courseSectionId;
      }
    }

    if(this.membershipType === this.profiles.SuperAdmin || this.membershipType === this.profiles.SchoolAdmin || this.membershipType === this.profiles.AdminAssitant) {

      if (!!event.value) {
        let filterCourseSectionBySelectedCourse = this.originalCourseSectionList.find(x => x.course.courseId === event.value);
        this.courseSections = filterCourseSectionBySelectedCourse.course.courseSection.map(y => {
          return {
            courseId: y.courseId,
            courseSectionId: y.courseSectionId,
            courseSectionName: y.courseSectionName
          }
      });
      } else {
        this.courseSections = [{ courseSectionId: '', courseSectionName: 'All' }];
      }
     
      if(!courseSectionId){
        this.courseSection = this.courseSections[0]?.courseSectionId;
      }
    }    
  }



  viewAttendanceSummaryDetails(element) {
    this.router.navigate(['/school', 'reports', 'attendance', 'absence-summary', 'absence-summary-details'], { state: { studentdata: element, dropdownValues: this.getStudentAbsenceReportModel, selectedReportBy: this.selectedReportBy, selectOptions: this.selectOptions, periodList: this.periodList, originalCourseSectionList: this.originalCourseSectionList, course: this.course, courseSection: this.courseSection } });
  }


  showAdvanceSearch() {
    this.showAdvanceSearchPanel = true;
    this.filterJsonParams = null;
  }

  hideAdvanceSearch(event) {
    this.showAdvanceSearchPanel = false;
  }

  ngOnDestroy(): void {
    this.studentAbsenceListSubject$.unsubscribe();
  }

}

