import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { AddCourseSectionComponent } from './add-course-section/add-course-section.component';
import icInfo from '@iconify/icons-ic/twotone-info';
import icCheckCircle from '@iconify/icons-ic/check-circle';

export interface StudentsList {
  studentSelected: boolean;
  name: string;
  studentId: number;
  alternateId: string;
  grade: string;
  phone: number;
}

const studentsList: StudentsList[] = [
  { studentSelected: true, name: 'Danielle Boucher', studentId: 1, alternateId: 'STD001', grade: 'Sophomore', phone: 3217984560 },
  { studentSelected: true, name: 'Lian Fang', studentId: 2, alternateId: 'STD002', grade: 'Sophomore', phone: 7617984560 },
  { studentSelected: true, name: 'James Miller', studentId: 3, alternateId: 'STD003', grade: 'Sophomore', phone: 7756984560 },
  { studentSelected: true, name: 'Olivia Smith', studentId: 7, alternateId: 'STD007', grade: 'Sophomore', phone: 64534984560 },
  { studentSelected: true, name: 'Amelia Jones', studentId: 19, alternateId: 'STD019', grade: 'Sophomore', phone: 4652984560 },
  { studentSelected: true, name: 'Richard Johnson', studentId: 10, alternateId: 'STD010', grade: 'Sophomore', phone: 4956984560 },
];

@Component({
  selector: 'vex-group-delete',
  templateUrl: './group-delete.component.html',
  styleUrls: ['./group-delete.component.scss']
})
export class GroupDeleteComponent implements OnInit {

  icInfo = icInfo;
  icCheckCircle = icCheckCircle;
  showScheduledStudents:boolean = true;

  displayedColumns: string[] = ['studentSelected', 'name', 'studentId', 'alternateId', 'grade', 'phone'];
  studentsList = studentsList;

  constructor(private dialog: MatDialog, public translateService:TranslateService) { }

  ngOnInit(): void {
  }

  selectCourseSection(){
    this.dialog.open(AddCourseSectionComponent, {
      width: '900px'
    });
  }

  deleteGroupStudents() {
    this.showScheduledStudents = !this.showScheduledStudents;
  }

}
