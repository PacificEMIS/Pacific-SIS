import { Component, EventEmitter, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import icRemoveCircle from '@iconify/icons-ic/twotone-remove-circle';
import icEdit from '@iconify/icons-ic/twotone-edit';
import icDeleteForever from '@iconify/icons-ic/twotone-delete-forever';
import icAdd from '@iconify/icons-ic/baseline-add';
import { HistoricalCreditTransfer, HistoricalGrade, HistoricalGradeAddViewModel, HistoricalMarkingPeriodListModel } from 'src/app/models/historical-marking-period.model';
import { HistoricalMarkingPeriodService } from 'src/app/services/historical-marking-period.service';
import { CommonService } from 'src/app/services/common.service';
import { GelAllGradeEquivalencyModel } from 'src/app/models/grade-level.model';
import { GradeLevelService } from 'src/app/services/grade-level.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { takeUntil } from 'rxjs/operators';
import { LoaderService } from 'src/app/services/loader.service';
import { Subject } from 'rxjs';
import { NgForm } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'vex-historical-grades-details',
  templateUrl: './historical-grades-details.component.html',
  styleUrls: ['./historical-grades-details.component.scss']
})
export class HistoricalGradesDetailsComponent implements OnInit, OnDestroy {
  destroySubject$: Subject<void> = new Subject();
  icRemoveCircle = icRemoveCircle;
  icEdit = icEdit;
  icDeleteForever = icDeleteForever;
  icAdd = icAdd;
  loading: boolean;
  selectedMarkingPeriodName: string;
  histStudentDetails;
  equivalencyList = [];
  @ViewChild('f') currentForm: NgForm;
  getGradeEquivalencyList: GelAllGradeEquivalencyModel = new GelAllGradeEquivalencyModel();
  historicalMarkingPeriodList: HistoricalMarkingPeriodListModel = new HistoricalMarkingPeriodListModel();
  historicalGradeAddViewModel: HistoricalGradeAddViewModel = new HistoricalGradeAddViewModel();
  historicalGrade: HistoricalGrade = new HistoricalGrade();
  allCourseTypes = []
  @Output() newItemEvent = new EventEmitter<string>();

  constructor(private historicalMarkingPeriodService: HistoricalMarkingPeriodService,
    private commonService: CommonService,
    private gradeLevelService: GradeLevelService,
    private defaultValuesService: DefaultValuesService,
    private loaderService: LoaderService,
    private snackbar: MatSnackBar,
    private router: Router,
    public translateService: TranslateService
  ) {
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
    this.histStudentDetails = this.historicalMarkingPeriodService.getHistStudentDetails();
    this.allCourseTypes = ['Regular','Honors','AP','IB','College']
  }

  ngOnInit(): void {
    this.getAllHistoricalmarkingPeriod();
    this.getGradeEquivalency();
  }

  redirectToMarkingPeriod() {
    this.defaultValuesService.setPageId('Historical Marking Periods');
    this.router.navigate(["school/settings/grade-settings"]);
  }

  getAllHistoricalmarkingPeriod() {
    this.historicalMarkingPeriodService.getAllhistoricalMarkingPeriodList(this.historicalMarkingPeriodList).subscribe(
      (res) => {
        if (res) {
          if (res._failure) {
            
          }
          else {
            this.historicalMarkingPeriodList.historicalMarkingPeriodList = res.historicalMarkingPeriodList;
            this.getAllHistoricalGradeList();
          }
        }
      }
    );

  }

  getGradeEquivalency() {
    this.gradeLevelService.getAllGradeEquivalency(this.getGradeEquivalencyList).subscribe((res) => {

      if (res._failure) {
        
        this.getGradeEquivalencyList = new GelAllGradeEquivalencyModel();
      }
      else {
        this.getGradeEquivalencyList = res;
      }
    })
  }


  histGradeEdit(index) {
    this.historicalGradeAddViewModel.historicalGradeList[index].gradeViewMode = false;
    this.historicalGradeAddViewModel.historicalGradeList[index].gradeAddMode = true;
    let lastIndex = this.historicalGradeAddViewModel.historicalGradeList[index].historicalCreditTransfer.length - 1;
    if (this.historicalGradeAddViewModel.historicalGradeList[index].historicalCreditTransfer[lastIndex].isDefaultRow) {
      this.historicalGradeAddViewModel.historicalGradeList[index].historicalCreditTransfer.pop();
    }
  }

  deleteHistoricalGrade(index) {
    this.historicalGradeAddViewModel.historicalGradeList.splice(index, 1);
    // this.historicalGradeAddViewModel.historicalGradeList.map(historicalValue=>{historicalValue.historicalCreditTransfer.pop();})    
    this.callAddEditHistGrade(this.historicalGradeAddViewModel,true);
  }

  creditEdit(histIndex, creditIndex) {
    this.historicalGradeAddViewModel.historicalGradeList[histIndex].historicalCreditTransfer[creditIndex].creditViewMode = false;
    this.historicalGradeAddViewModel.historicalGradeList[histIndex].historicalCreditTransfer[creditIndex].creditAddMode = true;
    // let lastIndex = this.historicalGradeAddViewModel.historicalGradeList[histIndex].historicalCreditTransfer.length - 1;
    // if (this.historicalGradeAddViewModel.historicalGradeList[histIndex].historicalCreditTransfer[lastIndex].isDefaultRow) {
      // this.historicalGradeAddViewModel.historicalGradeList[histIndex].historicalCreditTransfer.pop();
    // }
  }

