import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AttendanceChartComponent } from './attendance-chart.component';

describe('AttendanceChartComponent', () => {
  let component: AttendanceChartComponent;
  let fixture: ComponentFixture<AttendanceChartComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AttendanceChartComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AttendanceChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
