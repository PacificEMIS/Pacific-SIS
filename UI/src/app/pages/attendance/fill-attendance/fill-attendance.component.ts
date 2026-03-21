import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { StudentAttendanceService } from '../../../services/student-attendance.service';
import { CommonService } from '../../../services/common.service';
import { DefaultValuesService } from '../../../common/default-values.service';
import { CourseSectionForAttendanceViewModel } from '../../../models/attendance-administrative.model';
import { FillAttendanceViewModel } from '../../../models/fill-attendance.model';
import { LoaderService } from '../../../services/loader.service';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';

@Component({
  selector: 'vex-fill-attendance',
  templateUrl: './fill-attendance.component.html',
  styleUrls: ['./fill-attendance.component.scss'],
  animations: [fadeInUp400ms]
})
export class FillAttendanceComponent implements OnInit {
  destroySubject$: Subject<void> = new Subject();
  courseSectionViewList = [];
  selectedCourseSectionId: number = 0;
  loading: boolean;
  processing: boolean = false;
  resultMessage: string = null;
  resultSuccess: boolean = false;
  totalRecordsCreated: number = 0;
  courseSectionsProcessed: number = 0;
  showConfirmation: boolean = false;
  courseSectionList: CourseSectionForAttendanceViewModel = new CourseSectionForAttendanceViewModel();

  constructor(
    public translateService: TranslateService,
    private studentAttendanceService: StudentAttendanceService,
    private commonService: CommonService,
    private defaultValueService: DefaultValuesService,
    private loaderService: LoaderService,
    private snackbar: MatSnackBar
  ) {
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
  }

  ngOnInit(): void {
    this.courseSectionListForAttendanceAdministration();
  }

  courseSectionListForAttendanceAdministration() {
    this.studentAttendanceService.courseSectionListForAttendanceAdministration(this.courseSectionList).subscribe((res: CourseSectionForAttendanceViewModel) => {
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
        this.courseSectionViewList = [];
      } else {
        this.courseSectionViewList = res.courseSectionViewList;
      }
    });
  }

  promptConfirmation() {
    this.showConfirmation = true;
    this.resultMessage = null;
  }

  cancelFill() {
    this.showConfirmation = false;
  }

  fillAttendance() {
    this.showConfirmation = false;
    this.processing = true;
    this.resultMessage = null;

    const model: FillAttendanceViewModel = new FillAttendanceViewModel();
    model.courseSectionId = this.selectedCourseSectionId === 0 ? null : this.selectedCourseSectionId;

    this.studentAttendanceService.fillAttendanceAsPresent(model).subscribe((res: FillAttendanceViewModel) => {
      this.processing = false;
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
        this.resultSuccess = false;
        this.resultMessage = res._message;
        this.snackbar.open(res._message, '', { duration: 10000 });
      } else {
        this.resultSuccess = true;
        this.totalRecordsCreated = res.totalRecordsCreated;
        this.courseSectionsProcessed = res.courseSectionsProcessed;
        this.resultMessage = res._message;
        this.snackbar.open(res._message, '', { duration: 10000 });
      }
    });
  }

  getSelectionLabel(): string {
    if (this.selectedCourseSectionId === 0) {
      return this.translateService.instant('allCourseSections');
    }
    const section = this.courseSectionViewList.find(s => s.courseSectionId === this.selectedCourseSectionId);
    return section ? section.courseSectionName : '';
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
