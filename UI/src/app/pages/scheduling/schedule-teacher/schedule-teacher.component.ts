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
import { AddTeacherComponent } from './add-teacher/add-teacher.component';
import { AddCourseSectionComponent } from './add-course-section/add-course-section.component';
import { TranslateService } from '@ngx-translate/core';
import { AddDaysScheduleComponent } from './add-days-schedule/add-days-schedule.component';
import { TeacherScheduleService } from '../../../services/teacher-schedule.service';
import { CourseSectionList, StaffScheduleView, StaffScheduleViewModel } from '../../../models/teacher-schedule.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { GetMarkingPeriodTitleListModel } from '../../../models/marking-period.model';
import { MarkingPeriodService } from '../../../services/marking-period.service';
import { LoaderService } from '../../../services/loader.service';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { stagger60ms } from '../../../../@vex/animations/stagger.animation';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { weeks } from '../../../common/static-data';
import { DefaultValuesService } from '../../../common/default-values.service';
import { Permissions } from '../../../models/roll-based-access.model';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { CommonService } from '../../../services/common.service';
import { GetAllCourseListModel, GetAllProgramModel, GetAllSubjectModel } from '../../../models/course-manager.model';
import { CourseManagerService } from '../../../services/course-manager.service';
import { GetAllGradeLevelsModel } from '../../../models/grade-level.model';
import { GetAllMembersList } from '../../../models/membership.model';
import { LanguageModel } from '../../../models/language.model';
import { LoginService } from '../../../services/login.service';
import { GradeLevelService } from '../../../services/grade-level.service';
import { MembershipService } from '../../../services/membership.service';
@Component({
  selector: 'vex-schedule-teacher',
  templateUrl: './schedule-teacher.component.html',
  animations: [
    stagger60ms,
    fadeInUp400ms
  ],
  styleUrls: ['./schedule-teacher.component.scss']
})
export class ScheduleTeacherComponent implements OnInit, OnDestroy {
  selectedTeachers = [];
  selectedCourseSection = [];
  staffScheduleView: StaffScheduleViewModel;
  staffScheduleListForView: StaffScheduleViewModel = new StaffScheduleViewModel();
  getMarkingPeriodTitleListModel: GetMarkingPeriodTitleListModel = new GetMarkingPeriodTitleListModel();
  getAllProgramModel: GetAllProgramModel = new GetAllProgramModel();
  getAllSubjectModel: GetAllSubjectModel = new GetAllSubjectModel();
  getAllCourseListModel: GetAllCourseListModel = new GetAllCourseListModel();
  getAllGradeLevelsModel:GetAllGradeLevelsModel= new GetAllGradeLevelsModel();
  getAllMembersList: GetAllMembersList = new GetAllMembersList();
  languages: LanguageModel = new LanguageModel();
  isStartSchedulingPossible = false;
  weeks = weeks;
  cloneStaffScheduleList: StaffScheduleViewModel;
  cloneStaffScheduleListForCheckAvailibility: StaffScheduleViewModel;
  staffSchedulingFinished = false;
  checkAvailabilityLoader = false;
  startSchedulingLoader = false;
  globalLoader: boolean;
  destroySubject$: Subject<void> = new Subject();
  noConflictDetected = false;
  checkAvailabilityFinished = false;
  selectionProcessing = false;
  permissions: Permissions;
  isHasStartScheduling;
  constructor(
    private dialog: MatDialog,
    private translateService: TranslateService,
    private staffScheduleService: TeacherScheduleService,
    private snackBar: MatSnackBar,
    private loginService:LoginService,
    private gradeLevelService: GradeLevelService,
    private courseManagerService:CourseManagerService,
    private membershipService: MembershipService,
    private markingPeriodService: MarkingPeriodService,
    private loaderService: LoaderService,
    private pageRolePermissions: PageRolesPermission,
    private defaultValuesService: DefaultValuesService,
    private commonService: CommonService,
  ) {
    //translateService.use('en');
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.globalLoader = val;
    });
  }

  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission();
    this.getAllMarkingPeriodList();
    this.getAllCourse();
    this.getAllSubjectList();
    this.getAllProgramList();
    this.getAllLanguage();
    this.getAllGradeLevelList();
    this.getAllMembership();
  }

  getAllLanguage() {
    this.loginService.getAllLanguage(this.languages).pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        this.languages.tableLanguage = [];
      }else if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        this.languages.tableLanguage = [];
        if(!res.tableLanguage){
          this.snackBar.open(res._message, '', {
            duration: 10000
          });
        }
      }
      else {
        this.languages.tableLanguage = res.tableLanguage?.sort((a, b) => { return a.locale < b.locale ? -1 : 1; })

      }
    })
  }

  getAllMembership() {
    this.membershipService.getAllMembers(this.getAllMembersList).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        this.snackBar.open('Membership List failed. ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
      else {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.getAllMembersList.getAllMemberList = [];
          if (!res.getAllMemberList) {
            this.snackBar.open( res._message,'', {
              duration: 4000
            });
          }
        }
        else {
          this.getAllMembersList.getAllMemberList = res.getAllMemberList.filter((item) => {
            return (item.profileType == 'School Administrator' || item.profileType == 'Admin Assistant'
              || item.profileType == 'Teacher' || item.profileType == 'Homeroom Teacher')
          });
        }
      }
    })
  }

  getAllGradeLevelList(){   
    this.gradeLevelService.getAllGradeLevels(this.getAllGradeLevelsModel).subscribe(data => {   
      if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
        }       
      this.getAllGradeLevelsModel.tableGradelevelList=data.tableGradelevelList;      
    });
  }


  selectTeacher() {
    if(this.defaultValuesService.checkAcademicYear()){
    this.dialog.open(AddTeacherComponent, {
      width: '900px',
      data:{
        subjectList: this.getAllSubjectModel.subjectList,
        memberList: this.getAllMembersList.getAllMemberList,
        languagelist: this.languages.tableLanguage,
        gradelevelList: this.getAllGradeLevelsModel.tableGradelevelList
      }
    }).afterClosed().subscribe((res) => {
      this.selectedTeachers = res ? res : [];
      this.getTeacherScheduleView();
    });
  }
  }

  selectCourseSection() {
    if(this.defaultValuesService.checkAcademicYear()){
    this.dialog.open(AddCourseSectionComponent, {
      data: { 
        markingPeriods: this.getMarkingPeriodTitleListModel.getMarkingPeriodView,
        courseList: this.getAllCourseListModel.courseViewModelList,
        subjectList: this.getAllSubjectModel.subjectList,
        programList: this.getAllProgramModel.programList
      },
      width: '900px'
    }).afterClosed().subscribe((res) => {
      this.selectedCourseSection = res ? res : [];
      this.getTeacherScheduleView();
    });
    }
  }


  getTeacherScheduleView() {
    if (this.selectedTeachers.length > 0 && this.selectedCourseSection.length > 0) {
      this.staffSchedulingFinished = false;
      this.isStartSchedulingPossible = false;
      this.checkAvailabilityFinished = false;
      this.checkAvailabilityLoader = false;
      this.collectValuesFromTeacherAndCourseSection();
      this.selectionProcessing = true;

      this.staffScheduleService.staffScheduleViewForCourseSection(this.staffScheduleView).subscribe((res) => {
        if (typeof (res) == 'undefined') {
          this.staffScheduleListForView.staffScheduleViewList = [];
        }
        else {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            if (res.staffScheduleViewList == null) {
              this.snackBar.open(res._message, '', {
                duration: 5000
              });
              this.staffScheduleListForView.staffScheduleViewList = [];
            } else {
              this.staffScheduleListForView.staffScheduleViewList = [];
            }
          } else {
            this.staffScheduleListForView = JSON.parse(JSON.stringify(res));
            this.staffScheduleListForView.staffScheduleViewList = this.staffScheduleListForView.staffScheduleViewList.map((staffDetails) => {
              staffDetails.oneOrMoreCourseSectionChecked = true;
              staffDetails.allCourseSectionChecked = true;
              staffDetails.courseSectionViewList = this.findMarkingPeriodTitleById(staffDetails.courseSectionViewList);
              return staffDetails;
            });
            this.staffScheduleListForView.staffScheduleViewList = this.staffScheduleListForView.staffScheduleViewList.map((item) => {
              item.allCourseSectionChecked = item.courseSectionViewList.every((courseSection) => {
                return courseSection.checked;
              });
              return item;
            });
            this.cloneStaffScheduleListForCheckAvailibility = JSON.parse(JSON.stringify(this.staffScheduleListForView));
          }
          this.selectionProcessing = false;
        }
      });
    }

  }

  getAllProgramList() {
    this.courseManagerService.GetAllProgramsList(this.getAllProgramModel).subscribe(data => {
      if(data){
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          this.getAllProgramModel.programList=[];
          if(!data.programList){
            this.snackBar.open(data._message, '', {
              duration: 1000
            }); 
          }
        }else{
          this.getAllProgramModel.programList=data.programList;
        }
      }else{
        this.snackBar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 1000
        }); 
      }  
    });
  }
  getAllSubjectList() {
    this.courseManagerService.GetAllSubjectList(this.getAllSubjectModel).subscribe(data => {
      if(data){
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          this.getAllSubjectModel.subjectList=[];
          if(!data.subjectList){
            this.snackBar.open(data._message, '', {
              duration: 1000
            }); 
          }
        }else{
          this.getAllSubjectModel.subjectList = data.subjectList;
        }
      }else{
        this.snackBar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 1000
        }); 
      } 
    });
  }

  getAllCourse() {
    this.courseManagerService.GetAllCourseList(this.getAllCourseListModel).subscribe(data => {
      if (data) {
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          this.getAllCourseListModel.courseViewModelList = [];
          if (!data.courseViewModelList) {
            this.snackBar.open(data._message, '', {
              duration: 1000
            });
          }
        } else {
          this.getAllCourseListModel.courseViewModelList = data.courseViewModelList;
        }
      } else {
        this.snackBar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    })
  }


  // selected teachers and course sections
  collectValuesFromTeacherAndCourseSection() {
    const staffScheduleList: StaffScheduleView[] = [];
    this.selectedTeachers?.map((item, i) => {
      staffScheduleList.push(new StaffScheduleView);
      staffScheduleList[i].staffId = item.staffId;
    });

    const courseSectionList: CourseSectionList[] = [];
    this.selectedCourseSection?.map((item, i) => {
      courseSectionList.push(new CourseSectionList);
      courseSectionList[i].courseSectionId = item.courseSectionId;
    });

    this.staffScheduleView = new StaffScheduleViewModel;
    this.staffScheduleView.staffScheduleViewList = staffScheduleList;
    this.staffScheduleView.courseSectionViewList = courseSectionList;
  }

  // Find Marking Period By Title
  findMarkingPeriodTitleById(courseSectionList) {
    courseSectionList = courseSectionList.map((item) => {
      if (item.yrMarkingPeriodId) {
        item.yrMarkingPeriodId = '0_' + item.yrMarkingPeriodId;
      } else if (item.smstrMarkingPeriodId) {
        item.smstrMarkingPeriodId = '1_' + item.smstrMarkingPeriodId;
      } else if (item.qtrMarkingPeriodId) {
        item.qtrMarkingPeriodId = '2_' + item.qtrMarkingPeriodId;
      } else if (item.prgrsprdMarkingPeriodId) {
        item.prgrsprdMarkingPeriodId = '3_' + item.prgrsprdMarkingPeriodId;
      }

      if (item.yrMarkingPeriodId || item.smstrMarkingPeriodId || item.qtrMarkingPeriodId || item.prgrsprdMarkingPeriodId) {
        for (const markingPeriod of this.getMarkingPeriodTitleListModel.getMarkingPeriodView) {
          if (markingPeriod.value == item.yrMarkingPeriodId) {
            item.markingPeriodTitle = markingPeriod.text;
            break;
          } else if (markingPeriod.value == item.smstrMarkingPeriodId) {
            item.markingPeriodTitle = markingPeriod.text;
            break;
          } else if (markingPeriod.value == item.qtrMarkingPeriodId) {
            item.markingPeriodTitle = markingPeriod.text;
            break;
          } else if (markingPeriod.value == item.prgrsprdMarkingPeriodId) {
            item.markingPeriodTitle = markingPeriod.text;
            break;
          }
        }
      } else {
        item.markingPeriodTitle = 'Custom';
      }
      item.checked = true;
      if (item.scheduleType == 'Variable Schedule' || item.scheduleType == 'Fixed Schedule') {
        item.cloneMeetingDays = JSON.parse(JSON.stringify(item.meetingDays));
        const days = item.cloneMeetingDays.split('|');
        days.map((day) => {
          for (const [i, weekDay] of this.weeks.entries()) {
            if (weekDay.name.toLocaleLowerCase() == day.trim().toLocaleLowerCase()) {
              item.cloneMeetingDays = item.cloneMeetingDays + weekDay.id;
              break;
            }
          }
        });
      }
      return item;
    });
    return courseSectionList;
  }

  getAllMarkingPeriodList() {
    this.getMarkingPeriodTitleListModel.academicYear = this.defaultValuesService.getAcademicYear();
    this.markingPeriodService.getAllMarkingPeriodList(this.getMarkingPeriodTitleListModel).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
        this.getMarkingPeriodTitleListModel.getMarkingPeriodView = [];
        if (!this.getMarkingPeriodTitleListModel?.getMarkingPeriodView) {
          this.snackBar.open(data._message, '', {
            duration: 1000
          });
        }
      } else {
        this.getMarkingPeriodTitleListModel.getMarkingPeriodView = data.getMarkingPeriodView;
      }
    });
  }


  masterCheckToggle(event, index) {
    this.staffScheduleListForView.staffScheduleViewList[index].courseSectionViewList.forEach((item) => {
      item.checked = event;
    });

    this.cloneStaffScheduleListForCheckAvailibility.staffScheduleViewList[index].courseSectionViewList.forEach((item) => {
      item.checked = event;
    });
    if (this.isStartSchedulingPossible) {
      this.isAnyCourseSelectedOrNot();
    }
  }

  //  when User selecting single record
  singleSelection(event, indexOfCourseSection, indexOfStaffList) {
    this.staffScheduleListForView.staffScheduleViewList[indexOfStaffList].courseSectionViewList[indexOfCourseSection].checked = event;
    this.staffScheduleListForView.staffScheduleViewList[indexOfStaffList].allCourseSectionChecked = this.staffScheduleListForView.staffScheduleViewList[indexOfStaffList].courseSectionViewList.every((courseSection) => {
      return courseSection.checked;
    });

    this.cloneStaffScheduleListForCheckAvailibility.staffScheduleViewList[indexOfStaffList].courseSectionViewList[indexOfCourseSection].checked = event;
    this.cloneStaffScheduleListForCheckAvailibility.staffScheduleViewList[indexOfStaffList].allCourseSectionChecked = this.cloneStaffScheduleListForCheckAvailibility.staffScheduleViewList[indexOfStaffList].courseSectionViewList.every((courseSection) => {
      return courseSection.checked;
    });
    if (this.isStartSchedulingPossible) {
      this.isAnyCourseSelectedOrNot(); 
    }
  }

  //Check is any course section selected or not
  isAnyCourseSelectedOrNot() {
    let checkStaffScheduleList = this.removeConflictAndUncheckedRowsIfAny(this.cloneStaffScheduleList)
    if (checkStaffScheduleList.staffScheduleViewList.length) {
      this.isHasStartScheduling = false
    } else {
      this.isHasStartScheduling = true
    }
  }

  // Check Availablity of staff
  checkAvailability() {
    this.checkAvailabilityLoader = true;
    this.cloneStaffScheduleList = JSON.parse(JSON.stringify(this.cloneStaffScheduleListForCheckAvailibility));
    this.onlySendCheckedCourseSections();
    this.cloneStaffScheduleList._userName = this.defaultValuesService.getUserName();
    this.cloneStaffScheduleList._failure = false;
    this.staffScheduleService.checkAvailabilityStaffCourseSectionSchedule(this.cloneStaffScheduleList).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        this.cloneStaffScheduleList.staffScheduleViewList = [];
      }
      else {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          if (res.staffScheduleViewList == null) {
            this.snackBar.open(res._message, '', {
              duration: 5000
            });
          } else {
            this.noConflictDetected = false;
            this.cloneStaffScheduleList = res;
            this.checkForConflicts();
            this.checkIfStartSchedulingPossibleOrNot();
          }
        } else {
          this.noConflictDetected = true;
          this.cloneStaffScheduleList = res;
          this.checkForConflicts();
          this.checkIfStartSchedulingPossibleOrNot();
        }
      }
      this.checkAvailabilityFinished = true;
      this.checkAvailabilityLoader = false;


    });
  }



  onlySendCheckedCourseSections() {
    this.cloneStaffScheduleList.staffScheduleViewList.map((staffDetails) => {
      let oneOrMoreCourseSectionChecked = true;
      staffDetails.courseSectionViewList.map((item) => {
        let falseCount = 0;
        if (!item.checked) {
          falseCount = falseCount + 1;
        }
        if (falseCount == staffDetails.courseSectionViewList.length) {
          oneOrMoreCourseSectionChecked = false;
        }
      });
      if (!oneOrMoreCourseSectionChecked) {
        staffDetails.oneOrMoreCourseSectionChecked = false;
      }
    });

    this.cloneStaffScheduleList.staffScheduleViewList = this.cloneStaffScheduleList.staffScheduleViewList.filter((item) => {
      return item.oneOrMoreCourseSectionChecked;
    });

    this.cloneStaffScheduleList.staffScheduleViewList = this.cloneStaffScheduleList.staffScheduleViewList.map((staffDetails) => {
      staffDetails.courseSectionViewList = staffDetails.courseSectionViewList.filter((courseSection) => {
        return courseSection.checked;
      });
      return staffDetails;
    });

    this.cloneStaffScheduleList.staffScheduleViewList = this.cloneStaffScheduleList.staffScheduleViewList.map((staffDetails) => {
      staffDetails.courseSectionViewList = this.removeIdentifierFromMarkingPeriods(staffDetails.courseSectionViewList);
      return staffDetails;
    });
  }

  uncheckTheConflicted() {
    this.cloneStaffScheduleListForCheckAvailibility.staffScheduleViewList = this.cloneStaffScheduleListForCheckAvailibility.staffScheduleViewList.map((staffDetails) => {
      staffDetails.courseSectionViewList = staffDetails.courseSectionViewList.filter((courseSection) => {
        return !courseSection.conflictCourseSection;
      });
      return staffDetails;
    });
    this.cloneStaffScheduleListForCheckAvailibility.staffScheduleViewList = this.cloneStaffScheduleListForCheckAvailibility.staffScheduleViewList.filter((staffDetails) => {
      return staffDetails.courseSectionViewList.length > 0;
    });
  }

  checkForConflicts() {
    for (const conflictedStaff of this.cloneStaffScheduleList.staffScheduleViewList) {
      for (const viewStaff of this.staffScheduleListForView.staffScheduleViewList) {
        if (conflictedStaff.staffId == viewStaff.staffId) {
          viewStaff.conflictStaff = conflictedStaff.conflictStaff;

          for (const conflictedCourseSection of conflictedStaff.courseSectionViewList) {
            for (const viewCourseSection of viewStaff.courseSectionViewList) {
              if (conflictedCourseSection.courseSectionId == viewCourseSection.courseSectionId) {
                viewCourseSection.conflictCourseSection = conflictedCourseSection.conflictCourseSection;
                conflictedCourseSection.checked = viewCourseSection.checked;
              }
            }
          }
        }
      }
    }

  }

  checkIfStartSchedulingPossibleOrNot() {
    let startSchedulingPossible = false;
    this.cloneStaffScheduleList.staffScheduleViewList.map((staffDetails) => {
      staffDetails.courseSectionViewList.map((courseSection) => {
        if (!courseSection.conflictCourseSection) {
          startSchedulingPossible = true;
        }
      });
    });

    if (startSchedulingPossible) {
      this.isStartSchedulingPossible = true;
    } else {
      this.isStartSchedulingPossible = false;
    }
  }

  // start schedule after all valid checkings.
  startScheduling() {
    this.startSchedulingLoader = true;
    let staffScheduleList = JSON.parse(JSON.stringify(this.cloneStaffScheduleList));
    staffScheduleList = this.removeConflictAndUncheckedRowsIfAny(staffScheduleList);
    this.staffScheduleService.addStaffCourseSectionSchedule(staffScheduleList).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        staffScheduleList.staffScheduleViewList = [];
      }
      else {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          if (res.staffScheduleViewList == null) {
            this.snackBar.open(res._message, '', {
              duration: 5000
            });

          } else {
            this.snackBar.open(res._message, '', {
              duration: 5000
            });
          }
        } else {
          this.staffSchedulingFinished = true;
          this.selectedTeachers = [];
          this.selectedCourseSection = [];
        }
      }
      this.startSchedulingLoader = false;
    });
  }

  removeConflictAndUncheckedRowsIfAny(staffScheduleList) {
    const staffScheduleView = JSON.parse(JSON.stringify(this.staffScheduleListForView));
    staffScheduleList.staffScheduleViewList = staffScheduleView.staffScheduleViewList.map((staffDetails) => {
      staffDetails.courseSectionViewList = this.removeIdentifierFromMarkingPeriods(staffDetails.courseSectionViewList);
      staffDetails.courseSectionViewList = staffDetails.courseSectionViewList.filter((courseSection) => {
        return (courseSection.checked && !courseSection.conflictCourseSection);
      });
      return staffDetails;
    });
    staffScheduleList.staffScheduleViewList = staffScheduleList.staffScheduleViewList.filter((item) => {
      return item.courseSectionViewList.length > 0;
    });

    return staffScheduleList;
  }

  // remove marking period identifier before submitting any info
  removeIdentifierFromMarkingPeriods(courseSectionList): any {
    const courseSection = courseSectionList.map((item) => {
      if (item.yrMarkingPeriodId) {
        item.yrMarkingPeriodId = item.yrMarkingPeriodId.split('_')[1];
      } else if (item.smstrMarkingPeriodId) {
        item.smstrMarkingPeriodId = item.smstrMarkingPeriodId.split('_')[1];
      } else if (item.qtrMarkingPeriodId) {
        item.qtrMarkingPeriodId = item.qtrMarkingPeriodId.split('_')[1];
      } else if (item.prgrsprdMarkingPeriodId) {
        item.prgrsprdMarkingPeriodId = item.prgrsprdMarkingPeriodId.split('_')[1];
      }
      return item;
    });
    return courseSection;
  }


  viewCorrespondingSchedule(courseSectionDetails) {
    this.dialog.open(AddDaysScheduleComponent, {
      width: '600px',
      data: { scheduleDetails: courseSectionDetails, fromTeacherSchedule: true }
    });
  }


  ngOnDestroy(): void {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}
