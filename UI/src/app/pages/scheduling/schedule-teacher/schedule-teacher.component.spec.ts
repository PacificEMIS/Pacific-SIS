import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScheduleTeacherComponent } from './schedule-teacher.component';

describe('ScheduleTeacherComponent', () => {
  let component: ScheduleTeacherComponent;
  let fixture: ComponentFixture<ScheduleTeacherComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ScheduleTeacherComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ScheduleTeacherComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
