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
import icSearch from '@iconify/icons-ic/search';
import { MatSnackBar } from '@angular/material/snack-bar';
import { GetMarkingPeriodTitleListModel } from '../../../models/marking-period.model';
import { MarkingPeriodService } from '../../../services/marking-period.service';
import { CommonService } from '../../../services/common.service';
import { AllScheduledCourseSectionForStaffModel } from '../../../models/teacher-schedule.model';
import { TeacherScheduleService } from '../../../services/teacher-schedule.service';
import { StudentEffortGradeListModel, StudentEffortGradeMaster } from '../../../models/student-effort-grade.model';
import { EffotrGradeService } from '../../../services/effort-grade.service';
import { StudentScheduleService } from '../../../services/student-schedule.service';
import { GradesService } from '../../../services/grades.service';
import { DefaultValuesService } from '../../../common/default-values.service';
import { ScheduleStudentForView, ScheduleStudentListViewModel } from '../../../models/student-schedule.model';
import { EffortGradeLibraryCategoryListView, GetAllEffortGradeScaleListModel } from '../../../models/grades.model';
import { map } from 'rxjs/operators';
import { LoaderService } from '../../../services/loader.service';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { fadeInRight400ms } from 'src/@vex/animations/fade-in-right.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { SearchFilter, SearchFilterListViewModel } from '../../../models/search-filter.model';
import { ProfilesTypes } from '../../../enums/profiles.enum';

@Component({
  selector: 'vex-input-effort-grades',
  templateUrl: './input-effort-grades.component.html',
  styleUrls: ['./input-effort-grades.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms,
    fadeInRight400ms
  ]
})
export class InputEffortGradesComponent implements OnInit {
  icSearch = icSearch;
  viewDetailsModal = 0;
  showMessage = "pleaseSelectCourseSectionForEffortGrade";
  selectedStudent: number;
  showEffort: boolean = false;
  courseSectionId: number;
  courseSectionData;
  nameSearch: string = '';
  totalCount: number = 0;
  loading: boolean = false;
  showStudentList: boolean;
  showLoadFilter = true;
  filterJsonParams;
  searchCount: number;
  showAdvanceSearchPanel: boolean = false;
  searchValue: any;
  toggleValues: any;
  parentData;
  profiles= ProfilesTypes;
  markingPeriodList = [];
  effortGradeDetailViewList = [];
  effortCategoriesList = [];
  effortGradeScaleModelList = [];
  studentMasterList: ScheduleStudentForView[];
  searchFilter: SearchFilter = new SearchFilter();
  searchFilterListViewModel: SearchFilterListViewModel = new SearchFilterListViewModel();
  getEffortGradeScaleList: GetAllEffortGradeScaleListModel = new GetAllEffortGradeScaleListModel();
  getMarkingPeriodTitleListModel: GetMarkingPeriodTitleListModel = new GetMarkingPeriodTitleListModel();
  allScheduledCourseSectionBasedOnTeacher: AllScheduledCourseSectionForStaffModel = new AllScheduledCourseSectionForStaffModel();
  studentEffortGradeListModel: StudentEffortGradeListModel = new StudentEffortGradeListModel();
  effortGradeLibraryCategoryListView: EffortGradeLibraryCategoryListView = new EffortGradeLibraryCategoryListView();
  scheduleStudentListViewModel: ScheduleStudentListViewModel = new ScheduleStudentListViewModel();

