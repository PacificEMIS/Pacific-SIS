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

import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { AddTeacherComponent } from './add-teacher/add-teacher.component';
import { AddCourseComponent } from './add-course/add-course.component';
import { StaffListModel, StaffMasterSearchModel } from '../../../models/staff.model';
import { CourseModel, GetAllProgramModel, GetAllSubjectModel } from '../../../models/course-manager.model';
import { TeacherScheduleService } from '../../../services/teacher-schedule.service';
import { AllScheduledCourseSectionForStaffModel, StaffScheduleView, StaffScheduleViewModel } from '../../../models/teacher-schedule.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { GetMarkingPeriodTitleListModel } from '../../../models/marking-period.model';
import { MarkingPeriodService } from '../../../services/marking-period.service';
import { CourseSectionService } from '../../../services/course-section.service';
import { ScheduledStaffForCourseSection } from '../../../models/course-section.model';
import { map, takeUntil } from 'rxjs/operators';
import { LoaderService } from '../../../services/loader.service';
import { Subject } from 'rxjs';
import { weeks } from '../../../common/static-data';
import { Permissions } from '../../../models/roll-based-access.model';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { CommonService } from '../../../services/common.service';
import { DefaultValuesService } from '../../../common/default-values.service';
import { GetAllGradeLevelsModel } from '../../../models/grade-level.model';
import { GetAllMembersList } from '../../../models/membership.model';
import { LanguageModel } from '../../../models/language.model';
import { LoginService } from '../../../services/login.service';
import { CourseManagerService } from '../../../services/course-manager.service';
import { GradeLevelService } from '../../../services/grade-level.service';
import { MembershipService } from '../../../services/membership.service';

@Component({
  selector: 'vex-teacher-reassignment',
  templateUrl: './teacher-reassignment.component.html',
  styleUrls: ['./teacher-reassignment.component.scss']
})
export class TeacherReassignmentComponent implements OnInit {

