import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import icPrint from '@iconify/icons-ic/twotone-print';
import icHome from '@iconify/icons-ic/twotone-home';
import icMarkunreadMailbox from '@iconify/icons-ic/twotone-markunread-mailbox';
import icContactPhone from '@iconify/icons-ic/twotone-contact-phone';
import icGrade from '@iconify/icons-ic/twotone-grade';
import { ScheduleReportFilterModel, ScheduleReportFilterViewModel, ScheduleReportStaddAndCourseListFilterModel } from 'src/app/models/schedule-report-filter.model';
import { StudentScheduleService } from 'src/app/services/student-schedule.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatCheckbox } from '@angular/material/checkbox';
import { CommonService } from 'src/app/services/common.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { GetAllStaffModel } from 'src/app/models/staff.model';
import { StaffService } from 'src/app/services/staff.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { MatSort } from '@angular/material/sort';
import { BlockListViewModel } from 'src/app/models/school-period.model';
import { SchoolPeriodService } from 'src/app/services/school-period.service';
import { GetAllCourseListModel } from 'src/app/models/course-manager.model';
import { CourseManagerService } from 'src/app/services/course-manager.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';


// export interface ClassListData {
//   staffCheck: boolean;
//   courseSection: string;
//   teacher: string;
//   subject: string;
//   course: string;
//   noOfAssociatedStudent: number;
// }

// export const classListData: ClassListData[] = [
//   {staffCheck: false , teacher: 'Adriana Martin', courseSection: 'ATTN001', subject: 'Attendance Tracking', course: 'Attendance All', noOfAssociatedStudent: 20},
//   {staffCheck: true , teacher: 'Adriana Martin', courseSection: 'READ001', subject: 'Language Arts', course: 'Reading', noOfAssociatedStudent: 20},
//   {staffCheck: false , teacher: 'James Wiliams', courseSection: 'WRT002', subject: 'Language Arts', course: 'Writing', noOfAssociatedStudent: 20},
//   {staffCheck: false , teacher: 'Ekon Wiliams', courseSection: 'ALG02', subject: 'Mathematics', course: 'Algebra II', noOfAssociatedStudent: 20},
//   {staffCheck: false , teacher: 'Ekon Wiliams', courseSection: 'GEOM1', subject: 'Mathematics', course: 'Geometry', noOfAssociatedStudent: 20},
//   {staffCheck: true , teacher: 'Audre Keita', courseSection: 'BIO101', subject: 'Science', course: 'Biology', noOfAssociatedStudent: 20},
//   {staffCheck: false , teacher: 'Kwame Kimathi', courseSection: 'CHEM101', subject: 'Science', course: 'Chemistry', noOfAssociatedStudent: 20},
//   {staffCheck: false , teacher: 'James Miller', courseSection: 'PHY101', subject: 'Science', course: 'Physice', noOfAssociatedStudent: 20},
// ];

// export interface GenerateListData {
//   staffName: string;
//   staffId: string;
//   gender: string;
//   dateOfBirth: string;
//   firstLanguage: string;
//   mobilePhone: string;
//   fullAddress: string;
// }

// export const generateListData: GenerateListData[] = [
//   {staffName: 'Arthur Boucher', staffId: '12', gender: 'Male', dateOfBirth: 'Aug 10, 1985', firstLanguage: 'English', fullAddress: '2000 Ruth St NW, Atlanta, GA, 303178', mobilePhone: '6754328796'},
//   {staffName: 'Sophia Brown', staffId: '15', gender: 'Female', dateOfBirth: 'Aug 10, 1980', firstLanguage: 'English', fullAddress: '1422 Piedmont Rd NE #C3, Atlanta, GA, 30309', mobilePhone: '8754328796'},
//   {staffName: 'Wang Wang', staffId: '35', gender: 'Male', dateOfBirth: 'Mar 21, 1968', firstLanguage: 'English', fullAddress: '2520 Peachtree Rd #307, Atlanta, GA, 30305', mobilePhone: '9754328796'},
//   {staffName: 'Clare Garcia', staffId: '102', gender: 'Female', dateOfBirth: 'Dec 01, 1982', firstLanguage: 'English', fullAddress: '938 Mathews St SW, Atlanta, GA, 30309', mobilePhone: '5754328796'},
//   {staffName: 'Amelia Jones', staffId: '57', gender: 'Female', dateOfBirth: 'Aug 10, 1976', firstLanguage: 'English', fullAddress: '2269 Plaster Rd NE, Atlanta, GA, 30345', mobilePhone: '8754328796'},
//   {staffName: 'Audre Keita', staffId: '61', gender: 'Male', dateOfBirth: 'Jun 06, 1976', firstLanguage: 'English', fullAddress: '1287 Bookshire Ln NE, Atlanta, GA, 30319', mobilePhone: '9854328796'},
//   {staffName: 'Kwame Kimathi', staffId: '52', gender: 'Male', dateOfBirth: 'Aug 10, 1968', firstLanguage: 'English', fullAddress: '2664 Black Forest Trl SW, Atlanta, GA, 30331', mobilePhone: '3554328796'},
//   {staffName: 'James Miller', staffId: '13', gender: 'Male', dateOfBirth: 'Dec 01, 1985', firstLanguage: 'English', fullAddress: '367 Wilkinson Dr SE, Atlanta, GA, 30317', mobilePhone: '8254328796'},
//   {staffName: 'Olivia Smith', staffId: '97', gender: 'Female', dateOfBirth: 'Aug 09, 1986', firstLanguage: 'English', fullAddress: '2982 Briarcliff Rd NE, Atlanta GA, 30329', mobilePhone: '7554328796'},
//   {staffName: 'Amelia Jones', staffId: '67', gender: 'Female', dateOfBirth: 'Aug 10, 1990', firstLanguage: 'English', fullAddress: '226 Plaster Rd NE, Atlanta, GA, 30315', mobilePhone: '9854328796'},
// ];

