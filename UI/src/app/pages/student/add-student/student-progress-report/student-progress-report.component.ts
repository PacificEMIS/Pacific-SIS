import { Component, OnInit } from '@angular/core';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { StudentService } from 'src/app/services/student.service';

@Component({
  selector: 'vex-student-progress-report',
  templateUrl: './student-progress-report.component.html',
  styleUrls: ['./student-progress-report.component.scss']
})
export class StudentProgressReportComponent implements OnInit {

  constructor(
    private defaultValuesService: DefaultValuesService,
    private studentService: StudentService,
  ) { 
    this.defaultValuesService.checkAcademicYear() && !this.studentService.getStudentId() ? this.studentService.redirectToGeneralInfo() : !this.defaultValuesService.checkAcademicYear() && !this.studentService.getStudentId() ? this.studentService.redirectToStudentList() : '';
  }

  ngOnInit(): void {
  }

}