  constructor(private dialog: MatDialog,
    public translateService: TranslateService,
    private teacherReassignmentService: TeacherScheduleService,
    private snackbar: MatSnackBar,
    private markingPeriodService: MarkingPeriodService,
    private courseSectionService: CourseSectionService,
    private staffScheduleService: TeacherScheduleService,
    private loaderService: LoaderService,
    private pageRolePermissions: PageRolesPermission,
    private commonService: CommonService,
    private loginService: LoginService,
    private membershipService: MembershipService,
    private gradeLevelService: GradeLevelService,
    private courseManagerService: CourseManagerService,
    private defaultValuesService: DefaultValuesService,

    ) {
    //translateService.use('en');
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.globalLoader = val;
    });
  }
  destroySubject$: Subject<void> = new Subject();
  globalLoader: boolean;
  selectedCurrentTeacher: StaffListModel;
  selectedNewTeacher: StaffListModel;
  selectedCourse: CourseModel;
  allScheduledCourseSectionBasedOnTeacher: AllScheduledCourseSectionForStaffModel = new AllScheduledCourseSectionForStaffModel();
  checkAvailibilityBasedOnTeacher: StaffScheduleViewModel = new StaffScheduleViewModel();
  teacherReassignmentBasedOnTeacher = false;
  teacherReassignmentBasedOnCourse = false;
  checkAvailabilityLoader = false;
  checkAvailabilityFinished = false;
  noConflictDetected = false;
  isTeacherReassignPossible = false;
  isAllCourseSectionConflicted = false;
  teacherReassigningLoader = false;
  getMarkingPeriodTitleListModel: GetMarkingPeriodTitleListModel = new GetMarkingPeriodTitleListModel();
  getAllGradeLevelsModel:GetAllGradeLevelsModel= new GetAllGradeLevelsModel();
  getAllSubjectModel: GetAllSubjectModel = new GetAllSubjectModel();
  staffMasterSearchModel: StaffMasterSearchModel = new StaffMasterSearchModel();
  getAllMembersList: GetAllMembersList = new GetAllMembersList();
  getAllProgramModel: GetAllProgramModel = new GetAllProgramModel();
  languages: LanguageModel = new LanguageModel();
  weeks = weeks;
  isAllCoursesChecked = true;
  allScheduledTeacherBasedOnCourse: ScheduledStaffForCourseSection = new ScheduledStaffForCourseSection();
  checkAvailibilityBasedOnCourse: ScheduledStaffForCourseSection = new ScheduledStaffForCourseSection();

  disableMasterCheckboxBasedOnTeacherConflict = false;
  teacherReassigning = false;
  permissions: Permissions;
  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission();
    this.getAllMarkingPeriodList();
    this.getAllLanguage();
    this.getAllProgramList();
    this.getAllGradeLevelList();
    this.getAllSubjectList();
    this.getAllMembership();
    // this.allScheduledTeacherBasedOnCourse.courseSectionsList = null;
  }

  getAllMarkingPeriodList() {
    this.getMarkingPeriodTitleListModel.schoolId = this.defaultValuesService.getSchoolID();
    this.getMarkingPeriodTitleListModel.academicYear = this.defaultValuesService.getAcademicYear();
    this.markingPeriodService.getAllMarkingPeriodList(this.getMarkingPeriodTitleListModel).subscribe(data => {
     if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
        this.getMarkingPeriodTitleListModel.getMarkingPeriodView = [];
        if(!this.getMarkingPeriodTitleListModel?.getMarkingPeriodView){
          this.snackbar.open(data._message, '', {
            duration: 1000
          }); 
        }
      } else {
        this.getMarkingPeriodTitleListModel.getMarkingPeriodView = data.getMarkingPeriodView;
      }
    });
  }

  getAllLanguage() {
    this.languages._tenantName = this.defaultValuesService.getTenantName();
    this.loginService.getAllLanguage(this.languages).pipe(takeUntil(this.destroySubject$)).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        this.languages.tableLanguage = [];
      }else if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        this.languages.tableLanguage = [];
        if(!res.tableLanguage){
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
        }
      }
      else {
        this.languages.tableLanguage = res.tableLanguage?.sort((a, b) => { return a.locale < b.locale ? -1 : 1; })

      }
    })
  }

  getAllProgramList(){   
    this.courseManagerService.GetAllProgramsList(this.getAllProgramModel).subscribe(data => {          
      if(data){
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          this.getAllProgramModel.programList=[];
          if(!data.programList){
            this.snackbar.open(data._message, '', {
              duration: 1000
            }); 
          }
        }else{
          this.getAllProgramModel.programList=data.programList;
        }
      }else{
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 1000
        }); 
      }       
    });
  }

  getAllGradeLevelList(){   
    this.getAllGradeLevelsModel.schoolId = this.defaultValuesService.getSchoolID();
    this.getAllGradeLevelsModel._tenantName = this.defaultValuesService.getTenantName();
    this.getAllGradeLevelsModel._token = this.defaultValuesService.getToken();
    this.gradeLevelService.getAllGradeLevels(this.getAllGradeLevelsModel).subscribe(data => {          
      this.getAllGradeLevelsModel.tableGradelevelList=data.tableGradelevelList;      
    });
  }

  getAllSubjectList(){   
    this.courseManagerService.GetAllSubjectList(this.getAllSubjectModel).subscribe(data => {          
      if(data){
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          this.getAllSubjectModel.subjectList=[];
          if(!data.subjectList){
            this.snackbar.open(data._message, '', {
              duration: 1000
            }); 
          }
        }else{
          this.getAllSubjectModel.subjectList = data.subjectList;
        }
      }else{
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 1000
        }); 
      }  
    });
  }

  getAllMembership() {
    this.membershipService.getAllMembers(this.getAllMembersList).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        this.snackbar.open('Membership List failed. ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
      else {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.getAllMembersList.getAllMemberList = [];
          if (!res.getAllMemberList) {
            this.snackbar.open( res._message,'', {
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


  selectTeacher(teacherType) {
    if(this.defaultValuesService.checkAcademicYear()){
    // Im assuming currentTeacher as 0 and New Teacher as 1.
    this.noConflictDetected = false;
    this.isTeacherReassignPossible = false;
    this.selectedNewTeacher = null;
    this.teacherReassigning = false;
    if (!this.teacherReassignmentBasedOnCourse) {
      this.selectedCourse = null;
    }
    this.isAllCourseSectionConflicted = false;
    this.checkAvailabilityFinished = false;
    if (teacherType != 1) {
      this.teacherReassignmentBasedOnCourse = false;
      this.teacherReassignmentBasedOnTeacher = false;
      // this.allScheduledTeacherBasedOnCourse.courseSectionsList = null;
      this.selectedCourse = null;
    }

    this.dialog.open(AddTeacherComponent, {
      width: '900px',
      data:{
        subjectList: this.getAllSubjectModel.subjectList,
        memberList: this.getAllMembersList.getAllMemberList,
        languagelist: this.languages.tableLanguage,
        gradelevelList: this.getAllGradeLevelsModel.tableGradelevelList
      }
    }).afterClosed().subscribe((res) => {
      if (!res) {
        return;
      }
      if (teacherType === 0) {
        this.isAllCoursesChecked = true;
        this.disableMasterCheckboxBasedOnTeacherConflict = false;
        this.selectedCurrentTeacher = res;
        this.allScheduledCourseSectionBasedOnTeacher.staffId = this.selectedCurrentTeacher.staffId;
        this.getAllScheduledCourseSectionBasedOnTeacher();
      } else {
        this.selectedNewTeacher = res;
        this.checkAllCoursesBasedOnTeacher();
      }
    });
  }
  }

  selectCourse() {
    if(this.defaultValuesService.checkAcademicYear()){
    this.noConflictDetected = false;
    this.selectedCurrentTeacher = null;
    this.teacherReassignmentBasedOnCourse = false;
    this.teacherReassignmentBasedOnTeacher = false;
    this.isTeacherReassignPossible = false;
    this.isAllCourseSectionConflicted = false;
    this.checkAvailabilityFinished = false;
    this.selectedNewTeacher = null;
    this.teacherReassigning = false;
    this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList = null;
    this.dialog.open(AddCourseComponent, {
      width: '900px',
      data:{
        subjectList: this.getAllSubjectModel.subjectList,
        programList: this.getAllProgramModel.programList
      }
    }).afterClosed().subscribe((res) => {
      if (!res) {
        return;
      }
      this.selectedCourse = res;
      this.getScheduledTeachersBasedOnCourse();
    });
  }
  }

  getAllScheduledCourseSectionBasedOnTeacher() {
    this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList =[];
    this.teacherReassignmentService.getAllScheduledCourseSectionForStaff(this.allScheduledCourseSectionBasedOnTeacher).pipe(
      map((res) => {
        res._userName = this.defaultValuesService.getUserName();
        return res;
      })
    ).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        this.snackbar.open('Scheduled Course Section Failed ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
      else {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList = []
          if (!res.courseSectionViewList) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        } else {
          this.allScheduledCourseSectionBasedOnTeacher = res;
          this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList = this.findMarkingPeriodTitleById(this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList);
          this.teacherReassignmentBasedOnTeacher = true;
          this.teacherReassignmentBasedOnCourse = false;
        }
      }
    })
  }


  getScheduledTeachersBasedOnCourse() {
    this.allScheduledTeacherBasedOnCourse.courseSectionsList = []
    this.allScheduledTeacherBasedOnCourse.courseId = this.selectedCourse.courseId;
    this.courseSectionService.getAllStaffScheduleInCourseSection(this.allScheduledTeacherBasedOnCourse).pipe(
      map((res) => {
        res.courseSectionsList = res.courseSectionsList.filter((item) => {
          return item.staffCoursesectionSchedule.length > 0
        })
        return res;
      })
    ).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        this.snackbar.open('Course Sections Failed ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
      else {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.allScheduledTeacherBasedOnCourse.courseSectionsList = []
          if (!res.courseSectionsList) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        } else {
          this.allScheduledTeacherBasedOnCourse.courseSectionsList = res.courseSectionsList;
          this.moveLanguageNames();
          this.findMarkingPeriodTitle();

          this.teacherReassignmentBasedOnCourse = this.allScheduledTeacherBasedOnCourse.courseSectionsList.some((item) => {
            return item.staffCoursesectionSchedule.length
          })
          this.teacherReassignmentBasedOnTeacher = false;
        }
      }
    })
  }
 
  // Find Language Names by Id
  moveLanguageNames() {
    this.allScheduledTeacherBasedOnCourse.courseSectionsList.map((courseSection) => {
      courseSection.staffCoursesectionSchedule?.forEach((staff) => {
        if (staff.staffMaster.firstLanguage) {
          staff.staffMaster.firstLanguageName = staff.staffMaster.firstLanguageNavigation.locale;
        }
        if (staff.staffMaster.secondLanguage) {
          staff.staffMaster.secondLanguageName = staff.staffMaster.secondLanguageNavigation.locale;
        }
        if (staff.staffMaster.thirdLanguage) {
          staff.staffMaster.thirdLanguageName = staff.staffMaster.thirdLanguageNavigation.locale;
        }
      })
    })
  }

  checkAvailability() {
    if (this.teacherReassignmentBasedOnTeacher) {
      this.checkAvailabilityBasedOnTeacher();
    } else if (this.teacherReassignmentBasedOnCourse) {
      this.checkAvailabilityBasedOnCourses();
    }
  }

  reassignTeacher() {
    if (this.teacherReassignmentBasedOnTeacher) {
      this.reassignTeacherScheduleBasedOnTeacher();
    } else if (this.teacherReassignmentBasedOnCourse) {
      this.reassignTeacherScheduleBasedOnCourse();
    }
  }

  checkAvailabilityBasedOnTeacher() {
    this.checkAvailibilityBasedOnTeacher.existingStaff = this.allScheduledCourseSectionBasedOnTeacher.staffId;
    this.checkAvailibilityBasedOnTeacher.staffScheduleViewList = [new StaffScheduleView()]
    this.checkAvailibilityBasedOnTeacher.staffScheduleViewList[0].staffId = this.selectedNewTeacher.staffId;
    this.checkAvailibilityBasedOnTeacher.staffScheduleViewList[0].courseSectionViewList = this.onlySendCheckedCourse(this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList);
    this.checkAvailibilityBasedOnTeacher.staffScheduleViewList[0].courseSectionViewList = this.removeIdentifierFromMarkingPeriods(this.checkAvailibilityBasedOnTeacher.staffScheduleViewList[0].courseSectionViewList);
    this.checkAvailibilityBasedOnTeacher._failure = false;
    if (this.checkAvailibilityBasedOnTeacher.staffScheduleViewList[0].courseSectionViewList.length == 0) {
      this.snackbar.open('Select Course Section for Schedule Checking', '', {
        duration: 5000
      });
      return;
    }
    this.checkAvailabilityLoader = true;
    this.staffScheduleService.checkAvailabilityStaffCourseSectionSchedule(this.checkAvailibilityBasedOnTeacher).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        this.checkAvailibilityBasedOnTeacher.staffScheduleViewList = [];
      }
      else {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          if (res.staffScheduleViewList == null) {
            this.snackbar.open(res._message, '', {
              duration: 5000
            });
          } else {
            this.noConflictDetected = false;
            this.checkAvailibilityBasedOnTeacher = res;
            this.checkForConflicts(this.checkAvailibilityBasedOnTeacher.staffScheduleViewList[0].courseSectionViewList);
            if (this.checkAvailibilityBasedOnTeacher.staffScheduleViewList[0].conflictStaff) {
              this.isAllCoursesChecked = false;
              this.disableMasterCheckboxBasedOnTeacherConflict = true;
            }
            this.checkIfReassignPossibleOrNot();
          }
        } else {
          this.noConflictDetected = true;
          this.checkAvailibilityBasedOnTeacher = res;
          this.checkForConflicts(this.checkAvailibilityBasedOnTeacher.staffScheduleViewList[0].courseSectionViewList);
          this.checkIfReassignPossibleOrNot();
        }
      }
      this.checkAvailabilityFinished = true
      this.checkAvailabilityLoader = false;
    });
  }

  onlySendCheckedCourse(courseSectionList) {
    courseSectionList = courseSectionList.filter(item => item.checked);

    return courseSectionList;
  }

  checkForConflicts(courseSectionViewList) {

    for (let courseSectionInView of this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList) {
      for (let conflictCourseSection of courseSectionViewList) {
        if (courseSectionInView.courseSectionId === conflictCourseSection.courseSectionId) {
          courseSectionInView.checked = !conflictCourseSection.conflictCourseSection;
          courseSectionInView.conflictCourseSection = conflictCourseSection.conflictCourseSection
        }
      }
    }
    this.isAllCourseSectionConflicted = this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList.every((item => item.conflictCourseSection))
  }

  findMarkingPeriodTitle() {
    this.allScheduledTeacherBasedOnCourse.courseSectionsList?.map((item) => {
      if (item.quarters) {
        item.mpTitle = item.quarters.title;
      } else if (item.schoolYears) {
        item.mpTitle = item.schoolYears.title;
      } else if (item.semesters) {
        item.mpTitle = item.semesters.title;
      } else if (item.progressPeriods) {
        item.mpTitle = item.progressPeriods.title;
      }
       else {
        item.mpTitle = 'Custom'
      }
    })
  }

  removeIdentifierFromMarkingPeriods(courseSectionList) {
    let courseSection = courseSectionList.map((item) => {
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
    })
    return courseSection;
  }

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
        for (let markingPeriod of this.getMarkingPeriodTitleListModel.getMarkingPeriodView) {
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
        item.markingPeriodTitle = 'Custom'
      }
      item.checked = true;
      if (item.scheduleType == 'Variable Schedule' || item.scheduleType == 'Fixed Schedule') {
        item.cloneMeetingDays = JSON.parse(JSON.stringify(item.meetingDays));
        const days = item.meetingDays.split('|');
        item.cloneMeetingDays='';
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

  checkAllCoursesBasedOnTeacher() {
    this.allScheduledCourseSectionBasedOnTeacher?.courseSectionViewList?.forEach((item, i) => {
      this.singleSelectionBasedOnTeacher(true, i)
      item.conflictCourseSection = false;
      this.disableMasterCheckboxBasedOnTeacherConflict = false;
    })
  }

  checkIfReassignPossibleOrNot() {
    this.isTeacherReassignPossible = this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList.some((item => !item.conflictCourseSection))

  }


  reassignTeacherScheduleBasedOnTeacher() {
    let reassignTeacherSchedule: StaffScheduleViewModel = new StaffScheduleViewModel();
    reassignTeacherSchedule = JSON.parse(JSON.stringify(this.checkAvailibilityBasedOnTeacher))
    reassignTeacherSchedule.staffScheduleViewList[0].courseSectionViewList = this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList.filter((item) => {
      return item.checked;
    })

    this.teacherReassigning = true;
    this.teacherReassigningLoader = true;

    this.staffScheduleService.addStaffCourseSectionReSchedule(reassignTeacherSchedule).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        reassignTeacherSchedule.staffScheduleViewList = [];
      }
      else {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.teacherReassigning = false;

          if (res.staffScheduleViewList == null) {
            this.snackbar.open(res._message, '', {
              duration: 5000
            });
          } else {
            this.snackbar.open(res._message, '', {
              duration: 5000
            });
          }
        } else {
          this.resetAll();
        }

      }
      this.teacherReassigningLoader = false;
    })
  }

  reassignTeacherScheduleBasedOnCourse() {
    let reassignTeacherSchedule: ScheduledStaffForCourseSection = new ScheduledStaffForCourseSection();
    reassignTeacherSchedule.courseSectionsList = JSON.parse(JSON.stringify(this.allScheduledTeacherBasedOnCourse.courseSectionsList));
    reassignTeacherSchedule.reScheduleStaffId = this.selectedNewTeacher.staffId;
    reassignTeacherSchedule.courseSectionsList = this.sendCheckedAndNonConflictedStaffsBasedOnCourse(reassignTeacherSchedule.courseSectionsList);

    this.teacherReassigning = true;
    this.teacherReassigningLoader = true;

    this.staffScheduleService.addStaffCourseSectionReScheduleByCourse(reassignTeacherSchedule).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        reassignTeacherSchedule.courseSectionsList = [];
      }
      else {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.teacherReassigning = false;
          if (res.courseSectionsList == null) {
            this.snackbar.open(res._message, '', {
              duration: 5000
            });
          } else {
            this.snackbar.open(res._message, '', {
              duration: 5000
            });
          }
        } else {
          this.resetAll();
        }

      }
      this.teacherReassigningLoader = false;
    })
  }

  sendCheckedAndNonConflictedStaffsBasedOnCourse(courseSectionList) {
    courseSectionList = courseSectionList.map((staffList) => {
      staffList.staffCoursesectionSchedule = staffList.staffCoursesectionSchedule.filter((staff) => {
        return (staff.checked && !staff.conflict)
      });
      return staffList;
    })

    courseSectionList = courseSectionList.filter((staffList) => {
      return staffList.staffCoursesectionSchedule.length > 0;
    })

    return courseSectionList;
  }


  singleSelectionBasedOnTeacher(event, indexOfCourseSection) {
    this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList[indexOfCourseSection].checked = event;
    this.isAllCoursesChecked = this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList.every((course) => {
      return course.checked;
    });
  }
  masterCheckToggleBasedOnTeacher(event) {
    this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList.forEach((item) => {
      item.checked = event;
    });
    this.isAllCoursesChecked = event;
  }

  checkAvailabilityBasedOnCourses() {
    this.checkAvailibilityBasedOnCourse.courseSectionsList = JSON.parse(JSON.stringify(this.allScheduledTeacherBasedOnCourse.courseSectionsList));
    this.checkAvailibilityBasedOnCourse.reScheduleStaffId = this.selectedNewTeacher.staffId;
    this.onlySendCheckedStaffBasedOnCourse();
    let noStaffSelected = this.checkAvailibilityBasedOnCourse.courseSectionsList.every(item => item.staffCoursesectionSchedule.length == 0);
    if (noStaffSelected) {
      this.snackbar.open('Please select at least one teacher', '', {
        duration: 5000
      });
      return
    }
    this.isAllCourseSectionConflicted = false;
    this.checkAvailibilityBasedOnCourse.conflictIndexNo = null;
    this.checkAvailibilityBasedOnCourse._failure = false;
    this.checkAvailabilityLoader = true;
    this.staffScheduleService.checkAvailabilityStaffCourseSectionReSchedule(this.checkAvailibilityBasedOnCourse).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        this.checkAvailibilityBasedOnCourse.courseSectionsList = [];
      }
      else {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          if (res.courseSectionsList == null) {
            this.snackbar.open(res._message, '', {
              duration: 5000
            });
          } else {
            this.noConflictDetected = false;
            this.checkAvailibilityBasedOnCourse = res;
            this.manipulateView();
            this.checkForConflictsBasedOnCourse();
          }
        } else {
          this.noConflictDetected = true;
          this.checkAvailibilityBasedOnCourse = res;
          this.manipulateView();
          this.checkForConflictsBasedOnCourse();
        }
        this.checkAvailabilityFinished = true
        this.checkAvailabilityLoader = false;
      }
    })
  }

  manipulateView() {
    for (let [courseSectionIndex, courseSection] of this.allScheduledTeacherBasedOnCourse.courseSectionsList.entries()) {
      if (courseSection.staffCoursesectionSchedule.length > 0) {
        for (let staff of courseSection.staffCoursesectionSchedule) {
          if (staff.checked) {
            let matchedStaffIndex = this.checkAvailibilityBasedOnCourse.courseSectionsList[courseSectionIndex].staffCoursesectionSchedule.findIndex(item => item.staffId == +staff.staffId);
            this.checkAvailibilityBasedOnCourse.courseSectionsList[courseSectionIndex].staffCoursesectionSchedule[matchedStaffIndex].checked = true;
          }
        }

      }
    }
  }
  checkForConflictsBasedOnCourse() {
    if (this.checkAvailibilityBasedOnCourse.conflictIndexNo?.trim()) {
      let conflictedIndex = []
      conflictedIndex = this.checkAvailibilityBasedOnCourse.conflictIndexNo.split(',');
      for (let conflictCourseSection of conflictedIndex) {
        let matchedStaffIndex = this.allScheduledTeacherBasedOnCourse.courseSectionsList[+conflictCourseSection].staffCoursesectionSchedule.findIndex(item => item.checked);
        this.allScheduledTeacherBasedOnCourse.courseSectionsList[+conflictCourseSection].staffCoursesectionSchedule[matchedStaffIndex].conflict = true;
      }
      let sentStaffCount = 0;
      this.checkAvailibilityBasedOnCourse.courseSectionsList.map((item) => {
        if (item.staffCoursesectionSchedule.length) {
          sentStaffCount = sentStaffCount + 1;
        }
      });
      if (sentStaffCount == conflictedIndex.length) {
        this.isAllCourseSectionConflicted = true;
      } else {
        this.isTeacherReassignPossible = true;
      }

      for (let courseSection of this.allScheduledTeacherBasedOnCourse.courseSectionsList) {
        for (let staff of courseSection.staffCoursesectionSchedule) {
          if (!staff.checked) {
            staff.conflict = false;
          }
        }
      }
      for (let [courseSectionIndex, courseSection] of this.allScheduledTeacherBasedOnCourse.courseSectionsList.entries()) {
        if (!conflictedIndex.includes(courseSectionIndex.toString())) {
          for (let staff of courseSection.staffCoursesectionSchedule) {
            if (staff.conflict) {
              staff.conflict = false;
            }
          }
        }

      }
    } else {
      for (let courseSection of this.allScheduledTeacherBasedOnCourse.courseSectionsList) {
        for (let staff of courseSection.staffCoursesectionSchedule) {
          staff.conflict = false;
        }
      }
      this.isTeacherReassignPossible = true;
    }
  }

  onlySendCheckedStaffBasedOnCourse() {
    this.checkAvailibilityBasedOnCourse.courseSectionsList = this.checkAvailibilityBasedOnCourse.courseSectionsList.map((staffList) => {
      staffList.staffCoursesectionSchedule = staffList.staffCoursesectionSchedule.filter(staff => staff.checked);
      return staffList;
    })
  }

