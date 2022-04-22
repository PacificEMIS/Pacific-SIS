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

import { Component, Inject, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import icClose from '@iconify/icons-ic/close';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CourseManagerService } from 'src/app/services/course-manager.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MarkingPeriodService } from 'src/app/services/marking-period.service';
import { LoaderService } from 'src/app/services/loader.service';
import { CourseSectionService } from 'src/app/services/course-section.service';
import { CommonService } from 'src/app/services/common.service';
import { GetAllCourseListModel, GetAllProgramModel, GetAllSubjectModel, SearchCourseSectionViewModelForGroupDelete } from 'src/app/models/course-manager.model';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { GetMarkingPeriodTitleListModel, MarkingPeriodTitleList } from 'src/app/models/marking-period.model';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';

@Component({
  selector: 'vex-add-course-section',
  templateUrl: './add-course-section.component.html',
  styleUrls: ['./add-course-section.component.scss']
})
export class AddCourseSectionComponent implements OnInit, OnDestroy {
  icClose = icClose;
  displayedColumns: string[] = ['courseSection', 'course', 'markingPeriod', 'startDate', 'endDate', 'totalSeats', 'available'];
  searchRecord: boolean = false;
  courseDetails: MatTableDataSource<any>;
  programList = [];
  subjectList = [];
  courseList = [];
  markingPeriodList = [];
  loading: boolean;
  destroySubject$: Subject<void> = new Subject();
  getAllProgramModel: GetAllProgramModel = new GetAllProgramModel();
  getAllSubjectModel: GetAllSubjectModel = new GetAllSubjectModel();
  getAllCourseListModel: GetAllCourseListModel = new GetAllCourseListModel();
  getMarkingPeriodTitleListModel: GetMarkingPeriodTitleListModel = new GetMarkingPeriodTitleListModel();
  courseSectionSearch: SearchCourseSectionViewModelForGroupDelete = new SearchCourseSectionViewModelForGroupDelete();
  selectedMarkingPeriod: MarkingPeriodTitleList = new MarkingPeriodTitleList();
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;

  constructor(public translateService: TranslateService,
    private dialogRef: MatDialogRef<AddCourseSectionComponent>,
    private courseManagerService: CourseManagerService,
    private defaultService: DefaultValuesService,
    private snackbar: MatSnackBar,
    private markingPeriodService: MarkingPeriodService,
    private loaderService: LoaderService,
    private courseSectionService: CourseSectionService,
    private commonService: CommonService,
    @Inject(MAT_DIALOG_DATA) public data) {
    this.courseList = this.data.courseList;
    this.subjectList = this.data.subjectList;
    this.programList = this.data.programList;
    this.getMarkingPeriodTitleListModel.getMarkingPeriodView = this.data.markingPeriods;

    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
  }

  ngOnInit(): void {
  }

  changeMarkingPeriod(markingPeriodId) {
    let index = this.getMarkingPeriodTitleListModel.getMarkingPeriodView.findIndex((item) => {
      return item.value == markingPeriodId;
    });
    if (index !== -1) {
      this.selectedMarkingPeriod = this.getMarkingPeriodTitleListModel.getMarkingPeriodView[index];
    } else {
      this.selectedMarkingPeriod.value = null;
    }
  }

  searchCourseSection() {
    this.searchRecord = true;
    let cloneCourseSectionSearch: SearchCourseSectionViewModelForGroupDelete = new SearchCourseSectionViewModelForGroupDelete();
    cloneCourseSectionSearch = JSON.parse(JSON.stringify(this.courseSectionSearch));
    cloneCourseSectionSearch.courseSubject = cloneCourseSectionSearch.courseSubject ? cloneCourseSectionSearch.courseSubject : null;
    cloneCourseSectionSearch.courseProgram = cloneCourseSectionSearch.courseProgram ? cloneCourseSectionSearch.courseProgram : null;
    cloneCourseSectionSearch.courseId = cloneCourseSectionSearch.courseId ? cloneCourseSectionSearch.courseId : null;
    cloneCourseSectionSearch.markingPeriodId = cloneCourseSectionSearch.markingPeriodId ? cloneCourseSectionSearch.markingPeriodId : null;
    if (cloneCourseSectionSearch.markingPeriodId) {
      cloneCourseSectionSearch.markingPeriodStartDate = null;
    } else {
      let mpStartDate = this.defaultService.getMarkingPeriodStartDate();
      cloneCourseSectionSearch.markingPeriodStartDate = mpStartDate ? mpStartDate.split('T')[0] : null;
    }
    cloneCourseSectionSearch.forStudent = true;
    this.courseSectionService.searchCourseSectionForScheduleForGroupDelete(cloneCourseSectionSearch).subscribe((res) => {
      if (res) {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          if (res.allCourseSectionViewList === null) {
            this.courseDetails = new MatTableDataSource([]);
            this.snackbar.open(res._message, '', {
              duration: 5000
            });
          } else {
            this.courseDetails = new MatTableDataSource([]);
          }
        } else {
          res.allCourseSectionViewList = this.findMarkingPeriodTitleById(res.allCourseSectionViewList);
          this.courseDetails = new MatTableDataSource(res.allCourseSectionViewList);
          this.courseDetails.paginator = this.paginator;
        }
      } else {
        this.snackbar.open(this.defaultService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  findMarkingPeriodTitleById(courseDetails) {
    courseDetails = courseDetails.map((item) => {
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
      return item;
    });
    return courseDetails;
  }

  cellClicked(element) {
    this.dialogRef.close(element);
  }

  // For destroy the isLoading subject.
  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
