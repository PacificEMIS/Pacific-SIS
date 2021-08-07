import { Component, OnInit } from '@angular/core';
import icSchool from '@iconify/icons-ic/twotone-account-balance';
import icStudents from '@iconify/icons-ic/twotone-school';
import icUsers from '@iconify/icons-ic/twotone-people';
import icSchedule from '@iconify/icons-ic/twotone-date-range';
import icGrade from '@iconify/icons-ic/twotone-leaderboard';
import icAttendance from '@iconify/icons-ic/twotone-access-alarm';
import icEventAvailable from '@iconify/icons-ic/twotone-event-available';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'vex-reports',
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.scss']
})
export class ReportsComponent implements OnInit {

  icSchool = icSchool;
  icStudents = icStudents;
  icUsers = icUsers;
  icSchedule = icSchedule;
  icGrade = icGrade;
  icAttendance = icAttendance;
  icEventAvailable = icEventAvailable;

  constructor(public translateService: TranslateService) { 
    translateService.use('en');
  }

  ngOnInit(): void {
  }

}
