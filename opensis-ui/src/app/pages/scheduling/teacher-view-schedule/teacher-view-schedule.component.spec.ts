import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TeacherViewScheduleComponent } from './teacher-view-schedule.component';

describe('TeacherViewScheduleComponent', () => {
  let component: TeacherViewScheduleComponent;
  let fixture: ComponentFixture<TeacherViewScheduleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TeacherViewScheduleComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TeacherViewScheduleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
