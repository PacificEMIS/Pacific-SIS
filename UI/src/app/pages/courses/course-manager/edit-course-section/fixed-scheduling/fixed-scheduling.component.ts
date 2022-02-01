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

import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { RoomListViewModel } from '../../../../../models/room.model';
import { BlockListViewModel } from '../../../../../models/school-period.model';
import { RoomService } from '../../../../../services/room.service';
import { SchoolPeriodService } from '../../../../../services/school-period.service';
import { FixedSchedulingCourseSectionAddModel, OutputEmitDataFormat } from '../../../../../models/course-section.model';
import { CourseSectionService } from '../../../../../services/course-section.service';
import { weeks } from '../../../../../common/static-data';
import { DefaultValuesService } from '../../../../../common/default-values.service';
import { CommonService } from 'src/app/services/common.service';

@Component({
  selector: 'vex-fixed-scheduling',
  templateUrl: './fixed-scheduling.component.html',
  styleUrls: ['./fixed-scheduling.component.scss']
})
export class FixedSchedulingComponent implements OnInit,OnChanges {
  @ViewChild('f') currentForm: NgForm;
  roomListViewModel:RoomListViewModel=new RoomListViewModel();
  fixedSchedulingModel: FixedSchedulingCourseSectionAddModel = new FixedSchedulingCourseSectionAddModel();
  @Input() calendar;
  @Input() detailsFromParentModal;
  @Output() fixedScheduleData = new EventEmitter<OutputEmitDataFormat>()
  activeDays=[];
  periodList=[];
  weekArray: number[] = [];
  activeDaysError=false;
  weeks = weeks;
  constructor(
    private roomService:RoomService,
    private schoolPeriodService:SchoolPeriodService,
    private snackbar: MatSnackBar,
    public defaultValueService : DefaultValuesService,
    private courseSectionService:CourseSectionService,
    private commonService: CommonService,
    ) {
      this.courseSectionService.currentUpdate.subscribe((res)=>{
        if(res){
          this.currentForm.form.markAllAsTouched();
          this.sendFixedScheduleDataToParent();
        }
      })
     }

  ngOnChanges(OnChanges): void {
    if(OnChanges?.calendar?.previousValue?.title){
      this.activeDays=[];
    }
    if(this.calendar==undefined || this.calendar?.days==undefined){
      this.weekArray =[];
    }
    else{
      this.weekArray = this.calendar.days.split('').map(x => +x);
      let namesOfDayById=[];
      this.weekArray?.map((item)=>{
        this.weeks.map((week)=>{
          if(item==week.id){
            namesOfDayById.push(week.name);
          }
        })
      });
      this.weekArray=namesOfDayById;
    }
  }

  ngOnInit(): void {
    if(this.detailsFromParentModal.editMode){
      this.patchFormValue();
    }
    this.getAllRooms();
    this.getAllBlockList();
  }

  patchFormValue(){
    this.fixedSchedulingModel.courseFixedSchedule.roomId=this.detailsFromParentModal.courseSectionDetails.courseFixedSchedule.roomId;
    this.fixedSchedulingModel.courseFixedSchedule.periodId=this.detailsFromParentModal.courseSectionDetails.courseFixedSchedule.periodId;
    this.fixedSchedulingModel.courseFixedSchedule.attendanceTaken=this.detailsFromParentModal.courseSectionDetails.courseSection.attendanceTaken;
    this.fixedSchedulingModel.courseFixedSchedule.courseSectionId=this.detailsFromParentModal.courseSectionDetails.courseSection.courseSectionId;
    this.fixedSchedulingModel.courseFixedSchedule.serial=this.detailsFromParentModal.courseSectionDetails.courseFixedSchedule.serial;
    this.fixedSchedulingModel.courseFixedSchedule.gradeScaleId=this.detailsFromParentModal.courseSectionDetails.courseSection.gradeScaleId;
    this.fixedSchedulingModel.courseFixedSchedule.tenantId= this.defaultValueService.getTenantID();
    this.fixedSchedulingModel.courseFixedSchedule.updatedBy=this.defaultValueService.getUserGuidId();
    this.activeDays = this.detailsFromParentModal.courseSectionDetails.courseSection.meetingDays.split('|');
      }
 
  getAllRooms(){
    this.roomService.getAllRoom(this.roomListViewModel).subscribe(
      (res:RoomListViewModel)=>{
        if(typeof(res)=='undefined'){
          this.snackbar.open('' + this.defaultValueService.getHttpError(), '', {
            duration: 10000
          });
        }
        else{
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);    
            if(!res.tableroomList){
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
            }
          } 
          else{
            this.roomListViewModel=res;
          }
        }
      }
    )
  }

  getAllBlockList() {
    let blockListViewModel:BlockListViewModel= new BlockListViewModel();
    blockListViewModel.tenantId = this.defaultValueService.getTenantID();
    blockListViewModel.schoolId = this.defaultValueService.getSchoolID();
    this.schoolPeriodService.getAllBlockList(blockListViewModel).subscribe(
      (res: BlockListViewModel) => {
        if(typeof(res)=='undefined'){
          this.snackbar.open('' + this.defaultValueService.getHttpError(), '', {
            duration: 10000
          });
        }
        else{
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);    
            this.snackbar.open(res._message, '', {
              duration: 10000
            }); 
          } 
          else{
            this.periodList=res.getBlockListForView[0]?.blockPeriod
            this.fixedSchedulingModel.courseFixedSchedule.blockId=res.getBlockListForView[0]?.blockId;
          }
        }
        
      }
    );
  }

  
  selectDays(event:Event, name) {

    event.preventDefault();
    if(this.weekArray.includes(name)){
      let index = this.activeDays.indexOf(name);
      if(index>-1){
        this.activeDays.splice(index,1);
      }
      else{
        this.activeDays.push(name);
      }
    }
  }

  sendFixedScheduleDataToParent(){
    if(this.currentForm.form.valid && this.activeDays.length>0){
      this.fixedSchedulingModel.courseFixedSchedule.meetingDays=this.activeDays?.join('|');
      this.fixedScheduleData.emit({scheduleType:'fixedschedule',roomList: null,scheduleDetails:this.fixedSchedulingModel.courseFixedSchedule,error:false});
    }else{
    this.activeDaysError=true;
    this.fixedScheduleData.emit({scheduleType:'fixedschedule', roomList: null, scheduleDetails:null,error:true});
    }

  }

}

