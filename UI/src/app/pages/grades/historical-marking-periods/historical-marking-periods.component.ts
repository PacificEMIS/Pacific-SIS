import { Component, OnInit, ViewChild } from '@angular/core';
import icSearch from '@iconify/icons-ic/search';
import icFilterList from '@iconify/icons-ic/filter-list';
import icAdd from '@iconify/icons-ic/baseline-add';
import icEdit from '@iconify/icons-ic/twotone-edit';
import icDelete from '@iconify/icons-ic/twotone-delete';
import { TranslateService } from '@ngx-translate/core';
import { MatDialog } from '@angular/material/dialog';
import { AddHistoricalMarkingPeriodsComponent } from './add-historical-marking-periods/add-historical-marking-periods.component';
import { HistoricalMarkingPeriodService } from 'src/app/services/historical-marking-period.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { SharedFunction } from '../../shared/shared-function';
import { CommonService } from 'src/app/services/common.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HistoricalMarkingPeriodAddViewModel, HistoricalMarkingPeriodListModel } from 'src/app/models/historical-marking-period.model';
import { MatPaginator } from '@angular/material/paginator';
import { FormControl } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { LoaderService } from 'src/app/services/loader.service';
import { ExcelService } from 'src/app/services/excel.service';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import * as _moment from 'moment';
import { ConfirmDialogComponent } from '../../shared-module/confirm-dialog/confirm-dialog.component';
@Component({
  selector: 'vex-historical-marking-periods',
  templateUrl: './historical-marking-periods.component.html',
  styleUrls: ['./historical-marking-periods.component.scss']
})
export class HistoricalMarkingPeriodsComponent implements OnInit {