@Component({
  selector: 'vex-class-list',
  templateUrl: './class-list.component.html',
  styleUrls: ['./class-list.component.scss']
})
export class ClassListComponent implements OnInit, AfterViewInit {
  icPrint = icPrint;
  icHome = icHome;
  icMarkunreadMailbox = icMarkunreadMailbox;
  icContactPhone = icContactPhone;
  icGrade = icGrade;
  currentTab: string = 'selectSTeacher';
  displayedColumns: string[] = ['staffAndCourseCheck', 'courseSection', 'teacher', 'subject', 'course', 'noOfAssociatedStudent'];
  // classLists = classListData;
  classLists: MatTableDataSource<any>;
  displayedColumnsReportList: string[] = ['staffName', 'staffId', 'gender', 'dateOfBirth', 'firstLanguage', 'mobilePhone', 'fullAddress'];
  // generateList = generateListData;
  scheduleReportFilterModel:ScheduleReportFilterModel = new ScheduleReportFilterModel()
  // scheduleReportFilterViewModel:ScheduleReportFilterViewModel = new ScheduleReportFilterViewModel()
  totalCount = 0;
  listOfCourses = [];
  staffAndCourseCheck = []
  staffList = []
  periodList = []
  courseList = []
  subjectList = []
  classListsClone=[];
  form:FormGroup;
  getAllStaff: GetAllStaffModel = new GetAllStaffModel();
  blockListViewModel: BlockListViewModel = new BlockListViewModel();
  getAllCourseListModel: GetAllCourseListModel = new GetAllCourseListModel();
  filteredCourseListModel: ScheduleReportStaddAndCourseListFilterModel = new ScheduleReportStaddAndCourseListFilterModel();
  @ViewChild(MatSort) sort: MatSort
  @ViewChild('masterCheckBox') masterCheckBox: MatCheckbox;

  constructor(
       public translateService: TranslateService,
       private scheduleService: StudentScheduleService,
       private commonService: CommonService,
       private snackbar: MatSnackBar,
       private staffService: StaffService,
       public defaultValuesService: DefaultValuesService,
       private schoolPeriodService: SchoolPeriodService,
       private courseManager: CourseManagerService,
       private fb: FormBuilder,
    ) { 
    translateService.use("en");
    this.getAllStaff.pageSize =  0;
    this.getAllStaff.filterParams = null;
    this.getAllStaff.pageNumber = 1;
    this.getAllStaff.sortingModel = null;
    this.blockListViewModel.isListView=true;
    this.getDropDownData()
  }

  ngOnInit(): void {
    this.form = this.fb.group({
      staff:['all',[Validators.required]],
      period:['all',[Validators.required]],
      subject:['all',[Validators.required]],
      course:['all',[Validators.required]],
    })
  }

