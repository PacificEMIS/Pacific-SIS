import { Component, OnInit, AfterViewInit, ViewChild, OnDestroy } from '@angular/core';
import { FormControl } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTableDataSource } from '@angular/material/table';
import { TranslateService } from '@ngx-translate/core';
import icSearch from '@iconify/icons-ic/search';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { stagger40ms } from '../../../../@vex/animations/stagger.animation';
import { CommonService } from '../../../services/common.service';
import { DefaultValuesService } from '../../../common/default-values.service';
import { LoaderService } from '../../../services/loader.service';
import { StaffPortalService } from '../../../services/staff-portal.service';
import { TeacherScheduleService } from '../../../services/teacher-schedule.service';
import { AnomalousGradeViewModel, StudentAnomalsGrade } from '../../../models/anomalous-grade.model';
import { AllScheduledCourseSectionForStaffModel } from '../../../models/teacher-schedule.model';

@Component({
  selector: 'vex-anomalous-grades',
  templateUrl: './anomalous-grades.component.html',
  styleUrls: ['./anomalous-grades.component.scss'],
  animations: [fadeInUp400ms, stagger40ms]
})
export class AnomalousGradesComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild(MatPaginator) paginator: MatPaginator;

  icSearch = icSearch;
  loading = false;
  searchCtrl = new FormControl();
  includeInactive = false;

  displayedColumns = ['studentName', 'studentInternalId', 'courseSection', 'assignmentType', 'assignment', 'status', 'marks', 'comment'];
  dataSource = new MatTableDataSource<StudentAnomalsGrade>([]);

  request = new AnomalousGradeViewModel();
  courseSections: AllScheduledCourseSectionForStaffModel = new AllScheduledCourseSectionForStaffModel();
  selectedCourseSectionId: number | null = null;
  totalCount = 0;
  pageSize = 10;
  pageNumber = 1;

  private destroy$ = new Subject<void>();

  constructor(
    public translateService: TranslateService,
    public defaultValuesService: DefaultValuesService,
    private staffPortalService: StaffPortalService,
    private teacherScheduleService: TeacherScheduleService,
    private commonService: CommonService,
    private snackbar: MatSnackBar,
    private loaderService: LoaderService
  ) {
    this.loaderService.isLoading.pipe(takeUntil(this.destroy$)).subscribe(v => this.loading = v);
  }

  ngOnInit(): void {
    this.request.staffId = this.defaultValuesService.getUserId();
    this.loadCourseSections();
    this.fetch();
  }

  ngAfterViewInit(): void {
    this.searchCtrl.valueChanges
      .pipe(debounceTime(500), distinctUntilChanged(), takeUntil(this.destroy$))
      .subscribe(term => {
        this.pageNumber = 1;
        if (this.paginator) this.paginator.firstPage();
        this.fetch(term ?? null);
      });
  }

  loadCourseSections(): void {
    this.courseSections.staffId = this.request.staffId;
    this.teacherScheduleService.getAllScheduledCourseSectionForStaff(this.courseSections).subscribe(res => {
      if (!res) return;
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
        this.courseSections.courseSectionViewList = [];
      } else {
        this.courseSections = res;
      }
    });
  }

  onCourseSectionChange(): void {
    this.pageNumber = 1;
    if (this.paginator) this.paginator.firstPage();
    this.fetch();
  }

  onIncludeInactiveChange(): void {
    this.pageNumber = 1;
    if (this.paginator) this.paginator.firstPage();
    this.fetch();
  }

  onPageChange(event: any): void {
    this.pageSize = event.pageSize;
    this.pageNumber = event.pageIndex + 1;
    this.fetch(this.searchCtrl.value);
  }

  fetch(searchValue?: string): void {
    this.request.searchValue = searchValue || null;
    this.request.courseSectionId = this.selectedCourseSectionId || null;
    this.request.includeInactive = this.includeInactive;
    this.request.pageNumber = this.pageNumber;
    this.request.pageSize = this.pageSize;

    this.staffPortalService.getAnomalousGrade(this.request).subscribe(res => {
      if (!res) {
        this.dataSource.data = [];
        this.totalCount = 0;
        return;
      }
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
        this.dataSource.data = [];
        this.totalCount = 0;
      } else {
        this.dataSource.data = res.studentAnomalsGrades || [];
        this.totalCount = res.totalCount || 0;
      }
    });
  }

  statusOf(row: StudentAnomalsGrade): string {
    if (row.allowedMarks === 'Missing') return 'missing';
    if (row.allowedMarks === '*') return 'starred';
    return 'overCap';
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}