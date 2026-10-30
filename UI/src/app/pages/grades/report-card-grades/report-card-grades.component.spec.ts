import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ReportCardGradesComponent } from './report-card-grades.component';

describe('ReportCardGradesComponent', () => {
  let component: ReportCardGradesComponent;
  let fixture: ComponentFixture<ReportCardGradesComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ReportCardGradesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportCardGradesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
