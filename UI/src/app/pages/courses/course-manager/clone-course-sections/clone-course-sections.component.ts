import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import icClose from '@iconify/icons-ic/twotone-close';
import icCheck from '@iconify/icons-ic/baseline-check-circle';
import icSkip from '@iconify/icons-ic/baseline-remove-circle-outline';
import icWarning from '@iconify/icons-ic/baseline-warning';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { CloneCourseSectionsModel } from 'src/app/models/course-manager.model';
import { GetAcademicYearListModel } from 'src/app/models/marking-period.model';
import { CommonService } from 'src/app/services/common.service';
import { CourseManagerService } from 'src/app/services/course-manager.service';
import { MarkingPeriodService } from 'src/app/services/marking-period.service';

/**
 * Copies the course sections of a past academic year into the year the user is
 * working in. Opened from the course list for the whole school, or from a single
 * course's section list with `data.courseId` set to clone that course only.
 * The user previews first, then clones; the dialog closes with `true` once
 * something was written so the caller can refresh.
 */
@Component({
  selector: 'vex-clone-course-sections',
  templateUrl: './clone-course-sections.component.html',
  styleUrls: ['./clone-course-sections.component.scss']
})
export class CloneCourseSectionsComponent implements OnInit {
  icClose = icClose;
  icCheck = icCheck;
  icSkip = icSkip;
  icWarning = icWarning;

  form: FormGroup;
  loading = false;
  academicYears: { academyYear: number; year: string }[] = [];
  currentYear: number;
  currentYearLabel: string;
  result: CloneCourseSectionsModel = null;
  previewed = false;
  done = false;

  constructor(
    private dialogRef: MatDialogRef<CloneCourseSectionsComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { courseId?: number; courseTitle?: string },
    private fb: FormBuilder,
    private courseManager: CourseManagerService,
    private markingPeriodService: MarkingPeriodService,
    private snackbar: MatSnackBar,
    public defaultValuesService: DefaultValuesService,
    private commonService: CommonService
  ) { }

  ngOnInit(): void {
    this.currentYear = Number(this.defaultValuesService.getAcademicYear());
    this.currentYearLabel = String(this.currentYear);
    this.form = this.fb.group({
      sourceYear: [null, Validators.required],
      includeStaff: [true]
    });
    this.loadAcademicYears();
  }

  private loadAcademicYears() {
    this.loading = true;
    this.markingPeriodService.getAcademicYearList(new GetAcademicYearListModel()).subscribe((res: any) => {
      this.loading = false;
      if (!res) {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', { duration: 10000 });
        return;
      }
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
        this.snackbar.open(res._message, '', { duration: 10000 });
        return;
      }
      const years: { academyYear: number; year: string }[] = res.academicYears || [];
      const current = years.find(y => y.academyYear === this.currentYear);
      if (current) {
        this.currentYearLabel = current.year;
      }
      this.academicYears = years
        .filter(y => y.academyYear !== this.currentYear)
        .sort((a, b) => b.academyYear - a.academyYear);
      const previous = this.academicYears.find(y => y.academyYear < this.currentYear) || this.academicYears[0];
      if (previous) {
        this.form.patchValue({ sourceYear: previous.academyYear });
      }
    });
  }

  /** The year selection and teacher option are locked once a preview exists; changing them clears it. */
  onOptionsChanged() {
    this.result = null;
    this.previewed = false;
  }

  preview() {
    this.run(true);
  }

  clone() {
    this.run(false);
  }

  private run(dryRun: boolean) {
    if (this.form.invalid || this.loading || this.done) {
      return;
    }
    const request = new CloneCourseSectionsModel();
    request.sourceAcademicYear = this.form.value.sourceYear;
    request.includeStaff = !!this.form.value.includeStaff;
    request.dryRun = dryRun;
    request.courseId = this.data?.courseId ?? null;

    this.loading = true;
    this.courseManager.cloneCourseSections(request).subscribe(res => {
      this.loading = false;
      if (!res) {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', { duration: 10000 });
        return;
      }
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
        this.result = null;
        this.previewed = false;
        this.snackbar.open(res._message, '', { duration: 10000 });
        return;
      }
      this.result = res;
      this.previewed = dryRun;
      this.done = !dryRun;
      if (this.done) {
        this.form.disable();
      }
      this.snackbar.open(res._message, '', { duration: 10000 });
    });
  }

  close() {
    this.dialogRef.close(this.done);
  }
}
