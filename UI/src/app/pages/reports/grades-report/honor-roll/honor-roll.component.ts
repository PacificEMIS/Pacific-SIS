import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

export interface StudentListsData {
  studentCheck: boolean;
  studentName: string;
  studentId: string;
  alternateId: string;
  grade: string;
  section: string;
  phone: string;
  honorRoll: string;
}

export const studentListsData: StudentListsData[] = [
  {studentCheck: true , studentName: 'Arthur Boucher', studentId: 'STD0012', alternateId: 'STD0012', grade: '10th Grade', section: 'A', phone: '7654328967', honorRoll: 'Platinum'},
  {studentCheck: true , studentName: 'Sophia Brown', studentId: 'STD0015', alternateId: 'STD0015', grade: '10th Grade', section: 'A', phone: '5654328967', honorRoll: 'Platinum'},
  {studentCheck: true , studentName: 'Wang Wang', studentId: 'STD0035', alternateId: 'STD0035', grade: '10th Grade', section: 'A', phone: '7654328967', honorRoll: 'Silver'},
  {studentCheck: true , studentName: 'Clare Garcia', studentId: 'STD0102', alternateId: 'STD0102', grade: '10th Grade', section: 'A', phone: '9854328967', honorRoll: 'Gold'},
  {studentCheck: true , studentName: 'Amelia Jones', studentId: 'STD0067', alternateId: 'STD0067', grade: '10th Grade', section: 'A', phone: '9654328967', honorRoll: 'Platinum'},
  {studentCheck: true , studentName: 'Arthur Boucher', studentId: 'STD0013', alternateId: 'STD0013', grade: '10th Grade', section: 'A', phone: '7654328967', honorRoll: 'Gold'},
  {studentCheck: true , studentName: 'Sophia Brown', studentId: 'STD0052', alternateId: 'STD0052', grade: '10th Grade', section: 'A', phone: '7654328967', honorRoll: 'Bronze'},
  {studentCheck: true , studentName: 'Wang Wang', studentId: 'STD0035', alternateId: 'STD0035', grade: '10th Grade', section: 'A', phone: '6543212367', honorRoll: 'Silver'},
  {studentCheck: true , studentName: 'Clare Garcia', studentId: 'STD0102', alternateId: 'STD0102', grade: '10th Grade', section: 'A', phone: '7654328967', honorRoll: 'Silver'},
  {studentCheck: true , studentName: 'Amelia Jones', studentId: 'STD0067', alternateId: 'STD0067', grade: '10th Grade', section: 'A', phone: '9654328967', honorRoll: 'Gold'},
];

@Component({
  selector: 'vex-honor-roll',
  templateUrl: './honor-roll.component.html',
  styleUrls: ['./honor-roll.component.scss']
})
export class HonorRollComponent implements OnInit {
  
  displayedColumns: string[] = ['studentCheck', 'studentName', 'studentId', 'alternateId', 'grade', 'section', 'phone', 'honorRoll'];
  studentLists = studentListsData;

constructor(public translateService: TranslateService) { }

  ngOnInit(): void {
  }

}
