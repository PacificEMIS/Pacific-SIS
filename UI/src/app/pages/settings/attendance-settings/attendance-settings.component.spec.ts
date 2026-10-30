import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AttendanceSettingsComponent } from './attendance-settings.component';

describe('AttendanceSettingsComponent', () => {
  let component: AttendanceSettingsComponent;
  let fixture: ComponentFixture<AttendanceSettingsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ AttendanceSettingsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AttendanceSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
