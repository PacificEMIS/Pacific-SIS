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
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { map, takeUntil } from 'rxjs/operators';
import { DefaultValuesService } from '../../../../../common/default-values.service';
import { EffortGradeLibraryCategoryListView, GetAllEffortGradeScaleListModel } from '../../../../../models/grades.model';
import { GetStudentListByHomeRoomStaffModel, StudentEffortGradeMaster } from '../../../../../models/student-effort-grade.model';
import { EffotrGradeService } from '../../../../../services/effort-grade.service';
import { FinalGradeService } from '../../../../../services/final-grade.service';
import { GradesService } from '../../../../../services/grades.service';
import { LoaderService } from '../../../../../services/loader.service';
import { MarkingPeriodService } from '../../../../../services/marking-period.service';
import { StudentScheduleService } from '../../../../../services/student-schedule.service';
import { TeacherScheduleService } from '../../../../../services/teacher-schedule.service';
import { fadeInRight400ms } from '../../../../../../@vex/animations/fade-in-right.animation';
import { fadeInUp400ms } from '../../../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../../../@vex/animations/stagger.animation';
import { CommonService } from 'src/app/services/common.service';
import { Subject } from 'rxjs';
import { ProfilesTypes } from 'src/app/enums/profiles.enum';


@Component({
  selector: 'vex-effort-grade-details',
  templateUrl: './effort-grade-details.component.html',
  styleUrls: ['./effort-grade-details.component.scss'],
  animations: [
    fadeInRight400ms,
    stagger60ms,
    fadeInUp400ms
  ]
})
export class EffortGradeDetailsComponent implements OnInit, OnDestroy {
  pageStatus = "Grade Details";
  staffDetails;
  showMessage;
  showStudentList: boolean;
  selectedStudent: number;
  viewDetailsModal: number = 0;
  loading: boolean = false;
  destroySubject$: Subject<void> = new Subject();
  totalCount: number;
  courseSectionId: number;
  showEffort: boolean = false;
  effortCategoriesList = [];
  effortGradeDetailViewList = [];
  effortGradeScaleModelList = [];
  studentMasterList: StudentEffortGradeMaster[];
  profiles = ProfilesTypes;
  getEffortGradeScaleList: GetAllEffortGradeScaleListModel = new GetAllEffortGradeScaleListModel();
  effortGradeLibraryCategoryListView: EffortGradeLibraryCategoryListView = new EffortGradeLibraryCategoryListView();
  getStudentListByHomeRoomStaffModel: GetStudentListByHomeRoomStaffModel = new GetStudentListByHomeRoomStaffModel();
  addUpdateStudentEffortGradeModel: GetStudentListByHomeRoomStaffModel = new GetStudentListByHomeRoomStaffModel();

  constructor(public translateService: TranslateService,
    private finalGradeService: FinalGradeService,
    private snackbar: MatSnackBar,
    private router: Router,
    public defaultValuesService: DefaultValuesService,
    private loaderService: LoaderService,
    private gradesService: GradesService,
    private commonService: CommonService,
    private effotrGradeService: EffotrGradeService
  ) {
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
    //translateService.use('en');
    this.staffDetails = this.finalGradeService.getStaffDetails();
  }

  ngOnInit(): void {
    if (this.defaultValuesService.getUserMembershipType() === this.profiles.HomeroomTeacher) {
      this.getStudentListByHomeRoomStaffModel.staffId = parseInt(this.defaultValuesService.getUserId());
    } else {
      this.getStudentListByHomeRoomStaffModel.staffId = this.staffDetails.staffId;
    }
    if (!this.getStudentListByHomeRoomStaffModel.staffId && this.defaultValuesService.getUserMembershipType() !== this.profiles.HomeroomTeacher) {
      this.router.navigate(['/school', 'staff', 'teacher-functions', 'input-effort-grade']);
    }
    this.getAllEffortGradeLlibraryCategoryList();
  }

  goToAddEffortGrade() {
    this.router.navigate(['/school', 'settings', 'grade-settings']);
    this.defaultValuesService.setPageId('Effort Grade Setup');
  }

  closeDetailsModal() {
    this.viewDetailsModal = 0;
  }

