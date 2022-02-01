import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

export interface StudentListData {
  date: string;
  studentName: string;
  studentId: string;
  grade: string;
  periodAttendanceStatus: string;
}

export const studentListData: StudentListData[] = [
  {date: 'Jan 01, 2021' , studentName: 'Arthur Boucher', studentId: 'STD0012', grade: '10th Grade', periodAttendanceStatus: ''},
  {date: 'Jan 01, 2021' , studentName: 'Sophia Brown', studentId: 'STD0015', grade: '10th Grade', periodAttendanceStatus: ''},
  {date: 'Jan 01, 2021' , studentName: 'Wang Wang', studentId: 'STD0035', grade: '10th Grade', periodAttendanceStatus: ''},
  {date: 'Jan 01, 2021' , studentName: 'Clare Garcia', studentId: 'STD0102', grade: '10th Grade', periodAttendanceStatus: ''},
  {date: 'Jan 01, 2021' , studentName: 'Amelia Jones', studentId: 'STD0067', grade: '10th Grade', periodAttendanceStatus: ''},
  {date: 'Jan 01, 2021' , studentName: 'Arthur Boucher', studentId: 'STD0013', grade: '10th Grade', periodAttendanceStatus: ''},
  {date: 'Jan 01, 2021' , studentName: 'Sophia Brown', studentId: 'STD0052', grade: '10th Grade', periodAttendanceStatus: ''},
  {date: 'Jan 01, 2021' , studentName: 'Wang Wang', studentId: 'STD0035', grade: '10th Grade', periodAttendanceStatus: ''},
  {date: 'Jan 01, 2021' , studentName: 'Clare Garcia', studentId: 'STD0102', grade: '10th Grade', periodAttendanceStatus: ''},
  {date: 'Jan 01, 2021' , studentName: 'Amelia Jones', studentId: 'STD0067', grade: '10th Grade', periodAttendanceStatus: ''},
];

@Component({
  selector: 'vex-attendance-report',
  templateUrl: './attendance-report.component.html',
  styleUrls: ['./attendance-report.component.scss']
})
export class AttendanceReportComponent implements OnInit {
  displayedColumns: string[] = ['date','studentName', 'studentId', 'grade', 'periodAttendanceStatus'];
  studentList = studentListData;

  constructor(public translateService: TranslateService) { 
    // translateService.use("en");
  }

  ngOnInit(): void {
  }

}
