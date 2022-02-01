import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import icClose from '@iconify/icons-ic/twotone-close';

@Component({
  selector: 'vex-course-section',
  templateUrl: './course-section.component.html',
  styleUrls: ['./course-section.component.scss']
})
export class CourseSectionComponent implements OnInit {

  icClose = icClose;
  subjectList = [];
  programList = [];
  courseList = [];
  selectedCourseList = [];
  courseSectionList = [];
  defaultSelected: boolean = true;
  isDefault: boolean = true;
  selectedCourse: number;
  selectedSubjectName = "";
  selectedProgramName = "";

  constructor(private dialogRef: MatDialogRef<CourseSectionComponent>,
    public translateService: TranslateService,
    @Inject(MAT_DIALOG_DATA) public data
  ) { }

  ngOnInit(): void {
    this.subjectList = this.data.subjectList;
    this.programList = this.data.programList;
    this.courseList = this.data.courseList;
    this.selectedCourseList = this.data.courseList;
    this.selectedCourseList.map((item, index) => {
      if (index === 0) {
        this.courseSectionList = item.course.courseSection
      }
    });
  }

  viewDetails(course, id) {
    this.isDefault = false;
    this.selectedCourse = id;
    this.courseSectionList = course.course.courseSection;
  }

  selectedCourseSection(data) {
    this.dialogRef.close(data);
  }

  selectedSubject(value) {
    this.selectedSubjectName = value;
  }

  selectedProgram(value) {
    this.selectedProgramName = value;
  }

  search() {
    if (this.selectedSubjectName === "" && this.selectedProgramName === "") {
      this.selectedCourseList = this.courseList
    } else if (this.selectedSubjectName !== "" && this.selectedProgramName === "") {
      this.selectedCourseList = this.courseList.filter((item) => {
        if (item.course.courseSubject === this.selectedSubjectName) {
          return item;
        }
      });
    } else if (this.selectedSubjectName === "" && this.selectedProgramName !== "") {
      this.selectedCourseList = this.courseList.filter((item) => {
        if (item.course.courseProgram === this.selectedProgramName) {
          return item;
        }
      });
    } else {
      this.selectedCourseList = this.courseList.filter((item) => {
        if (item.course.courseSubject === this.selectedSubjectName && item.course.courseProgram === this.selectedProgramName) {
          return item;
        }
      });
    }
    this.isDefault = false;
    this.selectedCourse = null;
    this.courseSectionList = [];
  }

}