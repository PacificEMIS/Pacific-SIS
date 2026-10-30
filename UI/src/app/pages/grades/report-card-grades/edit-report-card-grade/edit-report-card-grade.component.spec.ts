import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { EditReportCardGradeComponent } from './edit-report-card-grade.component';

describe('EditPeriodComponent', () => {
  let component: EditReportCardGradeComponent;
  let fixture: ComponentFixture<EditReportCardGradeComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ EditReportCardGradeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditReportCardGradeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