  creditDelete(histIndex, creditIndex) {
    this.historicalGradeAddViewModel.historicalGradeList[histIndex].historicalCreditTransfer.splice(creditIndex, 1);
    // this.historicalGradeAddViewModel.historicalGradeList.map(historicalValue=>{historicalValue.historicalCreditTransfer.pop();})    
    this.callAddEditHistGrade(this.historicalGradeAddViewModel,true);
  }

  addMoreHistoricalGrade() {
    this.historicalGradeAddViewModel.historicalGradeList.push(new HistoricalGrade());
  }

  selectedMarkingPeriod(hrMarkingPeriodId) {
    if (hrMarkingPeriodId) {
      return this.historicalMarkingPeriodList.historicalMarkingPeriodList.filter(x => x.histMarkingPeriodId === +hrMarkingPeriodId)[0].academicYear + " - " + this.historicalMarkingPeriodList.historicalMarkingPeriodList.filter(x => x.histMarkingPeriodId === +hrMarkingPeriodId)[0].title;
    }
  }

  addCreditRow(index, event) {
    //this.historicalGradeAddViewModel.historicalGradeList[index].historicalCreditTransfer.push(new HistoricalCreditTransfer());
  }

  selectedGrade(gradeId) {
    if (gradeId !== null && gradeId !== undefined) {
      return this.getGradeEquivalencyList.gradeEquivalencyList.filter(x => x.equivalencyId === +gradeId)[0].gradeLevelEquivalency;
    }
  }

  addEditHistGrade() {
    this.currentForm.form.markAllAsTouched();
    if (this.currentForm.invalid) return;
    for (let i = 0; i < this.historicalGradeAddViewModel.historicalGradeList?.length; i++) {
      for (let j = 0; j < this.historicalGradeAddViewModel.historicalGradeList[i].historicalCreditTransfer?.length; j++) {
        if (this.historicalGradeAddViewModel.historicalGradeList[i].historicalCreditTransfer[j].creditEarned > this.historicalGradeAddViewModel.historicalGradeList[i].historicalCreditTransfer[j].creditAttempted) {
          this.snackbar.open('Credit Earned can be only equal to Credit Attempted or equal to 0.', '', {
            duration: 10000
          });
          return;
        }
      }
    }
    this.callAddEditHistGrade(this.historicalGradeAddViewModel);
  }

  callAddEditHistGrade(modelValues,isDelete?){
    this.historicalGradeAddViewModel.studentId = this.histStudentDetails.studentId;
    this.historicalMarkingPeriodService.addUpdateHistoricalGrade(modelValues).subscribe(
      (res: HistoricalGradeAddViewModel) => {
        if (res) {
          if (res._failure) {
            
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
          else {
            if(res._message === "Historical grade updated successfully" && isDelete === true)
              res._message = "Historical grade deleted successfully";
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
            this.historicalGradeAddViewModel = res;
            this.getAllHistoricalGradeList();
          }
        }
        else {
          this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      }
    );
  }
  getAllHistoricalGradeList() {
    this.historicalGradeAddViewModel.studentId = this.histStudentDetails.studentId;
    let cloneHistoricalGradeAddViewModel: HistoricalGradeAddViewModel = new HistoricalGradeAddViewModel();
    cloneHistoricalGradeAddViewModel = JSON.parse(JSON.stringify(this.historicalGradeAddViewModel));
    delete cloneHistoricalGradeAddViewModel.historicalGradeList;
    this.historicalMarkingPeriodService.getAllHistoricalGradeList(cloneHistoricalGradeAddViewModel).subscribe(
      (res) => {
        if (res._failure) {
          
          if (res.historicalGradeList === null) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          } else {
            this.historicalGradeAddViewModel = new HistoricalGradeAddViewModel();
          }
        }
        else {
          this.historicalGradeAddViewModel = res;
          this.newItemEvent.emit(this.historicalGradeAddViewModel.studentPhoto);
          this.historicalGradeAddViewModel.historicalGradeList.map(item => {
            item.historicalCreditTransfer.map(subItem => {
              subItem.courseCode && subItem.percentage ? subItem.isDefaultRow = false : subItem.isDefaultRow = true;
            });
          });
          for (let i = 0; i < this.historicalGradeAddViewModel.historicalGradeList?.length; i++) {
            for (let j = 0; j < this.historicalGradeAddViewModel.historicalGradeList[i].historicalCreditTransfer?.length; j++) {
              Object.assign(this.historicalGradeAddViewModel.historicalGradeList[i].historicalCreditTransfer[j], { creditAddMode: false });
            }
            Object.assign(this.historicalGradeAddViewModel.historicalGradeList[i], { gradeAddMode: false, gradeViewMode: true });
            if(this.historicalGradeAddViewModel.historicalGradeList[i].historicalCreditTransfer?.length === 0) {
              this.historicalGradeAddViewModel.historicalGradeList[i].historicalCreditTransfer.push(new HistoricalCreditTransfer());
            }
          }
        }
      }
    );

  }

  addCouseSection(index) {
    this.historicalGradeAddViewModel.historicalGradeList[index].historicalCreditTransfer.push(new HistoricalCreditTransfer());
  }

  removeCourseSection(index, history, subIndex) {    
    if(history.isNewEntry) {
      this.historicalGradeAddViewModel.historicalGradeList[index].historicalCreditTransfer.splice(subIndex, 1);
    } else {
      history.creditAddMode = false;
    }
  }

  removeMarkingPeriod(index, historicalGrade) {    
    if(historicalGrade.isNewEntry) {
      this.historicalGradeAddViewModel.historicalGradeList.splice(index, 1);
    } else {
      historicalGrade.gradeAddMode = false;
      historicalGrade.gradeViewMode = true;
    }
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
