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

import { Component, EventEmitter, Inject, OnInit, Output, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import icClose from '@iconify/icons-ic/twotone-close';
import { fadeInUp400ms } from '../../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../../@vex/animations/stagger.animation';
import { GetAllMembersList } from 'src/app/models/membership.model';
import { CalendarAddViewModel, Weeks } from 'src/app/models/calendar.model';
import { MembershipService } from '../../../../services/membership.service';
import { CalendarService } from '../../../../services/calendar.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import * as moment from 'moment';
import { SharedFunction } from '../../../shared/shared-function';
import { MatCheckbox, MatCheckboxChange } from '@angular/material/checkbox';
import { formatDate } from '@angular/common';
import { CommonService } from 'src/app/services/common.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';

@Component({
  selector: 'vex-add-calendar',
  templateUrl: './add-calendar.component.html',
  styleUrls: ['./add-calendar.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms
  ]
})
export class AddCalendarComponent implements OnInit {
  @ViewChild('checkBox') checkBox: MatCheckbox;
  checkAll: boolean;
  calendarTitle: string;
  minStartDate:string;
  maxStartDate: Date;
  calendarActionButtonTitle = "submit";
  getAllMembersList: GetAllMembersList = new GetAllMembersList();
  calendarAddViewModel = new CalendarAddViewModel();
  weekArray: number[] = [];
  membercount: number;
  memberArray: number[] = [];
  @Output() afterClosed = new EventEmitter<boolean>();
  form: FormGroup;
  icClose = icClose;
  weeks: Weeks[] = [
    { name: 'sunday', id: 0 },
    { name: 'monday', id: 1 },
    { name: 'tuesday', id: 2 },
    { name: 'wednesday', id: 3 },
    { name: 'thursday', id: 4 },
    { name: 'friday', id: 5 },
    { name: 'saturday', id: 6 }
  ];
  constructor(
    private dialogRef: MatDialogRef<AddCalendarComponent>,
    private fb: FormBuilder,
    private membershipService: MembershipService,
    private commonFunction: SharedFunction,
    private calendarService: CalendarService,
    @Inject(MAT_DIALOG_DATA) public data: any,
    private snackbar: MatSnackBar,
    private commonService: CommonService,
    public defaultValuesService: DefaultValuesService
  ) {
      this.calendarAddViewModel.schoolCalendar.startDate = this.defaultValuesService.getFullYearStartDate();
      this.minStartDate = this.defaultValuesService.getFullYearStartDate();
      this.maxStartDate = moment(this.defaultValuesService.getFullYearEndDate()).subtract(1, 'days').toDate();
  }

  ngOnInit(): void {
    this.form = this.fb.group({
      title: ['', Validators.required],
      startDate: ['', Validators.required],
      endDate: [''],
      isDefaultCalendar: [false]
    });
    if(this.data.calendarListCount==0){
      this.calendarAddViewModel.schoolCalendar.startDate=this.defaultValuesService.getFullYearStartDate();
    }

    if (this.data == null) {
      this.snackbar.open('Null vallue occur. ', '', {
        duration: 1000
      });

    }
    else {
      this.membercount = this.data.membercount
      this.getAllMembersList = this.data.allMembers
      if (this.data.calendar == null) {
        this.calendarTitle = "addCalendar";
      }
      else {
        this.calendarTitle = "editCalendar";
        this.calendarActionButtonTitle = "update";
        this.calendarAddViewModel.schoolCalendar = this.data.calendar;
        this.form.patchValue({ isDefaultCalendar: this.calendarAddViewModel.schoolCalendar.defaultCalender });
        this.weekArray = this.calendarAddViewModel.schoolCalendar.days.split('').map(x => +x);
        if (this.calendarAddViewModel.schoolCalendar.visibleToMembershipId != null && this.calendarAddViewModel.schoolCalendar.visibleToMembershipId != '') {
          let membershipIds: string[] = this.calendarAddViewModel.schoolCalendar.visibleToMembershipId.split(',');
          this.memberArray = membershipIds.map(Number);
        }
        if (this.memberArray.length === this.getAllMembersList.getAllMemberList.length) {
          this.checkAll = true
        }

      }

    }

  }

  getMinEndDateVal() {
    return moment(this.form.value.startDate).add(1, 'days').toDate();
  }

  checkDate(){
    let markingPeriodDate=new Date(this.defaultValuesService.getSchoolOpened()).getTime();
    let startDate=new Date(this.calendarAddViewModel.schoolCalendar.startDate).getTime(); 
    if((startDate!=markingPeriodDate) || (this.data.calendarListCount==0 && startDate!=markingPeriodDate)){
      this.form.controls.startDate.setErrors({'nomatch': true});
    }else{
      if(this.form.controls.startDate.errors?.nomatch){
        this.form.controls.startDate.setErrors(null);
      }
    }
  }

  showOptions(event:MatCheckboxChange){
  if(event.checked){
    this.calendarAddViewModel.schoolCalendar.startDate=this.defaultValuesService.getFullYearStartDate();
    this.checkDate();
  }
  }

  submitCalendar() {
    if (this.form.invalid) {
      return
    }
    this.calendarAddViewModel.schoolCalendar.title = this.form.value.title;
    this.calendarAddViewModel.schoolCalendar.defaultCalender = this.form.value.isDefaultCalendar;
    // this.calendarAddViewModel.schoolCalendar.academicYear = new Date(this.minStartDate).getFullYear();
    this.calendarAddViewModel.schoolCalendar.days = this.weekArray.toString().replace(/,/g, "");
    this.calendarAddViewModel.schoolCalendar.visibleToMembershipId = this.memberArray.toString();
    this.calendarAddViewModel.schoolCalendar.startDate = this.commonFunction.formatDateSaveWithoutTime(this.form.value.startDate);
    this.calendarAddViewModel.schoolCalendar.endDate = this.commonFunction.formatDateSaveWithoutTime(this.form.value.endDate);
    if (this.form.valid && this.weekArray.length > 0) {
      if (this.calendarAddViewModel.schoolCalendar.calenderId > 0) {
        delete this.calendarAddViewModel.schoolCalendar.academicYear;
        this.calendarService.updateCalendar(this.calendarAddViewModel).subscribe(data => {
          if (data._failure) {
            this.commonService.checkTokenValidOrNot(data._message);
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          } else {
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
            this.dialogRef.close('submited');
          }

        });
      }
      else {
        this.calendarService.addCalendar(this.calendarAddViewModel).subscribe(data => {
          if (data._failure) {
            this.commonService.checkTokenValidOrNot(data._message);
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
          } else {
            this.snackbar.open(data._message, '', {
              duration: 10000
            });
            this.dialogRef.close('submited');
          }
        });
      }
    }

  }
  selectDays(event, id) {
    event.preventDefault();
    let index = this.weekArray.indexOf(id);
    if (index > -1) {
      this.weekArray.splice(index, 1);
    }
    else {
      this.weekArray.push(id);
    }

  }

  updateCheck(event) {
    if (this.memberArray.length === this.getAllMembersList.getAllMemberList.length) {
      for (let i = 0; i < this.getAllMembersList.getAllMemberList.length; i++) {
        let index = this.memberArray.indexOf(this.getAllMembersList.getAllMemberList[i].membershipId);
        if (index > -1) {
          this.memberArray.splice(index, 1);
        }
        else {
          this.memberArray.push(this.getAllMembersList.getAllMemberList[i].membershipId);
        }
      }
    }
    else if (this.memberArray.length === 0) {
      for (let i = 0; i < this.getAllMembersList.getAllMemberList.length; i++) {
        let index = this.memberArray.indexOf(this.getAllMembersList.getAllMemberList[i].membershipId);
        if (index > -1) {
          this.memberArray.splice(index, 1);
        }
        else {
          this.memberArray.push(this.getAllMembersList.getAllMemberList[i].membershipId);
        }
      }
    }
    else {
      for (let i = 0; i < this.getAllMembersList.getAllMemberList.length; i++) {
        let index = this.memberArray.indexOf(this.getAllMembersList.getAllMemberList[i].membershipId);
        if (index > -1) {
          continue;
        }
        else {
          this.memberArray.push(this.getAllMembersList.getAllMemberList[i].membershipId);
        }
      }
    }
  }
  selectChildren(event, id) {
    event.preventDefault();
    let index = this.memberArray.indexOf(id);
    if (index > -1) {
      this.memberArray.splice(index, 1);
    }
    else {
      this.memberArray.push(id);
    }
    if (this.memberArray.length == this.getAllMembersList.getAllMemberList.length) {
      this.checkAll = true;
      this.checkBox.checked = true;
    } else {
      this.checkAll = false;
      this.checkBox.checked = false;
    }
  }


}
