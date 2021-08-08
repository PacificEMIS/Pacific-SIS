import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffCourseScheduleComponent } from './staff-course-schedule.component';

describe('StaffCourseScheduleComponent', () => {
  let component: StaffCourseScheduleComponent;
  let fixture: ComponentFixture<StaffCourseScheduleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StaffCourseScheduleComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StaffCourseScheduleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
