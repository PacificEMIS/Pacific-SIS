import { Component, OnInit } from '@angular/core';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { TranslateService } from '@ngx-translate/core';

interface Attendance {
  date: string;
  grade: string;
  students: number;
  daysPossible: number;
  present: number;
  absent: number;
  others: number;
  notTaken: number;
  ada: string;
  avgAttendance: number;
  avgAbsent: number;
}

@Component({
  selector: 'vex-average-attendance-by-day',
  templateUrl: './average-attendance-by-day.component.html',
  styleUrls: ['./average-attendance-by-day.component.scss']
})
export class AverageAttendanceByDayComponent implements OnInit {

  displayedColumns: string[] = ['date', 'grade', 'students', 'daysPossible', 'present', 'absent', 'others', 'notTaken', 'ada', 'avgAttendance', 'avgAbsent'];
  attendance: Attendance[] = [
    {date: 'Jan 01, 2021', grade: '9th Grade', students: 300, daysPossible: 99, present: 170, absent: 4, others: 0, notTaken: 12, ada: '87.63%', avgAttendance: 0.88, avgAbsent: 0.02},
    {date: 'Jan 01, 2021', grade: '10th Grade', students: 280, daysPossible: 99, present: 593, absent: 16, others: 0, notTaken: 6, ada: '87.33%', avgAttendance: 0.87, avgAbsent: 0.02},
    {date: 'Jan 01, 2021', grade: '11th Grade', students: 378, daysPossible: 99, present: 342, absent: 6, others: 0, notTaken: 3, ada: '88.14%', avgAttendance: 0.88, avgAbsent: 0.02},
    {date: 'Jan 01, 2021', grade: '12th Grade', students: 317, daysPossible: 99, present: 167, absent: 7, others: 0, notTaken: 2, ada: '89.08%', avgAttendance: 0.86, avgAbsent: 0.04},
    {date: 'Jan 01, 2021', grade: '9th Grade', students: 300, daysPossible: 99, present: 170, absent: 4, others: 0, notTaken: 12, ada: '87.63%', avgAttendance: 0.88, avgAbsent: 0.02},
    {date: 'Jan 01, 2021', grade: '10th Grade', students: 280, daysPossible: 99, present: 593, absent: 16, others: 0, notTaken: 6, ada: '87.33%', avgAttendance: 0.87, avgAbsent: 0.02},
    {date: 'Jan 01, 2021', grade: '11th Grade', students: 378, daysPossible: 99, present: 342, absent: 6, others: 0, notTaken: 3, ada: '88.14%', avgAttendance: 0.88, avgAbsent: 0.02},
    {date: 'Jan 01, 2021', grade: '12th Grade', students: 317, daysPossible: 99, present: 167, absent: 7, others: 0, notTaken: 2, ada: '89.08%', avgAttendance: 0.86, avgAbsent: 0.04},
  ];

  constructor(public translateService: TranslateService,
    private paginatorObj: MatPaginatorIntl,
    ) { 
      paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
    // translateService.use("en");
  }

  ngOnInit(): void {
  }

}
