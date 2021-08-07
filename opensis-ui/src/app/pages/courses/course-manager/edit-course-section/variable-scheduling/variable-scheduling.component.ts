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

import { Component, OnInit, ViewChild, Input, OnChanges, EventEmitter, Output, SimpleChanges, ChangeDetectionStrategy, ChangeDetectorRef, AfterViewChecked } from '@angular/core';
import icClose from '@iconify/icons-ic/twotone-close';
import icPlusCircle from '@iconify/icons-ic/add-circle-outline';
import { weekDay } from '../../../../../enums/day.enum';
import { SchoolPeriodService } from '../../../../../services/school-period.service';
import { BlockListViewModel } from '../../../../../models/school-period.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { RoomService } from '../../../../../services/room.service';
import { RoomListViewModel } from '../../../../../models/room.model';
import { CourseVariableSchedule, OutputEmitDataFormat, CourseSectionAddViewModel, DeleteCourseSectionSchedule } from '../../../../../models/course-section.model';
import { CourseSectionService } from '../../../../../services/course-section.service';
import { map } from 'rxjs/operators';
import { NgForm } from '@angular/forms';
import { weeks } from '../../../../../common/static-data';
import { CommonService } from 'src/app/services/common.service';
@Component({
  selector: 'vex-variable-scheduling',
  templateUrl: './variable-scheduling.component.html',
  styleUrls: ['./variable-scheduling.component.scss'],
})
export class VariableSchedulingComponent implements OnInit, OnChanges {
  @Input() selectedCalendar;
  @Input() seatChangeFlag;
  icClose = icClose;
  icPlusCircle = icPlusCircle;
  variableScheduleList = [];
  blockListViewModel: BlockListViewModel = new BlockListViewModel();
  roomListViewModel: RoomListViewModel = new RoomListViewModel();
  courseSectionAddViewModel: CourseSectionAddViewModel = new CourseSectionAddViewModel();

  selected = null;
  selectedBlocks = [];
  selectedPeriod = []
  divCount = [];
  weekDaysList = weeks;
  filterDays;
  periodList = [];
  selectedRooms = [];
  roomIdWithCapacity = [];
  @ViewChild('form') currentForm: NgForm;
  @Input() detailsFromParentModal;
  @Output() variableScheduleData = new EventEmitter<OutputEmitDataFormat>();

  constructor(private snackbar: MatSnackBar,
    private schoolPeriodService: SchoolPeriodService,
    private roomService: RoomService,
    private courseSectionService: CourseSectionService,
    private cdr: ChangeDetectorRef,
    private commonService: CommonService,
    ) {
    this.courseSectionService.currentUpdate.subscribe((res) => {
      if (res) {
        this.sendVariableScheduleDataToParent();
      }
    })
  }

  ngOnInit(): void {
    this.getAllBlockList();
    this.getAllRooms();

    if (this.detailsFromParentModal.editMode) {
      for (let i = 0; i < this.detailsFromParentModal.courseSectionDetails.courseVariableSchedule.length; i++) {
        this.courseSectionAddViewModel.courseVariableScheduleList[i] = this.detailsFromParentModal.courseSectionDetails.courseVariableSchedule[i];
        this.weekDaysList.map(val => {
          if (this.courseSectionAddViewModel.courseVariableScheduleList[i].day === val.name) {
            this.courseSectionAddViewModel.courseVariableScheduleList[i].day = val.name;
          }
        })
        this.divCount[i] = i;
      }


    }

  }
  ngOnChanges(changes: SimpleChanges): void {
    if (this.selectedCalendar?.days) {
      this.getDays(this.selectedCalendar.days);
    }
  }


  getDays(days: string) {
    const calendarDays = days;
    let splitDays = calendarDays?.split('').map(x => +x).sort();
    let permittedWeekDays = []
    for(let item of weeks){
      for(let day of splitDays){
        if(day === item.id){
          permittedWeekDays.push(item);
          break;
        }
      }
    }
    this.weekDaysList = permittedWeekDays;
    if(!this.detailsFromParentModal.editMode){
      this.courseSectionAddViewModel.courseVariableScheduleList.length=0;
      this.courseSectionAddViewModel.courseVariableScheduleList=[new CourseVariableSchedule()]
      this.divCount.length = 0;
      this.pushDaysInAddMode()
    }
  }

  pushDaysInAddMode(){

      this.weekDaysList.map((item, i) => {
        this.divCount.push(i);
        if (i !== 0) {
          this.courseSectionAddViewModel.courseVariableScheduleList.push(new CourseVariableSchedule());
        }
        this.courseSectionAddViewModel.courseVariableScheduleList[i].day = item.name;
        this.courseSectionAddViewModel.courseVariableScheduleList[i].courseId = this.detailsFromParentModal.courseDetails.courseId;
        this.courseSectionAddViewModel.courseVariableScheduleList[i].courseId = this.detailsFromParentModal.courseDetails.courseId;
        this.courseSectionAddViewModel.courseVariableScheduleList[i].takeAttendance = false;


      });
  
  }

