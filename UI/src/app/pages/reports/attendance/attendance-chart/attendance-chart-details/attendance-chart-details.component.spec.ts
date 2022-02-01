import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AttendanceChartDetailsComponent } from './attendance-chart-details.component';

describe('AttendanceChartDetailsComponent', () => {
  let component: AttendanceChartDetailsComponent;
  let fixture: ComponentFixture<AttendanceChartDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AttendanceChartDetailsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AttendanceChartDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