  constructor(
    public translateService: TranslateService,
    private dialog: MatDialog,
    private snackbar: MatSnackBar,
    private markingPeriodService: MarkingPeriodService,
    private commonService: CommonService,
    private teacherReassignmentService: TeacherScheduleService,
    private effotrGradeService: EffotrGradeService,
    private studentScheduleService: StudentScheduleService,
    private gradesService: GradesService,
    private defaultValuesService: DefaultValuesService,
    private loaderService: LoaderService,
  ) {
    // translateService.use("en");
    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    });
  }

  ngOnInit(): void {
    this.allScheduledCourseSectionBasedOnTeacher.staffId = +this.defaultValuesService.getUserId();
    this.getAllMarkingPeriodList();
    this.getAllEffortGradeLlibraryCategoryList();
  }

  closeDetailsModal() {
    this.viewDetailsModal = 0;
  }

  getAllMarkingPeriodList() {
    this.getMarkingPeriodTitleListModel.schoolId = this.defaultValuesService.getSchoolID();
    this.getMarkingPeriodTitleListModel.academicYear = this.defaultValuesService.getAcademicYear();
    this.markingPeriodService.getAllMarkingPeriodList(this.getMarkingPeriodTitleListModel).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
        this.getMarkingPeriodTitleListModel.getMarkingPeriodView = [];
        if (!this.getMarkingPeriodTitleListModel?.getMarkingPeriodView) {
          this.snackbar.open(data._message, '', {
            duration: 1000
          });
        }
      } else {
        this.getMarkingPeriodTitleListModel.getMarkingPeriodView = data.getMarkingPeriodView;
        this.getAllScheduledCourseSectionBasedOnTeacher();
      }
    });
  }

  getAllScheduledCourseSectionBasedOnTeacher() {
    //this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList = null;
    this.teacherReassignmentService.getAllScheduledCourseSectionForStaff(this.allScheduledCourseSectionBasedOnTeacher).pipe(
      map((res) => {
        res._userName = this.defaultValuesService.getUserName();
        return res;
      })
    ).subscribe((res) => {
      if (res) {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList = [];
          if (!res.courseSectionViewList) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        } else {
          this.allScheduledCourseSectionBasedOnTeacher = res;
          this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList = this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList.filter(x => x.gradeScaleType !== 'Ungraded');

        }
      }
      else {
        this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });

      }
    })
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
    this.studentMasterList.map((item: any) => {
      let isDisplay = false;
      res.scheduleStudentForView.map((subItem) => {
        if (item.studentId === subItem.studentId) {
          isDisplay = true;
        }
      })
      item.isDisplay = isDisplay;
    })
    if (this.studentMasterList.every((x: any) => !x.isDisplay)) {
      this.showMessage = 'noRecordFound';
      this.showEffort = false;
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

  selectedCourseSection(courseSection) {
    this.courseSectionId = courseSection;
    this.selectedStudent = null;
    this.showEffort = false;
    this.parentData = { courseSectionData: courseSection };
    let courseSectionDetails = this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList.filter(x => x.courseSectionId === +this.courseSectionId);
    this.courseSectionData = this.findMarkingPeriodTitleById(courseSectionDetails[0]);
    this.markingPeriodList = this.getMarkingPeriodTitleListModel.getMarkingPeriodView.filter(x => x.value === this.courseSectionData.markingPeriodId);
    this.studentEffortGradeListModel.markingPeriodId = this.getMarkingPeriodTitleListModel.getMarkingPeriodView.filter(x => x.value === this.courseSectionData.markingPeriodId)[0].value;
    this.studentEffortGradeListModel.schoolId = this.defaultValuesService.getSchoolID();
    this.studentEffortGradeListModel.tenantId = this.defaultValuesService.getTenantID();
    this.studentEffortGradeListModel.courseId = courseSectionDetails[0].courseId;
    this.studentEffortGradeListModel.courseSectionId = courseSectionDetails[0].courseSectionId;
    this.studentEffortGradeListModel.calendarId = courseSectionDetails[0].calendarId;
    this.effotrGradeService.getAllStudentEffortGradeList(this.studentEffortGradeListModel).subscribe((res) => {
      if (res) {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          this.studentEffortGradeListModel.courseId = courseSectionDetails[0].courseId;
          this.studentEffortGradeListModel.calendarId = courseSectionDetails[0].calendarId;
          this.searchScheduledStudentForGroupDrop(courseSectionDetails[0].courseSectionId);
        }
        else {
          this.scheduleStudentListViewModel.courseSectionId = courseSectionDetails[0].courseSectionId;
          this.scheduleStudentListViewModel.profilePhoto = true;
          this.studentScheduleService.searchScheduledStudentForGroupDrop(this.scheduleStudentListViewModel).subscribe((res) => {
            if (res) {
              if (res._failure) {
                this.commonService.checkTokenValidOrNot(res._message);
                this.showMessage = 'noRecordFound';
              } else {
                res.scheduleStudentForView.map((item: any) => {
                  item.isDisplay = true;
                })
                this.studentMasterList = res.scheduleStudentForView;
                this.totalCount = this.studentMasterList.length;
                if (this.studentMasterList.length === 0) {
                  this.showMessage = 'noRecordFound';
                } else {
                  this.getAllSearchFilter();
                }
              }
            }
          });
          this.studentEffortGradeListModel = res;
        }
      }
      else {
        this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  findMarkingPeriodTitleById(courseDetails) {
    if (courseDetails.yrMarkingPeriodId) {
      courseDetails.markingPeriodId = '0_' + courseDetails.yrMarkingPeriodId;
    } else if (courseDetails.smstrMarkingPeriodId) {
      courseDetails.markingPeriodId = '1_' + courseDetails.smstrMarkingPeriodId;
    } else if (courseDetails.qtrMarkingPeriodId) {
      courseDetails.markingPeriodId = '2_' + courseDetails.qtrMarkingPeriodId;
    } else if (courseDetails.prgrsprdMarkingPeriodId) {
      courseDetails.markingPeriodId = '3_' + courseDetails.prgrsprdMarkingPeriodId;
    }
    else {
      courseDetails.markingPeriodId = this.getMarkingPeriodTitleListModel.getMarkingPeriodView[0].value;
    }
    return courseDetails;
  }

  searchScheduledStudentForGroupDrop(courseSectionId) {
    this.scheduleStudentListViewModel.courseSectionId = courseSectionId;
    this.scheduleStudentListViewModel.profilePhoto = true;
    this.studentScheduleService.searchScheduledStudentForGroupDrop(this.scheduleStudentListViewModel).subscribe((res) => {
      if (res) {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          this.showMessage = 'noRecordFound';
        } else {
          this.studentMasterList = res.scheduleStudentForView;
          this.totalCount = this.studentMasterList.length;
          if (this.studentMasterList.length === 0) {
            this.showMessage = 'noRecordFound';
          } else {
            this.getAllSearchFilter();
          }
          this.studentEffortGradeListModel.studentEffortGradeList = [new StudentEffortGradeMaster()];
          this.studentMasterList.map((item, i) => {
            this.initializeDefaultValues(item, i);
            this.studentEffortGradeListModel.studentEffortGradeList.push(new StudentEffortGradeMaster());
          });
          this.scheduleStudentListViewModel = new ScheduleStudentListViewModel();
          this.studentEffortGradeListModel.studentEffortGradeList.pop();
        }
      }
      else {
        this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  getAllEffortGradeLlibraryCategoryList() {
    this.gradesService.getAllEffortGradeLlibraryCategoryList(this.effortGradeLibraryCategoryListView).subscribe(
      (res: EffortGradeLibraryCategoryListView) => {
        if (typeof (res) == 'undefined') {
          this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
        else {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            this.effortCategoriesList = null
            if (!res.effortGradeLibraryCategoryList) {
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
            }
          }
          else {
            this.effortCategoriesList = res.effortGradeLibraryCategoryList;
            this.getAllEffortGradeScale();
            for (let category of this.effortCategoriesList) {
              for (let item of category.effortGradeLibraryCategoryItem) {
                this.effortGradeDetailViewList.push({ categoryName: category.categoryName, effortCategoryId: item.effortCategoryId, effortItemTitle: item.effortItemTitle, effortItemId: item.effortItemId });
              }
            }
          }
        }
      }
    );
  }

  viewDetails(id) {
    this.selectedStudent = id;
    this.showEffort = true;
    if (this.viewDetailsModal === 0) {
      this.viewDetailsModal = 1;
    } else {
      this.viewDetailsModal = 0;
    }
  }

  selectCategory(item, index, value) {
    this.studentEffortGradeListModel.studentEffortGradeList[this.selectedStudent].studentEffortGradeDetail[index] = { effortCategoryId: item.effortCategoryId, effortItemId: item.effortItemId, effortGradeScaleId: JSON.parse(value.target.value) };
  }

  getAllEffortGradeScale() {
    if (this.getEffortGradeScaleList.sortingModel?.sortColumn == "") {
      this.getEffortGradeScaleList.sortingModel.sortColumn = "sortOrder"
      this.getEffortGradeScaleList.sortingModel.sortDirection = "asc"
    }

    this.gradesService.getAllEffortGradeScaleList(this.getEffortGradeScaleList).subscribe(data => {
      if (data) {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          this.effortGradeScaleModelList = null;
          if (!data.effortGradeScaleList) {
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          }
        } else {
          this.effortGradeScaleModelList = data.effortGradeScaleList;

          if (this.effortGradeScaleModelList.length > 0 && this.effortCategoriesList.length > 0) {
            this.showStudentList = false;
          }
        }
      }
      else {
        this.snackbar.open(data._message, '', {
          duration: 10000
        });
      }
    });
  }

  initializeDefaultValues(item, i) {
    for (let category of this.effortGradeDetailViewList) {
      this.studentEffortGradeListModel.studentEffortGradeList[i].studentEffortGradeDetail.push({ effortCategoryId: null, effortItemId: null, effortGradeScaleId: null });
    }
    this.studentEffortGradeListModel.studentEffortGradeList[i].studentId = item.studentId;
    this.studentEffortGradeListModel.studentEffortGradeList[i].studentFinalGradeSrlno = 0;
    this.studentEffortGradeListModel.studentEffortGradeList[i].teacherComment = null;
  }

  submitEffortGrade() {
    delete this.studentEffortGradeListModel.academicYear;
    this.effotrGradeService.addUpdateStudentEffortGrade(this.studentEffortGradeListModel).subscribe(data => {
      if (data) {

        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          if (data.studentEffortGradeList == null) {

            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          } else {

          }
        } else {
          // this.effortGradeScaleModelList = data.studentEffortGradeList;
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
        }
      }
      else {
        this.snackbar.open(data._message, '', {
          duration: 10000
        });
      }
    });
  }

  getAllSearchFilter() {
    this.searchFilterListViewModel.module = 'Student';
    this.commonService.getAllSearchFilter(this.searchFilterListViewModel).subscribe((res) => {
      if (typeof (res) === 'undefined') {
        this.snackbar.open('Filter list failed. ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
      else {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          this.searchFilterListViewModel.searchFilterList = []
          if (!res.searchFilterList) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        }
        else {
          this.searchFilterListViewModel = res;
          let filterData = this.searchFilterListViewModel.searchFilterList.filter(x => x.filterId == this.searchFilter.filterId);
          if (filterData.length > 0) {
            this.searchFilter.jsonList = filterData[0].jsonList;
          }
          if (this.filterJsonParams == null) {

            this.searchFilter = this.searchFilterListViewModel.searchFilterList[this.searchFilterListViewModel.searchFilterList.length - 1];
          }
        }
      }
    }
    );
  }

  searchByFilterName(filter) {
    this.searchFilter = filter;

    if (this.defaultValuesService.getUserMembershipType() === this.profiles.HomeroomTeacher || this.defaultValuesService.getUserMembershipType() === this.profiles.Teacher) {
      this.scheduleStudentListViewModel.courseSectionId = this.courseSectionId;
      this.scheduleStudentListViewModel.profilePhoto = true;
      this.scheduleStudentListViewModel.filterParams = JSON.parse(filter.jsonList);
      this.studentScheduleService.searchScheduledStudentForGroupDrop(this.scheduleStudentListViewModel).subscribe((res) => {
        if (res) {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            this.showMessage = 'noRecordFound';
          } else {
            this.getSearchResult(res);
            this.scheduleStudentListViewModel.filterParams = null;
          }
        }
        else {
          this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      });
    }
    //   // else{
    //   //   this.getAllStudent.filterParams = JSON.parse(filter.jsonList);
    //   //   this.getAllStudent.sortingModel = null;
    //   //   this.studentService.GetAllStudentList(this.getAllStudent).subscribe(data => {
    //   //    if(data._failure){
    //   //     this.commonService.checkTokenValidOrNot(data._message);
    //   //       if (data.studentListViews === null) {
    //   //         this.StudentModelList = new MatTableDataSource([]);
    //   //         this.snackbar.open(data._message, '', {
    //   //           duration: 10000
    //   //         });
    //   //       } else {
    //   //         this.StudentModelList = new MatTableDataSource([]);
    //   //       }
    //   //     } else {
    //   //       this.totalCount = data.totalCount;
    //   //       this.pageNumber = data.pageNumber;
    //   //       this.pageSize = data._pageSize;
    //   //       this.StudentModelList = new MatTableDataSource(data.studentListViews);
    //   //       this.getAllStudent = new StudentListModel();
    //   //     }
    //   //   });
    //   // }
  }

  showAdvanceSearch() {
    this.showAdvanceSearchPanel = true;
    this.filterJsonParams = null;
  }

  hideAdvanceSearch(event) {
    this.showAdvanceSearchPanel = false;
  }

}
