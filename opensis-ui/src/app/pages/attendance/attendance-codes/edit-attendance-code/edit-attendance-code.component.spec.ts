import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditAttendanceCodeComponent } from './edit-attendance-code.component';

describe('EditAttendanceCodeComponent', () => {
  let component: EditAttendanceCodeComponent;
  let fixture: ComponentFixture<EditAttendanceCodeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditAttendanceCodeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditAttendanceCodeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
