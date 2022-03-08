import { Component, OnInit } from '@angular/core';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { TranslateService } from '@ngx-translate/core';
import { DefaultValuesService } from 'src/app/common/default-values.service';

export interface StudentListData {
  studentName: string;
  studentId: string;
  alternateId: string;
  grade: string;
  section: string;
  phone: number;
  gpa: string;
  unweightedGpa: string;
  weightedGpa: string;
  classRank: number;
}

export const studentListData: StudentListData[] = [
  {studentName: 'Arthur Boucher', studentId: 'STD0012', alternateId: 'STD0012', grade: '10th Grade', section: 'A', phone: 5643267895, gpa: '3.73', unweightedGpa: '3.73', weightedGpa: '', classRank: 1},
  {studentName: 'Sophia Brown', studentId: 'STD0015', alternateId: 'STD0015', grade: '10th Grade', section: 'A', phone: 5643267895, gpa: '3.67', unweightedGpa: '3.67', weightedGpa: '', classRank: 2},
  {studentName: 'Wang Wang', studentId: 'STD0035', alternateId: 'STD0035', grade: '10th Grade', section: 'B', phone: 5643267895, gpa: '3.49', unweightedGpa: '3.49', weightedGpa: '', classRank: 3},
  {studentName: 'Clare Garcia', studentId: 'STD0102', alternateId: 'STD0102', grade: '10th Grade', section: 'B', phone: 5643267895, gpa: '3.47', unweightedGpa: '3.47', weightedGpa: '', classRank: 4},
  {studentName: 'Amelia Jones', studentId: 'STD0067', alternateId: 'STD0067', grade: '10th Grade', section: 'C', phone: 5643267895, gpa: '3.12', unweightedGpa: '3.12', weightedGpa: '', classRank: 5},
  {studentName: 'Arthur Boucher', studentId: 'STD0013', alternateId: 'STD0013', grade: '10th Grade', section: 'A', phone: 5643267895, gpa: '2.83', unweightedGpa: '2.83', weightedGpa: '', classRank: 6},
  {studentName: 'Wang Wang', studentId: 'STD0035', alternateId: 'STD0035', grade: '10th Grade', section: 'B', phone: 5643267895, gpa: '2.86', unweightedGpa: '2.86', weightedGpa: '', classRank: 7},
  {studentName: 'Clare Garcia', studentId: 'STD0002', alternateId: 'STD0002', grade: '10th Grade', section: 'C', phone: 5643267895, gpa: '1.96', unweightedGpa: '1.96', weightedGpa: '', classRank: 8},
  {studentName: 'Amelia Jones', studentId: 'STD0076', alternateId: 'STD0076', grade: '10th Grade', section: 'A', phone: 5643267895, gpa: '1.00', unweightedGpa: '1.00', weightedGpa: '', classRank: 9},
];

@Component({
  selector: 'vex-class-rank-list',
  templateUrl: './class-rank-list.component.html',
  styleUrls: ['./class-rank-list.component.scss']
})
export class ClassRankListComponent implements OnInit {

  displayedColumns: string[] = ['studentName', 'studentId', 'alternateId', 'grade', 'section', 'phone', 'gpa', 'unweightedGpa', 'weightedGpa', 'classRank'];
  studentList = studentListData;

  constructor(
    public translateService: TranslateService,
    private paginatorObj: MatPaginatorIntl,
    private defaultValuesService:DefaultValuesService
    ) { 
      this.defaultValuesService.setReportCompoentTitle.next("GPA / Class Rank List");
    // translateService.use("en");
  }


  ngOnInit(): void {
  }

}
