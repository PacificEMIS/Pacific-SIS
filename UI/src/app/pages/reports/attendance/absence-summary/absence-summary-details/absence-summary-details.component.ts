import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

export interface StudentListsData {
  date: string;
  attendance: string;
  adminOfficeComment: string;
  teacherComment: string;
}

export const studentListsData: StudentListsData[] = [
  {date: 'Jan 4, 2022', attendance: 'Absent', adminOfficeComment: 'Confirmed', teacherComment: 'Out Sick'},
  {date: 'Jan 5, 2022', attendance: 'Absent', adminOfficeComment: 'Confirmed', teacherComment: 'Out Sick'},
  {date: 'Jan 6, 2022', attendance: 'Absent', adminOfficeComment: 'Confirmed', teacherComment: 'Out Sick'},
  {date: 'Jan 7, 2022', attendance: 'Absent', adminOfficeComment: 'Confirmed', teacherComment: 'Out Sick'},
  {date: 'Jan 9, 2022', attendance: 'Absent', adminOfficeComment: 'Confirmed', teacherComment: 'Out Sick'},
  {date: 'Jan 10, 2022', attendance: 'Half Day', adminOfficeComment: 'Approved', teacherComment: 'Early Pulled Out'},
];

@Component({
  selector: 'vex-absence-summary-details',
  templateUrl: './absence-summary-details.component.html',
  styleUrls: ['./absence-summary-details.component.scss']
})
export class AbsenceSummaryDetailsComponent implements OnInit {

  displayedColumns: string[] = ['date', 'attendance', 'adminOfficeComment', 'teacherComment'];
  studentLists = studentListsData;

  constructor(public translateService: TranslateService) { }
  ngOnInit(): void {
  }

}
