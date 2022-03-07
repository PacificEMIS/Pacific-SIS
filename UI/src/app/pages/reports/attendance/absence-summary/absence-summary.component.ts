import { Component, OnInit } from '@angular/core';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';

export interface StudentListsData {
  studentName: string;
  studentId: string;
  alternateId: string;
  grade: string;
  phone: string;
  absent: number;
  halfDay: number;
}

export const studentListsData: StudentListsData[] = [
  {studentName: 'Arthur Boucher', studentId: 'STD0012', alternateId: 'STD0012', grade: '10th Grade', phone: '7654328967', absent: 1, halfDay: 0},
  {studentName: 'Sophia Brown', studentId: 'STD0015', alternateId: 'STD0015', grade: '10th Grade', phone: '5654328967', absent: 4, halfDay: 1},
  {studentName: 'Wang Wang', studentId: 'STD0035', alternateId: 'STD0035', grade: '10th Grade', phone: '7654328967', absent: 3, halfDay: 0},
  {studentName: 'Clare Garcia', studentId: 'STD0102', alternateId: 'STD0102', grade: '10th Grade', phone: '9854328967', absent: 1, halfDay: 1},
  {studentName: 'Amelia Jones', studentId: 'STD0067', alternateId: 'STD0067', grade: '10th Grade', phone: '9654328967', absent: 1, halfDay: 0},
  {studentName: 'Arthur Boucher', studentId: 'STD0013', alternateId: 'STD0013', grade: '10th Grade', phone: '7654328967', absent: 2, halfDay: 0},
  {studentName: 'Sophia Brown', studentId: 'STD0052', alternateId: 'STD0052', grade: '10th Grade', phone: '7654328967', absent: 7, halfDay: 0},
  {studentName: 'Wang Wang', studentId: 'STD0035', alternateId: 'STD0035', grade: '10th Grade', phone: '6543212367', absent: 4, halfDay: 0},
  {studentName: 'Clare Garcia', studentId: 'STD0102', alternateId: 'STD0102', grade: '10th Grade', phone: '7654328967', absent: 2, halfDay: 0},
  {studentName: 'Amelia Jones', studentId: 'STD0067', alternateId: 'STD0067', grade: '10th Grade', phone: '9654328967', absent: 3, halfDay: 0},
];

@Component({
  selector: 'vex-absence-summary',
  templateUrl: './absence-summary.component.html',
  styleUrls: ['./absence-summary.component.scss']
})
export class AbsenceSummaryComponent implements OnInit {

  displayedColumns: string[] = ['studentName', 'studentId', 'alternateId', 'grade', 'phone', 'absent', 'halfDay'];
  studentLists = studentListsData;

  constructor(
    public translateService: TranslateService, 
    private router: Router,
    private paginatorObj: MatPaginatorIntl,
    ) { 
      paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
    }
  ngOnInit(): void {
  }

  viewAttendanceSummaryDetails() {
    // this.router.navigate(['/school', 'attendance', 'missing-attendance', 'missing-attendance-details']);
    this.router.navigate(['/school', 'reports', 'attendance', 'absence-summary', 'absence-summary-details']);
  }

}