  onPeriodChange(periodId, indexOfDynamicRow) {
    let index = this.blockListViewModel.getBlockListForView[0]?.blockPeriod.findIndex((x) => {
      return x.periodId === +periodId
    })
    this.selectedPeriod[indexOfDynamicRow] = index;
  }

  addMoreRotatingScheduleRow() {
    this.courseSectionAddViewModel.courseVariableScheduleList.push(new CourseVariableSchedule());
    this.divCount.push(2); // Why 2? We have to fill up the divCount, It could be anything.
  }

  deleteRow(indexOfDynamicRow) {
    this.divCount.splice(indexOfDynamicRow, 1);
    this.courseSectionAddViewModel.courseVariableScheduleList.splice(indexOfDynamicRow, 1);
    this.selectedBlocks.splice(indexOfDynamicRow, 1);
    this.selectedPeriod.splice(indexOfDynamicRow, 1);
  }

  getAllBlockList() {
    this.schoolPeriodService.getAllBlockList(this.blockListViewModel).subscribe(data => {
     if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
        this.periodList = [];
        if (!data.getBlockListForView) {
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
        }
      } else {
        this.blockListViewModel = data;
        if (data.getBlockListForView.length > 0) {
          this.periodList = data.getBlockListForView[0].blockPeriod;
        }
        if (this.detailsFromParentModal.editMode) {
          for (let [i, val] of this.courseSectionAddViewModel.courseVariableScheduleList.entries()) {
            this.blockListViewModel.getBlockListForView?.map((item, j) => {
              let periodIndex = this.blockListViewModel.getBlockListForView[j].blockPeriod.findIndex((x) => {
                return x.periodId == +this.courseSectionAddViewModel.courseVariableScheduleList[i].periodId;
              });
              if (periodIndex != -1) {
                this.selectedPeriod[i] = periodIndex;
              }
            });
          }
        }
      }
    });
  }

  getAllRooms() {
    this.roomService.getAllRoom(this.roomListViewModel).subscribe(
      (res: RoomListViewModel) => {
        if (typeof (res) == 'undefined') {
          this.snackbar.open('Room list failed. ' + sessionStorage.getItem("httpError"), '', {
            duration: 10000
          });
        }
        else {
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
              if(!res.tableroomList){
                this.snackbar.open('Room List failed. ' + res._message, '', {
                  duration: 10000
                });
              }
          }
          else {
            this.roomListViewModel = res;
            this.roomListViewModel.tableroomList.map((item, index) => {
              this.roomIdWithCapacity[item.roomId] = item.capacity;
            })

          }
        }
      })
  }




  sendVariableScheduleDataToParent() {
    this.currentForm.form.markAllAsTouched()
    let invalidSeatCapacity = false;
    invalidSeatCapacity = this.courseSectionAddViewModel.courseVariableScheduleList.some((item, i) => {
      if (this.detailsFromParentModal.form.value.seats > this.roomIdWithCapacity[item.roomId]) {
        return true;
      } else {
        return false;
      }
    });
    let formValid = true;
    for (let variable of this.courseSectionAddViewModel.courseVariableScheduleList) {
      if (variable.day == null || variable.periodId == null || variable.roomId == null) {
        formValid = false;
        break;
      }
    }
    if (formValid && !invalidSeatCapacity) {
      this.checkDuplicateRow();
    } else {
      this.variableScheduleData.emit({ scheduleType: 'variableSchedule', roomList: null, scheduleDetails: this.courseSectionAddViewModel.courseVariableScheduleList, error: true });
    }
  }
  checkDuplicateRow() {
    let Ids = [];
    for (let [i, val] of this.courseSectionAddViewModel.courseVariableScheduleList.entries()) {
      Ids[i] = this.courseSectionAddViewModel.courseVariableScheduleList[i].day
        + this.courseSectionAddViewModel.courseVariableScheduleList[i].periodId.toString()
        + this.courseSectionAddViewModel.courseVariableScheduleList[i].roomId
    }
    let checkDuplicate = Ids.sort().some((item, i) => {
      if (item == Ids[i + 1]) {
        this.snackbar.open('Cannot Save Duplicate Variable Schedule ', '', {
          duration: 5000
        });
        return true;
      } else {
        return false;
      }
    })
    for (let i = 0; i < this.courseSectionAddViewModel.courseVariableScheduleList.length; i++) {
      let blockId = this.periodList[0].blockId;
      this.courseSectionAddViewModel.courseVariableScheduleList[i].blockId = blockId
    }

    if (checkDuplicate) {
      this.variableScheduleData.emit({ scheduleType: 'variableSchedule', roomList: null, scheduleDetails: this.courseSectionAddViewModel.courseVariableScheduleList, error: true });
    } else {
      this.variableScheduleData.emit({ scheduleType: 'variableSchedule', roomList: null, scheduleDetails: this.courseSectionAddViewModel.courseVariableScheduleList, error: false });
    }
  }
}
