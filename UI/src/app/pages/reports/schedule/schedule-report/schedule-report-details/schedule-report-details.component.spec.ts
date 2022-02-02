import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScheduleReportDetailsComponent } from './schedule-report-details.component';

describe('ScheduleReportDetailsComponent', () => {
  let component: ScheduleReportDetailsComponent;
  let fixture: ComponentFixture<ScheduleReportDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ScheduleReportDetailsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ScheduleReportDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
