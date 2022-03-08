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
  section: string;
  phone: string;
}

export const studentListsData: StudentListsData[] = [
  {studentCheck: true , studentName: 'Arthur Boucher', studentId: 'STD0012', alternateId: 'STD0012', grade: 'Grade 11', section: 'Section A', phone: '7654328967'},
  {studentCheck: true , studentName: 'Sophia Brown', studentId: 'STD0015', alternateId: 'STD0015', grade: 'Grade 10', section: 'Section B', phone: '5654328967'},
  {studentCheck: true , studentName: 'Wang Wang', studentId: 'STD0035', alternateId: 'STD0035', grade: 'Grade 11', section: 'Section A', phone: '7654328967'},
  {studentCheck: true , studentName: 'Clare Garcia', studentId: 'STD0102', alternateId: 'STD0102', grade: 'Grade 11', section: 'Section A', phone: '9854328967'},
  {studentCheck: true , studentName: 'Amelia Jones', studentId: 'STD0067', alternateId: 'STD0067', grade: 'Grade 11', section: 'Section B', phone: '9654328967'},
  {studentCheck: true , studentName: 'Arthur Boucher', studentId: 'STD0013', alternateId: 'STD0013', grade: 'Grade 9', section: 'Section A', phone: '7654328967'},
  {studentCheck: true , studentName: 'Sophia Brown', studentId: 'STD0052', alternateId: 'STD0052', grade: 'Grade 9', section: 'Section B', phone: '7654328967'},
  {studentCheck: true , studentName: 'Wang Wang', studentId: 'STD0035', alternateId: 'STD0035', grade: 'Grade 10', section: 'Section A', phone: '6543212367'},
  {studentCheck: true , studentName: 'Clare Garcia', studentId: 'STD0102', alternateId: 'STD0102', grade: 'Grade 11', section: 'Section A', phone: '7654328967'},
  {studentCheck: true , studentName: 'Amelia Jones', studentId: 'STD0067', alternateId: 'STD0067', grade: 'Grade 9', section: 'Section A', phone: '9654328967'},
];

@Component({
  selector: 'vex-progress-report',
  templateUrl: './progress-report.component.html',
  styleUrls: ['./progress-report.component.scss']
})
export class ProgressReportComponent implements OnInit {

  displayedColumns: string[] = ['studentCheck', 'studentName', 'studentId', 'alternateId', 'grade', 'section', 'phone'];
  studentLists = studentListsData;

  constructor(public translateService: TranslateService,
    private paginatorObj: MatPaginatorIntl,
    private defaultValuesService:DefaultValuesService
    ) { 
    paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
    this.defaultValuesService.setReportCompoentTitle.next("Progress Reports");
    }

  ngOnInit(): void {
  }

}
