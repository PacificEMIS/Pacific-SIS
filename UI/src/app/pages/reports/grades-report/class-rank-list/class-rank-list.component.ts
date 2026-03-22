import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { AdvancedSearchExpansionModel } from 'src/app/models/common.model';
import { GetAllGradeLevelsModel } from 'src/app/models/grade-level.model';
import { CommonService } from 'src/app/services/common.service';
import { GradeLevelService } from 'src/app/services/grade-level.service';
import { LoaderService } from 'src/app/services/loader.service';
import { ReportService } from 'src/app/services/report.service';
import { fadeInRight400ms } from 'src/@vex/animations/fade-in-right.animation';

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

@Component({
  selector: 'vex-class-rank-list',
  templateUrl: './class-rank-list.component.html',
  styleUrls: ['./class-rank-list.component.scss'],
  animations: [
    fadeInRight400ms
  ]
})
export class ClassRankListComponent implements OnInit {

  displayedColumns: string[] = ['studentName', 'studentId', 'alternateId', 'grade', 'section', 'gpa', 'unweightedGpa', 'weightedGpa', 'classRank'];
  studentList;
  gradeLevelList: GetAllGradeLevelsModel = new GetAllGradeLevelsModel();
  filterForm!: FormGroup;
  selectedGradeId;
  destroySubject$: Subject<void> = new Subject();
  loading: boolean;
  totalCount;
  pageNumber: number;
  pageSize: number;
  currentFilterParams: any[] = [];
  advancedSearchExpansionModel: AdvancedSearchExpansionModel = new AdvancedSearchExpansionModel();
  showAdvanceSearchPanel: boolean = false;
  isFromAdvancedSearch: boolean = false;
  searchValue;
  toggleValues;

  constructor(
    public translateService: TranslateService,
    private paginatorObj: MatPaginatorIntl,
    private defaultValuesService: DefaultValuesService,
    private gradeLevelService: GradeLevelService,
    private snackbar: MatSnackBar,
    private commonService: CommonService,
    private fb: FormBuilder,
    private reportService: ReportService,
    private defaultService: DefaultValuesService,
    private loaderService: LoaderService,
  ) {
    this.advancedSearchExpansionModel.accessInformation = false;
    this.advancedSearchExpansionModel.enrollmentInformation = false;
    this.advancedSearchExpansionModel.searchAllSchools = false;
    this.defaultValuesService.setReportCompoentTitle.next("GPA / Class Rank List");
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
  }

  ngOnInit() {
  this.getAllGradeLevel();
  this.filterForm = this.fb.group({
    searchText: [''],
    gradeLevel: ['']
  });

  this.getClassRankList([]);
  this.filterForm.get('searchText')!.valueChanges
    .pipe(debounceTime(400), distinctUntilChanged(), takeUntil(this.destroySubject$))
    .subscribe(searchValue => {
      if (searchValue) {
        this.filterForm.patchValue({ gradeLevel: '' }, { emitEvent: false });
      }

      const filterParams = this.buildFilterParams(searchValue, this.filterForm.value.gradeLevel);
      this.currentFilterParams = filterParams;
      this.getClassRankList(filterParams);
    });
    this.filterForm.get('gradeLevel')!.valueChanges
    .pipe(distinctUntilChanged(), takeUntil(this.destroySubject$))
    .subscribe(gradeValue => {
      if (gradeValue) {
        this.filterForm.patchValue({ searchText: '' }, { emitEvent: false });
      }

      const filterParams = this.buildFilterParams(this.filterForm.value.searchText, gradeValue);
      this.currentFilterParams = filterParams;
      this.getClassRankList(filterParams);
    });
}


  private buildFilterParams(searchText: string, gradeLevel: string) {
  const filterParams: any[] = [];

    if (searchText && !gradeLevel) {
      filterParams.push({
        columnName: null,
        filterValue: searchText.trim(),
        filterOption: 3
      });
    } else if (!searchText && gradeLevel) {
      filterParams.push({
        filterOption: 11,
        columnName: 'gradeId',
        filterValue: gradeLevel
      });
    }

    return filterParams;
  }

  getClassRankList(filterParams?: any) {
    const requestBody = {
      filterParams,
      sortingModel: null,
      pageNumber: this.pageNumber || 1,
      pageSize: this.pageSize || 10
    };

    this.reportService.getClassRankListReport(requestBody).subscribe(res => {

      if (res) {
        if (res._failure) {
          this.studentList = [];
          this.totalCount = 0;
          this.snackbar.open(res._message, '', { duration: 10000 });
        } else {
          this.studentList = res?.studentCgpaDetails || [];
          this.totalCount = res?.totalCount || this.studentList.length;
          this.pageSize = res?._pageSize;
        }
      } else {
        this.studentList = [];
        this.totalCount = 0;
        this.snackbar.open(this.defaultService.getHttpError(), '', { duration: 10000 });
      }
    });
  }


  getAllGradeLevel() {
    this.gradeLevelService.getAllGradeLevels(this.gradeLevelList).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        this.gradeLevelList.tableGradelevelList = []
        this.snackbar.open('Grade Level List failed. ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
      else {
        if (res._failure) {

        } else {
          this.gradeLevelList = res;
        }
      }
    });
  }

  getPageEvent(event) {
    this.pageNumber = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.getClassRankList(this.currentFilterParams);
  }

  filterData(res) {
    this.isFromAdvancedSearch = true;
    this.pageNumber = 1;
    if (res) {
      this.currentFilterParams = res.filterParams;
      this.filterForm.patchValue({ searchText: '', gradeLevel: '' }, { emitEvent: false });
      this.getClassRankList(res.filterParams);
    }
  }

  getToggleValues(event) {
    this.toggleValues = event;
  }

  hideAdvanceSearch(event) {
    this.showAdvanceSearchPanel = false;
  }

  getSearchInput(event) {
    this.searchValue = event;
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}
