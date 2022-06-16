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

import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
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
import { LoaderService } from 'src/app/services/loader.service';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { GetStudentListByCourseSectionModel } from 'src/app/models/report.model';
import { ReportService } from 'src/app/services/report.service';
import { ExcelService } from 'src/app/services/excel.service';
import moment from 'moment';


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
export class ClassListComponent implements OnInit, AfterViewInit, OnDestroy {
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
  getStudentListByCourseSectionModel: GetStudentListByCourseSectionModel = new GetStudentListByCourseSectionModel();
  @ViewChild(MatSort) sort: MatSort
  @ViewChild('masterCheckBox') masterCheckBox: MatCheckbox;
  destroySubject$: Subject<void> = new Subject();
  loading: boolean;
  selectedFieldsArray = [];
  generateCourseSectionList: any;
  totalStudentCount: number;
  membershipType;

  fieldsDetailsArray = {
    identificationInformation: [
      { label: 'fullName', property: 'fullName', checked: false },
      { label: 'salutation', property: 'salutation', checked: false },
      { label: 'firstGivenName', property: 'firstGivenName', checked: false },
      { label: 'middleName', property: 'middleName', checked: false },
      { label: 'lastFamilyName', property: 'lastFamilyName', checked: false },
      { label: 'suffix', property: 'suffix', checked: false },
      { label: 'preferredCommonName', property: 'preferredCommonName', checked: false },
      { label: 'previousMaidenName', property: 'previousMaidenName', checked: false },
      { label: 'studentId', property: 'studentId', checked: false },
      { label: 'alternateId', property: 'alternateId', checked: false },
      { label: 'districtId', property: 'districtId', checked: false },
      { label: 'stateId', property: 'stateId', checked: false },
      { label: 'admissionNumber', property: 'admissionNumber', checked: false },
      { label: 'rollNumber', property: 'rollNumber', checked: false },
      { label: 'socialSecurityNumber', property: 'socialSecurityNumber', checked: false },
      { label: 'otherGovtIssuedNumber', property: 'otherGovtIssuedNumber', checked: false },
    ],
    demographicInformation: [
      { label: 'dateOfBirth', property: 'dateOfBirth', checked: false },
      { label: 'gender', property: 'gender', checked: false },
      { label: 'race', property: 'race', checked: false },
      { label: 'ethnicity', property: 'ethnicity', checked: false },
      { label: 'maritalStatus', property: 'maritalStatus', checked: false },
      { label: 'countryOfBirth', property: 'countryOfBirth', checked: false },
      { label: 'nationality', property: 'nationality', checked: false },
      { label: 'firstLanguage', property: 'firstLanguage', checked: false },
      { label: 'secondLanguage', property: 'secondLanguage', checked: false },
      { label: 'thirdLanguage', property: 'thirdLanguage', checked: false },
    ],
    schoolEnrollmentInfo: [
      { label: 'rollingRetentionOption', property: 'rollingRetentionOption', checked: false },
      { label: 'section', property: 'section', checked: false },
      { label: 'estimatedGraduationDate', property: 'estimatedGraduationDate', checked: false },
      { label: 'gradeLevel', property: 'gradeLevel', checked: false },
    ],
    addressContact: [
      { label: 'studentsFullHomeAddress', property: 'studentsFullHomeAddress', checked: false },
      { label: 'homeAddressLineOne', property: 'homeAddressLineOne', checked: false },
      { label: 'homeAddressLineTwo', property: 'homeAddressLineTwo', checked: false },
      { label: 'homeAddressCountry', property: 'homeAddressCountry', checked: false },
      { label: 'stateProvinceLocality', property: 'homeAddressState', checked: false },
      { label: 'homeAddressCity', property: 'homeAddressCity', checked: false },
      { label: 'homeAddressZip', property: 'homeAddressZip', checked: false },
      { label: 'busNo', property: 'busNo', checked: false },
      { label: 'busPickup', property: 'busPickup', checked: false },
      { label: 'busDropoff', property: 'busDropoff', checked: false },
    ],
    studentMailingAddress: [
      { label: 'studentsFullMailingAddress', property: 'studentsFullMailingAddress', checked: false },
      { label: 'mailingAddressLineOne', property: 'mailingAddressLineOne', checked: false },
      { label: 'mailingAddressLineTwo', property: 'mailingAddressLineTwo', checked: false },
      { label: 'mailingAddressCountry', property: 'mailingAddressCountry', checked: false },
      { label: 'stateProvinceLocality', property: 'mailingAddressState', checked: false },
      { label: 'mailingAddressCity', property: 'mailingAddressCity', checked: false },
      { label: 'mailingAddressZip', property: 'mailingAddressZip', checked: false },
    ],
    personalContactInformation: [
      { label: 'homePhone', property: 'homePhone', checked: false },
      { label: 'mobilePhone', property: 'mobilePhone', checked: false },
      { label: 'personalEmail', property: 'personalEmail', checked: false },
      { label: 'schoolEmail', property: 'schoolEmail', checked: false },
    ]
  }

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
       private loaderService: LoaderService,
       private reportService: ReportService,
       private excelService: ExcelService
    ) { 
      this.defaultValuesService.setReportCompoentTitle.next("Class Lists");
    // translateService.use("en");
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
    this.membershipType = this.defaultValuesService.getUserMembershipType();
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
    if(this.membershipType === 'Teacher' || this.membershipType === 'Homeroom Teacher') this.scheduleReportFilterModel.staffId = this.defaultValuesService.getUserId();
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

  // changeTab(status) {
  //   this.currentTab = status;
  // }

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
          if(item.scheduledStudentCount !== 0) this.staffAndCourseCheck.push(item);
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
      if(this.membershipType === 'Teacher' || this.membershipType === 'Homeroom Teacher') this.scheduleReportFilterModel.staffId = this.defaultValuesService.getUserId();

       this.scheduleService.scheduledCourseSectionListForReport(this.scheduleReportFilterModel).subscribe(res=>{
        this.classListsClone = res?.courseSectionViewList
        this.classLists=new MatTableDataSource(this.classListsClone)
        this.totalCount=this.classListsClone.length
        this.checkUncheckOnFilteredData()
       })
    }
   }

  generateClassLists() {
    if (this.selectedFieldsArray.length) {
      this.selectedFieldsArray.map(fields => {
        if (fields.property === 'studentName') {
          fields.property = 'fullName';
        }
      });
    }

    let selectedCourseSection = this.staffAndCourseCheck.filter(item => item.scheduledStudentCount !== 0);
    if (selectedCourseSection?.length === 0) {
      this.snackbar.open('Please select any course section to generate report.', '', {
        duration: 2000
      });
      return;
    }

    this.getStudentListByCourseSectionModel.courseIds = selectedCourseSection.map(item => {
      return { courseId: item.courseId, courseSectionId: item.courseSectionId };
    });

    this.reportService.getStudentListByCourseSection(this.getStudentListByCourseSectionModel).subscribe((res: any) => {
      if (res) {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
        } else {
          this.generateCourseSectionList = res?.courseSectionForStaffs;
          this.totalStudentCount = res?.totalStudents;
          if (this.generateCourseSectionList?.length) {
            this.generateCourseSectionList.map(item => {
              if (item.studentLists?.length) {
                item.studentLists?.map(subItem => {
                  const middleName = subItem.studentView?.middleName ? ' ' + subItem.studentView?.middleName + ' ' : ' ';
                  subItem.studentView.fullName = subItem.studentView?.firstGivenName + middleName + subItem.studentView?.lastFamilyName;
                  subItem.studentView.studentName = subItem.studentView?.firstGivenName + middleName + subItem.studentView?.lastFamilyName;

                  subItem.studentView.dateOfBirth = subItem.studentView?.dob ? moment(subItem.studentView?.dob).format('MMM DD, YYYY') : null;

                  subItem.studentView.section = subItem.studentView?.sectionName;
                  subItem.studentView.gradeLevel = subItem.studentView?.gradeLevelTitle;

                  subItem.studentView.studentId = subItem.studentView?.studentInternalId;

                  subItem?.fieldsCategoryList[0]?.customFields?.map(subOfSubItem => {
                    subItem.studentView[subOfSubItem.title] = subOfSubItem.customFieldsValue?.length > 0 ? subOfSubItem.customFieldsValue[0].customFieldValue : subOfSubItem.defaultSelection;
                  });

                  subItem?.fieldsCategoryList[1]?.customFields?.map(subOfSubItem => {
                    subItem.studentView[subOfSubItem.title] = subOfSubItem.customFieldsValue?.length > 0 ? subOfSubItem.customFieldsValue[0].customFieldValue : subOfSubItem.defaultSelection;
                  });

                  subItem.customCategory = [];
                  subItem?.fieldsCategoryList?.map(subOfSubItem => {
                    if (!subOfSubItem?.isSystemCategory && subOfSubItem?.customFields?.length) {
                      subItem.customCategory.push(subOfSubItem);
                    }
                  });

                  if (subItem?.customCategory?.length) {
                    subItem.customCategory.map(innerItem => {
                      innerItem?.customFields?.map(subOfSubItem => {
                        subItem.studentView[subOfSubItem.title] = subOfSubItem.customFieldsValue?.length > 0 ? subOfSubItem.customFieldsValue[0].customFieldValue : subOfSubItem.defaultSelection;
                      });
                    });
                  }
                });
              }
            });
          }
          this.currentTab = 'selectFields';
        }
      }
    });
  }

  changeTab(status) {
    if (status === 'selectFields' && this.generateCourseSectionList?.length > 0 && this.staffAndCourseCheck.length > 0) {
      if (this.selectedFieldsArray.length) {
        this.selectedFieldsArray.map(fields => {
          if (fields.property === 'studentName') {
            fields.property = 'fullName';
          }
        });
      }
      this.currentTab = status;
    } else if (status === 'generateReport' && this.selectedFieldsArray.length > 0) {
      this.selectedFieldsArray.map((fields,index) => {
        if (fields.property === 'fullName') {
          fields.property = 'studentName';
        }
        // if(index>5){
        //   fields.visible=false
        // }
      });
      this.currentTab = status;
    } else if (status === 'selectSTeacher') {
      this.currentTab = status;
    }
  }

  changeFields(event, type, masterCheck?, key?) {
    if (masterCheck) {
      if (this.fieldsDetailsArray[key][0].checked) {
        this.fieldsDetailsArray[key].map((item, index) => {
          if (index > 0) {
            item.checked = false;
            if (this.selectedFieldsArray.findIndex(x => x.property === item.property) === -1) {
              this.selectedFieldsArray.push({ property: item.property });
            }
          }
        })
      } else {
        this.fieldsDetailsArray[key].map((item, index) => {
          if (index > 0) {
            // item.checked = false;
            const index = this.selectedFieldsArray.findIndex(x => x.property === item.property);
            this.selectedFieldsArray.splice(index, 1);
          }
        })
      }
    } else {
      if (event.checked) {
        if (key) {
          if(this.fieldsDetailsArray[key][0].checked){
            this.fieldsDetailsArray[key][0].checked=false;
            this.fieldsDetailsArray[key].map((item, index) => {
              if (index > 0) {
                const index = this.selectedFieldsArray.findIndex(x => x.property === item.property);
                this.selectedFieldsArray.splice(index, 1);
              }
            })
          }
            this.selectedFieldsArray.push({ property: type});
        } else {
          this.selectedFieldsArray.push({ property: type});
        }
      } else {
        if (key) {
          // this.fieldsDetailsArray[key][0].checked = false;
          const index = this.selectedFieldsArray.findIndex(x => x.property === type);
          this.selectedFieldsArray.splice(index, 1);
        } else {
          const index = this.selectedFieldsArray.findIndex(x => x.property === type);
          this.selectedFieldsArray.splice(index, 1);
        }
      }
    }
  }

  generateExcel() {
    if (this.generateCourseSectionList?.length) {
      let object = {};
      let object1 = {};
      let studentList = [];
      this.generateCourseSectionList.map((item) => {
        Object.assign(object, {
          [this.defaultValuesService.translateKey('courseSection')]: item.courseSectionName,
          [this.defaultValuesService.translateKey('course')]: item.courseTitle,
          [this.defaultValuesService.translateKey('teacher')]: item.staffName ? item.staffName : this.defaultValuesService.translateKey('noTeacherScheduled')
        });
        if (item.studentLists?.length) {
          item.studentLists?.map(subItem => {
            this.selectedFieldsArray.map((fields) => {
              Object.assign(object1, { [this.defaultValuesService.translateKey(fields.property)]: subItem.studentView[fields.property] ? subItem.studentView[fields.property] : '-' });
            });
            Object.assign(object, object1);
            studentList.push(JSON.parse(JSON.stringify(object)));
          });
        }
      });
      this.excelService.exportAsExcelFile(studentList, 'Schedule_Class_Lists Report');
    }
  }

   // For destroy the isLoading subject.
  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
  
}
