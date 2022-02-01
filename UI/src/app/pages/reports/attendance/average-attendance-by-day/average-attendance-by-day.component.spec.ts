import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AverageAttendanceByDayComponent } from './average-attendance-by-day.component';

describe('AverageAttendanceByDayComponent', () => {
  let component: AverageAttendanceByDayComponent;
  let fixture: ComponentFixture<AverageAttendanceByDayComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AverageAttendanceByDayComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AverageAttendanceByDayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