  ngAfterViewInit(){
    this.scheduleService.scheduledCourseSectionListForReport(this.scheduleReportFilterModel).subscribe(res=>{
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        if (res.courseSectionViewList === null) {
          this.totalCount = null;
          this.classLists = new MatTableDataSource([]);
          this.classListsClone = []
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
        } else {
          this.classLists = new MatTableDataSource([]);
          this.classListsClone = []
          this.totalCount = null;
        }
      } else {
        this.classListsClone = res?.courseSectionViewList
        this.totalCount = res.courseSectionViewList.length;
        this.classLists = new MatTableDataSource(this.classListsClone);
        this.checkUncheckOnFilteredData()
      }
    })
  }

  checkUncheckOnFilteredData() {
    this.classListsClone.forEach((item) => {
      item.checked = false;
    });
    this.listOfCourses = this.classListsClone.map((item) => {
      this.staffAndCourseCheck.map((selectedUser) => {
        if (item.courseSectionId == selectedUser.courseSectionId) {
          item.checked = true;
          return item;
        }
      });
      return item;
    });
    this.masterCheckBox.checked = this.listOfCourses.every((item) => {
      return item.checked;
    })
  }

  changeTab(status) {
    this.currentTab = status;
  }

  getDropDownData() {
    this.staffService.getAllStaffList(this.getAllStaff).subscribe(res => {
        res.staffMaster.forEach(element=>{
          this.staffList.push({staffId:element.staffId, staffName:`${element.firstGivenName} ${element.middleName?element.middleName:''}${element.lastFamilyName}`})
        })
    })
    this.schoolPeriodService.getAllBlockList(this.blockListViewModel).subscribe((res: BlockListViewModel) => {
      res.getBlockListForView.forEach(element=>{
        element.blockPeriod.forEach(index=>{
            this.periodList.push({periodId:index.periodId, periodTitle:index.periodTitle})
        })
      })
    })
    this.courseManager.GetAllCourseList(this.getAllCourseListModel).subscribe(res => {
      res.courseViewModelList.forEach((element:any)=>{
        this.subjectList.push({courseId:element.course.courseId, courseSubject:element.course.courseSubject})
        this.courseList.push({courseId:element.course.courseId, courseTitle:element.course.courseTitle})
      })
    })
  }

  someComplete(): boolean {
    let indetermine = false;
    for (let user of this.listOfCourses) {
      for (let selectedUser of this.staffAndCourseCheck) {
        if (user.courseSectionId == selectedUser.courseSectionId) {
          indetermine = true;
        }
      }
    }
    if (indetermine) {
      this.masterCheckBox.checked = this.listOfCourses.every((item) => {
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
    this.listOfCourses.forEach(user => { user.checked = event; });
    this.classLists = new MatTableDataSource(this.listOfCourses);
    this.decideCheckUncheck();
  }

  onChangeSelection(eventStatus: boolean, id) {
    for (let item of this.listOfCourses) {
      if (item.courseSectionId == id) {
        item.checked = eventStatus;
        break;
      }
    }
    this.classLists = new MatTableDataSource(this.listOfCourses);
    this.masterCheckBox.checked = this.listOfCourses.every((item) => {
      return item.checked;
    });
    this.decideCheckUncheck();
  }

  decideCheckUncheck() {
    this.listOfCourses.map((item) => {
      let isIdIncludesInSelectedList = false;
      if (item.checked) {
        for (let selectedUser of this.staffAndCourseCheck) {
          if (item.courseSectionId == selectedUser.courseSectionId) {
            isIdIncludesInSelectedList = true;
            break;
          }
        }
        if (!isIdIncludesInSelectedList) {
          this.staffAndCourseCheck.push(item);
        }
      } else {
        for (let selectedUser of this.staffAndCourseCheck) {
          if (item.courseSectionId == selectedUser.courseSectionId) {
            this.staffAndCourseCheck = this.staffAndCourseCheck.filter((user) => user.courseSectionId != item.courseSectionId);
            break;
          }
        }
      }
      isIdIncludesInSelectedList = false;
    });
    this.staffAndCourseCheck = this.staffAndCourseCheck.filter((item) => item.checked);
  }

  onSubjectChange(subjectValue){
    this.filteredCourseListModel.subject=subjectValue;
    this.filterCourse(this.filteredCourseListModel);
   }
   onStaffChange(staffId){
    this.filteredCourseListModel.staffId=staffId;
    this.filterCourse(this.filteredCourseListModel);
   }
   onPeriodChange(periodId){
    this.filteredCourseListModel.periodId=periodId;
    this.filterCourse(this.filteredCourseListModel);
   }
   onCourseChange(course){
    this.filteredCourseListModel.course=course.courseTitle;
    this.filteredCourseListModel.courseId=course.courseId;
    this.filterCourse(this.filteredCourseListModel);
   }

   filterCourse(filter:ScheduleReportStaddAndCourseListFilterModel){
    if(filter.subject=='all' && filter.courseId=='all' && filter.periodId=='all' && filter.course=='all'){
      this.classLists=new MatTableDataSource(this.classListsClone);
    }else{
      this.scheduleReportFilterModel.courseSubject=filter.subject=='all'?null:filter.subject;
      this.scheduleReportFilterModel.courseProgram=filter.course=='all'?null:filter.course;
      this.scheduleReportFilterModel.courseId=filter.courseId=='all'?null:filter.courseId;
      this.scheduleReportFilterModel.staffId=filter.staffId=='all'?null:filter.staffId;
      this.scheduleReportFilterModel.blockPeriodId=filter.periodId=='all'?null:filter.periodId;

       this.scheduleService.scheduledCourseSectionListForReport(this.scheduleReportFilterModel).subscribe(res=>{
        this.classListsClone = res?.courseSectionViewList
        this.classLists=new MatTableDataSource(this.classListsClone)
        this.totalCount=this.classListsClone.length
        this.checkUncheckOnFilteredData()
       })
    }
   }
}
