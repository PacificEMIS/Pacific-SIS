import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AttendanceSettingsComponent } from './attendance-settings.component';

describe('AttendanceSettingsComponent', () => {
  let component: AttendanceSettingsComponent;
  let fixture: ComponentFixture<AttendanceSettingsComponent>;

  beforeEach(async(() => {
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
