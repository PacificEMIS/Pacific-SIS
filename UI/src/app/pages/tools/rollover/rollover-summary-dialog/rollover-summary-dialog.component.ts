import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import icClose from '@iconify/icons-ic/twotone-close';
import icWarning from '@iconify/icons-ic/warning';
import { TranslateService } from '@ngx-translate/core';
import { GradeGenderCount, RolloverReadinessViewModel, SectionCompletenessRow } from '../../../../models/roll-over.model';

export interface RolloverSummaryDialogData {
  summary: RolloverReadinessViewModel;
  academicYear: string;
  loadFailed: boolean;
  // Review-only: opened from the "year still in progress" state to review data
  // completeness ahead of time — no confirm/rollover button is offered.
  reviewOnly?: boolean;
}

// Grade rows pivoted into one column per gender, for compact display.
export interface PivotTable {
  genderLabels: string[];
  rows: { gradeLevelTitle: string; counts: number[]; total: number }[];
  columnTotals: number[];
  grandTotal: number;
}

export interface DispositionView {
  labelKey: string;
  isNotSet: boolean;
  total: number;
  pivot: PivotTable;
}

export interface ExitCodeView {
  title: string;
  total: number;
  pivot: PivotTable;
}

@Component({
  selector: 'vex-rollover-summary-dialog',
  templateUrl: './rollover-summary-dialog.component.html',
  styleUrls: ['./rollover-summary-dialog.component.scss']
})
export class RolloverSummaryDialogComponent implements OnInit {
  icClose = icClose;
  icWarning = icWarning;

  studentsPivot: PivotTable;
  totalStudents = 0;
  dispositions: DispositionView[] = [];
  notSetTotal = 0;
  terminalPivot: PivotTable;
  terminalGradeTitlesJoined = '';
  exitCodes: ExitCodeView[] = [];
  missingAttendance: SectionCompletenessRow[] = [];
  missingGrades: SectionCompletenessRow[] = [];
  unlinkedEnrollmentCount = 0;
  disabledWithOpenEnrollmentCount = 0;

  private dispositionLabelKeys = {
    'Next grade at current school': 'rolloverDispositionPromotees',
    'Retain': 'rolloverDispositionRepeaters',
    'Do not enroll after this school year': 'rolloverDispositionCompleters',
    'Enrol to another school': 'rolloverDispositionTransferring',
    'NotSet': 'rolloverDispositionNotSet'
  };

  constructor(
    public dialogRef: MatDialogRef<RolloverSummaryDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: RolloverSummaryDialogData,
    public translateService: TranslateService) { }

  ngOnInit(): void {
    const summary = this.data.summary;
    if (!summary || this.data.loadFailed) {
      return;
    }
    this.studentsPivot = this.buildPivot(summary.studentsByGradeGender);
    this.dispositions = (summary.dispositions || []).map(d => ({
      labelKey: this.dispositionLabelKeys[d.dispositionKey] || d.dispositionKey,
      isNotSet: d.dispositionKey === 'NotSet',
      total: d.total,
      pivot: this.buildPivot(d.rows)
    }));
    const notSet = (summary.dispositions || []).find(d => d.dispositionKey === 'NotSet');
    this.notSetTotal = notSet ? notSet.total : 0;
    this.terminalPivot = this.buildPivot(summary.terminalGradeNeedsReview);
    this.terminalGradeTitlesJoined = (summary.terminalGradeTitles || []).join(', ');
    this.exitCodes = (summary.exitsByCode || []).map(e => ({
      title: e.exitCodeTitle,
      total: e.total,
      pivot: this.buildPivot(e.rows)
    }));
    this.totalStudents = summary.totalStudents || 0;
    this.missingAttendance = summary.sectionsMissingAttendance || [];
    this.missingGrades = summary.sectionsMissingGrades || [];
    this.unlinkedEnrollmentCount = summary.unlinkedEnrollmentCount || 0;
    this.disabledWithOpenEnrollmentCount = summary.disabledWithOpenEnrollmentCount || 0;
  }

  buildPivot(items: GradeGenderCount[]): PivotTable {
    const unspecified = this.translateService.instant('unspecified');
    const list = items || [];
    const genderLabels: string[] = [];
    list.forEach(i => {
      const g = i.gender ? i.gender : unspecified;
      if (!genderLabels.includes(g)) {
        genderLabels.push(g);
      }
    });
    genderLabels.sort((a, b) => a === unspecified ? 1 : b === unspecified ? -1 : a.localeCompare(b));

    const rowMap = new Map<string, { gradeLevelTitle: string; sortOrder: number; counts: number[]; total: number }>();
    list.forEach(i => {
      const title = i.gradeLevelTitle ? i.gradeLevelTitle : unspecified;
      if (!rowMap.has(title)) {
        rowMap.set(title, {
          gradeLevelTitle: title,
          sortOrder: i.sortOrder != null ? i.sortOrder : 999,
          counts: genderLabels.map(() => 0),
          total: 0
        });
      }
      const row = rowMap.get(title);
      const genderIndex = genderLabels.indexOf(i.gender ? i.gender : unspecified);
      row.counts[genderIndex] += i.count;
      row.total += i.count;
    });

    const rows = Array.from(rowMap.values()).sort((a, b) => a.sortOrder - b.sortOrder);
    const columnTotals = genderLabels.map((g, idx) => rows.reduce((sum, r) => sum + r.counts[idx], 0));
    const grandTotal = rows.reduce((sum, r) => sum + r.total, 0);
    return { genderLabels, rows, columnTotals, grandTotal };
  }

  onConfirm(): void {
    this.dialogRef.close(true);
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
