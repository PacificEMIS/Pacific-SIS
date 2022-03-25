import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import icClose from '@iconify/icons-ic/close';

export interface CourseDetails {
    course: string;
    courseSection: string;
    markingPeriod: string;
    startDate: string;
    endDate: string
    totalSeats: number;
    available: number;
}

const courseDetails: CourseDetails[] = [
  { courseSection: 'MATH001', course: 'Mathematics', markingPeriod: 'Full Year', startDate: 'Jan 01, 2022', endDate: 'Dec 20, 2022', totalSeats: 115, available: 63 },
  { courseSection: 'GEOM02', course: 'Mathematics', markingPeriod: 'Quarter 2', startDate: 'Apr 22, 2022', endDate: 'Jun 30, 2022', totalSeats: 105, available: 10 },
  { courseSection: 'ALGB001', course: 'Mathematics', markingPeriod: 'Full Year', startDate: 'Jan 01, 2022', endDate: 'Dec 20, 2022', totalSeats: 105, available: 7 },
  { courseSection: 'MATH002', course: 'Mathematics', markingPeriod: 'Full Year', startDate: 'Jan 01, 2022', endDate: 'Dec 20, 2022', totalSeats: 110, available: 63 },
  { courseSection: 'MATH003', course: 'Mathematics', markingPeriod: 'Full Year', startDate: 'Jan 01, 2022', endDate: 'Dec 20, 2022', totalSeats: 120, available: 63 }
];

@Component({
  selector: 'vex-add-course-section',
  templateUrl: './add-course-section.component.html',
  styleUrls: ['./add-course-section.component.scss']
})
export class AddCourseSectionComponent implements OnInit {

  icClose = icClose;

  displayedColumns: string[] = ['courseSection', 'course', 'markingPeriod', 'startDate', 'endDate', 'totalSeats', 'available'];
  courseDetails = courseDetails;

  constructor(public translateService:TranslateService) { }

  ngOnInit(): void {
  }

}