  getAllEffortGradeLlibraryCategoryList() {
    this.gradesService.getAllEffortGradeLlibraryCategoryList(this.effortGradeLibraryCategoryListView).subscribe(
      (res: EffortGradeLibraryCategoryListView) => {
        if (res) {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            this.effortCategoriesList = null;
            this.showStudentList = false;
            if (!res.effortGradeLibraryCategoryList) {
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
            }
          }
          else {
            if (!res.effortGradeLibraryCategoryList.every(item => item.effortGradeLibraryCategoryItem.length === 0)) {
              this.effortCategoriesList = res.effortGradeLibraryCategoryList.filter(item => item.effortGradeLibraryCategoryItem.length > 0);
              for (let category of this.effortCategoriesList) {
                for (let item of category.effortGradeLibraryCategoryItem) {
                  this.effortGradeDetailViewList.push({ categoryName: category.categoryName, effortCategoryId: item.effortCategoryId, effortItemTitle: item.effortItemTitle, effortItemId: item.effortItemId });
                }
              }
              this.getAllEffortGradeScale();
            } else {
              this.showStudentList = false;
            }
          }
        } else {
          this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
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
    this.addUpdateStudentEffortGradeModel.studentsByHomeRoomStaffView[this.selectedStudent].studentEffortGradeDetail[index] = { effortCategoryId: item.effortCategoryId, effortItemId: item.effortItemId, effortGradeScaleId: JSON.parse(value.target.value) };
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
          this.showStudentList = false;
          if (!data.effortGradeScaleList) {
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          }
        } else {
          this.effortGradeScaleModelList = data.effortGradeScaleList;
          if (this.effortGradeScaleModelList.length > 0 && this.effortCategoriesList.length > 0) {
            this.showStudentList = true;
            this.getStudentListByHomeRoomStaff();
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

  getStudentListByHomeRoomStaff() {
    this.effotrGradeService.getStudentListByHomeRoomStaff(this.getStudentListByHomeRoomStaffModel).subscribe(res => {
      if (res) {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          this.studentMasterList = [];
          this.totalCount = null;
          this.showMessage = 'noStudentsFound';
        } else {
          this.totalCount = res.studentsByHomeRoomStaffView.length;
          if (!this.totalCount) {
            this.showMessage = 'noStudentsFound';
            this.studentMasterList = [];
          } else {
            this.addUpdateStudentEffortGradeModel = res;
            this.addUpdateStudentEffortGradeModel.studentsByHomeRoomStaffView.map((item, index) => {
              if (item.studentEffortGradeDetail.length) {
                this.initializeValues(item, index);
              } else {
                this.initializeDefaultValues(item, index);
              }
            });
            this.studentMasterList = res.studentsByHomeRoomStaffView;
          }
        }
      } else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  // If student has already Grade
  initializeValues(data, i) {
    const studentEffortGradeDetail = data.studentEffortGradeDetail;
    this.addUpdateStudentEffortGradeModel.studentsByHomeRoomStaffView[i].studentEffortGradeDetail = [];
    for (let category of this.effortGradeDetailViewList) {
      this.addUpdateStudentEffortGradeModel.studentsByHomeRoomStaffView[i].studentEffortGradeDetail.push({ effortCategoryId: category.effortCategoryId, effortItemId: category.effortItemId, effortGradeScaleId: null });
    }

    studentEffortGradeDetail.map((item) => {
      this.addUpdateStudentEffortGradeModel.studentsByHomeRoomStaffView[i].studentEffortGradeDetail.map((effortItem, index) => {
        if ((item.effortCategoryId === effortItem.effortCategoryId) && (item.effortItemId === effortItem.effortItemId)) {
          this.addUpdateStudentEffortGradeModel.studentsByHomeRoomStaffView[i].studentEffortGradeDetail[index] = item;
        }
      });
    });
  }

  // If student has not any grade
  initializeDefaultValues(data, i) {
    for (let category of this.effortGradeDetailViewList) {
      this.addUpdateStudentEffortGradeModel.studentsByHomeRoomStaffView[i].studentEffortGradeDetail.push({ effortCategoryId: category.effortCategoryId, effortItemId: category.effortItemId, effortGradeScaleId: null });
    }
  }

  submitEffortGrade() {
    this.effotrGradeService.addUpdateStudentEffortGrade(this.addUpdateStudentEffortGradeModel).subscribe(data => {
      if (data) {
        if (data._failure) {
          this.commonService.checkTokenValidOrNot(data._message);
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
        } else {
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
          this.getStudentListByHomeRoomStaff();
        }
      }
      else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  // For destroy the isLoading subject.
  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}
