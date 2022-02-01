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

import { AfterViewInit, Component, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import icSearch from "@iconify/icons-ic/search";
import { StaffPortalService } from '../../../../services/staff-portal.service';
import { ScheduledCourseSectionViewModel } from '../../../../models/dashboard.model';
import { GetAllStaffModel } from '../../../../models/staff.model';
import { AllCourseSectionView } from '../../../../models/course-manager.model';
import { MatTableDataSource } from '@angular/material/table';
import { Subject } from 'rxjs';
import { FormControl } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { fadeInUp400ms } from '../../../../../@vex/animations/fade-in-up.animation';
import { stagger40ms } from '../../../../../@vex/animations/stagger.animation';
import { fadeInRight400ms } from '../../../../../@vex/animations/fade-in-right.animation';
import { CommonService } from 'src/app/services/common.service';

import { DefaultValuesService } from 'src/app/common/default-values.service';
@Component({
  selector: 'vex-missing-attendance-list',
  templateUrl: './missing-attendance-list.component.html',
  styleUrls: ['./missing-attendance-list.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms,
    fadeInRight400ms
  ]
})
export class MissingAttendanceListComponent implements OnInit, AfterViewInit, OnDestroy {

  icSearch = icSearch;
  @Output() showTakeAttendance = new EventEmitter<any>();
  @Input() courseSectionClass;
  scheduledCourseSectionViewModel: ScheduledCourseSectionViewModel = new ScheduledCourseSectionViewModel();
  getAllStaffModel: GetAllStaffModel = new GetAllStaffModel();
  staffDetails;
  courseSectionViewList: MatTableDataSource<AllCourseSectionView>;
  loading: boolean;
  destroySubject$: Subject<void> = new Subject();
  totalCount: number = 0;
  pageSize: number;
  missingAttendanceDateList = [];
  searchKeyword: string;
  pageNumber: number;
  searchCtrl: FormControl;
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  columns = [
    { label: 'date', property: 'date', type: 'text', visible: true },
    { label: 'courseSectionName', property: 'courseSectionName', type: 'text', visible: true },
    { label: 'period', property: 'period', type: 'text', visible: true },
    { label: 'action', property: 'action', type: 'text', visible: true },

  ];

  constructor(
    public translateService: TranslateService,
    private staffPortalService: StaffPortalService,
    private snackbar: MatSnackBar,
    private commonService: CommonService,
    private defaultValuesService: DefaultValuesService
    ) {
  }

  ngOnInit(): void {
    this.searchCtrl = new FormControl();
    this.missingAttendanceListForCourseSection();
  }

  takeAttendance(element,courseSection) {
    this.staffPortalService.setCourseSectionDetails(courseSection);
    this.showTakeAttendance.emit(element);
   
  }

  missingAttendanceListForCourseSection() {
    this.getAllStaffModel.sortingModel = null;
    this.getAllStaffModel.staffId = this.defaultValuesService.getUserId();
    this.getAllStaffModel.courseSectionId = this.defaultValuesService.getCourseSectionId();
    this.staffPortalService.missingAttendanceListForCourseSection(this.getAllStaffModel).subscribe((res: ScheduledCourseSectionViewModel) => {
      if (res) {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.courseSectionViewList = new MatTableDataSource([]);
          if (!res.courseSectionViewList) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        }
        else {
          this.totalCount = res.missingAttendanceCount;
          this.pageNumber = res.pageNumber;
          this.pageSize = res._pageSize;
          this.courseSectionViewList = new MatTableDataSource(res.courseSectionViewList);
        }
      }
      else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }


  ngAfterViewInit() {
    this.getAllStaffModel = new GetAllStaffModel();
    this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term != '') {
        this.callWithFilterValue(term);
      } else {
        this.callWithoutFilterValue()
      }
    });
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
      Object.assign(this.getAllStaffModel, { filterParams: filterParams });
    }
    this.getAllStaffModel.pageNumber = event.pageIndex + 1;
    this.getAllStaffModel.pageSize = event.pageSize;
    this.missingAttendanceListForCourseSection();
  }


  callWithFilterValue(term) {
    let filterParams = [
      {
        columnName: null,
        filterValue: term,
        filterOption: 4
      }
    ]
    Object.assign(this.getAllStaffModel, { filterParams: filterParams });
    this.getAllStaffModel.pageNumber = 1;
    this.paginator.pageIndex = 0;
    this.getAllStaffModel.pageSize = this.pageSize;
    this.missingAttendanceListForCourseSection();
  }

  callWithoutFilterValue() {
    Object.assign(this.getAllStaffModel, { filterParams: null });
    this.getAllStaffModel.pageNumber = this.paginator.pageIndex + 1;
    this.getAllStaffModel.pageSize = this.pageSize;
    this.missingAttendanceListForCourseSection();
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}
