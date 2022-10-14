import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SchoolwideScheduleReportComponent } from './schoolwide-schedule-report.component';

describe('SchoolwideScheduleReportComponent', () => {
  let component: SchoolwideScheduleReportComponent;
  let fixture: ComponentFixture<SchoolwideScheduleReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SchoolwideScheduleReportComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SchoolwideScheduleReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
