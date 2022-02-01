import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AverageDailyAttendanceComponent } from './average-daily-attendance.component';

describe('AverageDailyAttendanceComponent', () => {
  let component: AverageDailyAttendanceComponent;
  let fixture: ComponentFixture<AverageDailyAttendanceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AverageDailyAttendanceComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AverageDailyAttendanceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
