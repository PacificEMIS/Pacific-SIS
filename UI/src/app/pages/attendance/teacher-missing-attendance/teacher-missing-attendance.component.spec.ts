import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TeacherMissingAttendanceComponent } from './teacher-missing-attendance.component';

describe('TeacherMissingAttendanceComponent', () => {
  let component: TeacherMissingAttendanceComponent;
  let fixture: ComponentFixture<TeacherMissingAttendanceComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TeacherMissingAttendanceComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TeacherMissingAttendanceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