singleSelectionBasedOnCourse(event, courseSectionIndex, checkedStaffIndex,ref) {
    event.preventDefault();
    if (!this.allScheduledTeacherBasedOnCourse.courseSectionsList[courseSectionIndex].staffCoursesectionSchedule[checkedStaffIndex].checked) {
      this.isTeacherReassignPossible = false;
      this.allScheduledTeacherBasedOnCourse.courseSectionsList[courseSectionIndex].staffCoursesectionSchedule[checkedStaffIndex].checked = true;
      ref.checked=true;
    }else{
      this.allScheduledTeacherBasedOnCourse.courseSectionsList[courseSectionIndex].staffCoursesectionSchedule[checkedStaffIndex].checked = false;
      ref.checked=false
    }
    for (let [i, staff] of this.allScheduledTeacherBasedOnCourse.courseSectionsList[courseSectionIndex].staffCoursesectionSchedule.entries()) {
      if (i != checkedStaffIndex) 
        staff.checked = false;
    }
  }

  //  Resetting all the variables after finishing a task
  resetAll() {
    this.noConflictDetected = false;
    this.selectedCurrentTeacher = null;
    this.teacherReassignmentBasedOnCourse = false;
    this.teacherReassignmentBasedOnTeacher = false;
    this.isTeacherReassignPossible = false;
    this.isAllCourseSectionConflicted = false;
    this.checkAvailabilityFinished = false;
    this.selectedCourse = null;
    this.disableMasterCheckboxBasedOnTeacherConflict = false;
  }

}