  icSearch = icSearch;
  icFilterList = icFilterList;
  icAdd = icAdd;
  icEdit = icEdit;
  icDelete = icDelete;
  totalCount = 0;
  pageNumber: number;
  pageSize: number;
  loading: boolean;
  searchCtrl: FormControl;
  historicalMarkingPeriodAddViewModel: HistoricalMarkingPeriodAddViewModel = new HistoricalMarkingPeriodAddViewModel();
  markingPeriodList: MatTableDataSource<any> = new MatTableDataSource<any>();
  columns = [
    { label: 'Marking Period Name', property: 'markingPeriodName', type: 'text', visible: true },
    { label: 'Grade Post Date', property: 'gradePostDate', type: 'text', visible: true },
    { label: 'School Year', property: 'schoolYear', type: 'text', visible: true },
    { label: 'Action', property: 'action', type: 'text', visible: true }
  ];
  historicalMarkingPeriodListModel: HistoricalMarkingPeriodListModel = new HistoricalMarkingPeriodListModel();

  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  constructor(public translateService: TranslateService,
    private historicalMarkingPeriodService: HistoricalMarkingPeriodService,
    public defaultValuesService: DefaultValuesService,
    private commonFunction: SharedFunction,
    private commonService: CommonService,
    private loaderService: LoaderService,
    private excelService: ExcelService,
    private snackbar: MatSnackBar,
    private dialog: MatDialog) {

    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    });
    if(!defaultValuesService.checkAcademicYear()){
      this.columns.pop()
    }
  }

  ngOnInit(): void {
    this.searchCtrl = new FormControl();
    this.getAllMerkingPeriodList();
  }

  ngAfterViewInit() {

    //  Searching
    this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term != '') {
        let filterParams = [
          {
            columnName: null,
            filterValue: term,
            filterOption: 3
          }
        ]

        Object.assign(this.historicalMarkingPeriodListModel, { filterParams: filterParams });
        this.historicalMarkingPeriodListModel.pageNumber = 1;
        this.paginator.pageIndex = 0;
        this.historicalMarkingPeriodListModel.pageSize = this.pageSize;
        this.getAllMerkingPeriodList();

      }
      else {
        Object.assign(this.historicalMarkingPeriodListModel, { filterParams: null });
        this.historicalMarkingPeriodListModel.pageNumber = this.paginator.pageIndex + 1;
        this.historicalMarkingPeriodListModel.pageSize = this.pageSize;
        this.getAllMerkingPeriodList();

      }
    })

  }

  goToAdd() {
    this.dialog.open(AddHistoricalMarkingPeriodsComponent, {
      width: '500px'
    }).afterClosed().subscribe((data) => {
      if (data === 'submited') {
        this.getAllMerkingPeriodList();
      }
    });
  }

  goToEdit(element) {
    this.dialog.open(AddHistoricalMarkingPeriodsComponent, {
      data: element,
      width: '600px'
    }).afterClosed().subscribe((data) => {
      if (data === 'submited') {
        this.getAllMerkingPeriodList();
      }
    });
  }

  confirmDelete(element) {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: {
        title: 'Are you sure?',
        message: 'You are about to delete ' + element.title + '.'
      }
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult) {
        this.deleteHistorydata(element);
      }
    });
  }

  deleteHistorydata(element){
    this.historicalMarkingPeriodAddViewModel.historicalMarkingPeriod = element;
    this.historicalMarkingPeriodService.deleteHistoricalMarkingPeriod(this.historicalMarkingPeriodAddViewModel).subscribe(
      (res: HistoricalMarkingPeriodAddViewModel) => {
        if (res) {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
          else {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
            this.getAllMerkingPeriodList();
          }
        }
        else{
          this.snackbar.open( this.defaultValuesService.getHttpError(), '', {
            duration: 10000
          });
        }
      }
    );
  }

  getAllMerkingPeriodList() {
    this.historicalMarkingPeriodService.getAllhistoricalMarkingPeriodList(this.historicalMarkingPeriodListModel).subscribe(
      (res) => {
        if (res) {
          if (res._failure) {
            this.commonService.checkTokenValidOrNot(res._message);
            if (res.historicalMarkingPeriodList === null) {
              this.totalCount = null;
              this.markingPeriodList = new MatTableDataSource([]);
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
            } else {
              this.markingPeriodList = new MatTableDataSource([]);
              this.totalCount = null;
            }
          }
          else {
            this.markingPeriodList = new MatTableDataSource(res.historicalMarkingPeriodList);
            this.totalCount = res.totalCount;
            this.pageSize = res._pageSize;
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


  toggleColumnVisibility(column, event: Event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }
  getPageEvent(event) {
    if (this.searchCtrl.value != null && this.searchCtrl.value.trim() !== '') {
      const filterParams = [
        {
          columnName: null,
          filterValue: this.searchCtrl.value,
          filterOption: 1
        }
      ];
      Object.assign(this.historicalMarkingPeriodListModel, { filterParams });
    }
    this.historicalMarkingPeriodListModel.pageNumber = event.pageIndex + 1;
    this.historicalMarkingPeriodListModel.pageSize = event.pageSize;
    this.getAllMerkingPeriodList()
  }

  exportToExcel() {
    const historicalMarkingPeriodListModel: HistoricalMarkingPeriodListModel = new HistoricalMarkingPeriodListModel();
    historicalMarkingPeriodListModel.pageNumber = 0;
    historicalMarkingPeriodListModel.pageSize = 0;
    this.historicalMarkingPeriodService.getAllhistoricalMarkingPeriodList(historicalMarkingPeriodListModel).subscribe(
      (res) => {
        if (res._failure) {
          this.commonService.checkTokenValidOrNot(res._message);
          this.snackbar.open('Failed to export historical marking periods list.', '', {
            duration: 10000
          });
        }
        else {
          if (res.historicalMarkingPeriodList?.length > 0) {
            const reportList = res.historicalMarkingPeriodList?.map((x) => {
              return {
                [this.defaultValuesService.translateKey('markingPeriodName')]: x.title,
                [this.defaultValuesService.translateKey('gradePostDate')]: _moment(x.gradePostDate).format('MMMM yyyy'),
                [this.defaultValuesService.translateKey('schoolYear')]: x.academicYear
              };
            });
            this.excelService.exportAsExcelFile(reportList, 'Historical_Marking_Periods_List_');
          } else {
            this.snackbar.open('No records found. Failed to export Historical marking periods list', '', {
              duration: 5000
            });
          }
        }
      }
    );
  }

}
