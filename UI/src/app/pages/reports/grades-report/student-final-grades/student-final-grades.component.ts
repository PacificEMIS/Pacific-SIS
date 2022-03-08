import { Component, OnInit } from '@angular/core';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { TranslateService } from '@ngx-translate/core';
import { DefaultValuesService } from 'src/app/common/default-values.service';

export interface StudentListsData {
  studentCheck: boolean;
  studentName: string;
  studentId: string;
  alternateId: string;
  grade: string;
  phone: string;
}

export const studentListsData: StudentListsData[] = [
  {studentCheck: true , studentName: 'Arthur Boucher', studentId: 'STD0012', alternateId: 'STD0012', grade: '10th Grade', phone: '7654328967'},
  {studentCheck: true , studentName: 'Sophia Brown', studentId: 'STD0015', alternateId: 'STD0015', grade: '10th Grade', phone: '5654328967'},
  {studentCheck: false , studentName: 'Wang Wang', studentId: 'STD0035', alternateId: 'STD0035', grade: '10th Grade', phone: '7654328967'},
  {studentCheck: false , studentName: 'Clare Garcia', studentId: 'STD0102', alternateId: 'STD0102', grade: '10th Grade', phone: '9854328967'},
  {studentCheck: false , studentName: 'Amelia Jones', studentId: 'STD0067', alternateId: 'STD0067', grade: '10th Grade', phone: '9654328967'},
  {studentCheck: false , studentName: 'Arthur Boucher', studentId: 'STD0013', alternateId: 'STD0013', grade: '10th Grade', phone: '7654328967'},
  {studentCheck: false , studentName: 'Sophia Brown', studentId: 'STD0052', alternateId: 'STD0052', grade: '10th Grade', phone: '7654328967'},
  {studentCheck: false , studentName: 'Wang Wang', studentId: 'STD0035', alternateId: 'STD0035', grade: '10th Grade', phone: '6543212367'},
  {studentCheck: false , studentName: 'Clare Garcia', studentId: 'STD0102', alternateId: 'STD0102', grade: '10th Grade', phone: '7654328967'},
  {studentCheck: false , studentName: 'Amelia Jones', studentId: 'STD0067', alternateId: 'STD0067', grade: '10th Grade', phone: '9654328967'},
];



@Component({
  selector: 'vex-student-final-grades',
  templateUrl: './student-final-grades.component.html',
  styleUrls: ['./student-final-grades.component.scss']
})
export class StudentFinalGradesComponent implements OnInit {

  currentTab: string = 'selectStudents';

  displayedColumns: string[] = ['studentCheck', 'studentName', 'studentId', 'alternateId', 'grade', 'phone'];
  studentLists = studentListsData;


  constructor(
    public translateService: TranslateService,
    private paginatorObj: MatPaginatorIntl,
    private defaultValuesService: DefaultValuesService
    ) { 
      paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
      this.defaultValuesService.setReportCompoentTitle.next("Student Final Grades");
    // translateService.use("en");
  }

  ngOnInit(): void {
  }

  changeTab(status) {
    this.currentTab = status;
  }

}
