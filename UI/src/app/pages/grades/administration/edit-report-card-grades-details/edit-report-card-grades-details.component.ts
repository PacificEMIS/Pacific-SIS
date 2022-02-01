import { Component, EventEmitter, Input, OnChanges, OnInit, Output, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import icEdit from '@iconify/icons-ic/twotone-edit';
import icRestore from '@iconify/icons-ic/twotone-restore';
import { GradeScaleListView } from 'src/app/models/grades.model';
import { ResponseStudentReportCardGradesModel } from 'src/app/models/report-card.model';
import { DataEditInfoComponent } from 'src/app/pages/shared-module/data-edit-info/data-edit-info.component';
import { CommonService } from 'src/app/services/common.service';
import { GradesService } from 'src/app/services/grades.service';

@Component({
  selector: 'vex-edit-report-card-grades-details',
  templateUrl: './edit-report-card-grades-details.component.html',
  styleUrls: ['./edit-report-card-grades-details.component.scss']
})
export class EditReportCardGradesDetailsComponent implements OnInit, OnChanges {

  icEdit = icEdit;
  icRestore = icRestore;
  inactive:boolean = true;
  showUpdate = false;
  @Input() studentReportCardGrades;
  @Input() gradeScaleList;
  @Output() updateData = new EventEmitter<any>();
  @ViewChild('f') currentForm: NgForm;
  gradeScaleStandardList = [];
  // gradeScaleList = [];
  cloneResponseStudentReportCardGradesModel;
  gradeScaleListView: GradeScaleListView = new GradeScaleListView();
  responseStudentReportCardGradesModel: ResponseStudentReportCardGradesModel = new ResponseStudentReportCardGradesModel();

  constructor(
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
  }
  
  ngOnChanges() {
    this.responseStudentReportCardGradesModel = new ResponseStudentReportCardGradesModel();
    this.responseStudentReportCardGradesModel = this.studentReportCardGrades;
    this.cloneResponseStudentReportCardGradesModel = JSON.stringify(this.responseStudentReportCardGradesModel);
    // this.getAllGradeScaleList();
    this.studentReportCardGrades.courseSectionWithGradesViewModelList.map((item) => {
      item.gradeScaleList = this.getGradeScaleList(item);
      item.minValue = this.getMinValue(item);
      // item.maxValue = this.getMaxValue(item);
    });
    this.showUpdate = false;
  }

  getGradeScaleList(grade) {
    let gradeDataSet = [];
    if (this.gradeScaleList) {
      this.gradeScaleList.map(item => {
        if (item.gradeScaleId === grade.gradeScaleId) {
          gradeDataSet = item.grade;
        }
      });
    }
    return gradeDataSet;
  }

  getMinValue(grade) {
    let sortedData = [];
    let minValue = 0;
    if (this.gradeScaleList) {
      this.gradeScaleList.map(item => {
        if (item.gradeScaleId === grade.gradeScaleId) {
          sortedData = item.grade.sort((a, b) => a.breakoff - b.breakoff);
          minValue = sortedData[0].breakoff;
        }
      });
    }
    return minValue;
  }

  // getMaxValue(grade) {
  //   let sortedData = [];
  //   let maxValue = 0
  //   if (this.gradeScaleList) {
  //     this.gradeScaleList.map(item => {
  //       if (item.gradeScaleId === grade.gradeScaleId) {
  //         sortedData = item.grade.sort((a, b) => b.breakoff - a.breakoff);
  //         if (sortedData.length === 0) {
  //           maxValue = sortedData[0].breakoff;
  //         } else {
  //           maxValue = sortedData[sortedData.length - 1].breakoff;
  //         }
  //       }
  //     });
  //   }
  //   return maxValue;
  // }

  // getAllGradeScaleList() {
  //   this.gradesService.getAllGradeScaleList(this.gradeScaleListView).subscribe(data => {
  //     if (data._failure) {
  //       this.commonService.checkTokenValidOrNot(data._message);
  //       if (!data.gradeScaleList) {
  //         this.snackbar.open(data._message, '', {
  //           duration: 10000
  //         });
  //       }
  //     } else {
  //       // this.gradeScaleStandardList = data.gradeScaleList.filter(x => x.gradeScaleId === +standardGradeScaleId)[0]?.grade;
  //       this.gradeScaleList = data.gradeScaleList.filter(x => x.useAsStandardGradeScale === false);
  //       this.studentReportCardGrades.courseSectionWithGradesViewModelList.map((item) => {
  //         item.gradeScaleList = this.getGradeScaleList(item)
  //       });
  //     }
  //   });
  // }

  // For open the data chage history dialog
  openDataEdit(element) {
    this.dialog.open(DataEditInfoComponent, {
      width: '500px',
      data: { createdBy: element.createdBy, createdOn: element.createdOn, modifiedBy: element.updatedBy, modifiedOn: element.updatedOn }
    });
  }

  gradeToPercent(grade, index, data) {
    this.responseStudentReportCardGradesModel.courseSectionWithGradesViewModelList[index].percentMarks = data.filter(x => x.title === grade)[0].breakoff;
  }

  percentToGrade(percent, index, data) {
    let sortedData = [];
    sortedData = data.sort((a, b) => b.breakoff - a.breakoff);
    sortedData.map((item, i) => {
      if (i === 0) {
        if (percent >= item.breakoff) {
          this.responseStudentReportCardGradesModel.courseSectionWithGradesViewModelList[index].gradeObtained = item.title;
        }
      } else {
        if (percent >= item.breakoff && percent < sortedData[i - 1].breakoff) {
          this.responseStudentReportCardGradesModel.courseSectionWithGradesViewModelList[index].gradeObtained = item.title;
        }
      }
    });
  }

  omitSpecialChar(event) {
    let k = event.charCode
    return ((k >= 48 && k <= 57) || k === 8 || k === 46);
  }

  cancel() {
    this.studentReportCardGrades = JSON.parse(this.cloneResponseStudentReportCardGradesModel);
    this.studentReportCardGrades.courseSectionWithGradesViewModelList.map((item) => {
      item.gradeScaleList = this.getGradeScaleList(item)
      item.minValue = this.getMinValue(item);
      // item.maxValue = this.getMaxValue(item);
    });
    this.responseStudentReportCardGradesModel = JSON.parse(this.cloneResponseStudentReportCardGradesModel);
    this.showUpdate = false;
  }

  submit() {
    this.updateData.emit(this.responseStudentReportCardGradesModel);
  }

}
