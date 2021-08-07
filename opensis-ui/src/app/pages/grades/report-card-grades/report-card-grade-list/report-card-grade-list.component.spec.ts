import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportCardGradeListComponent } from './report-card-grade-list.component';

describe('PeriodsListComponent', () => {
  let component: ReportCardGradeListComponent;
  let fixture: ComponentFixture<ReportCardGradeListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReportCardGradeListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportCardGradeListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
