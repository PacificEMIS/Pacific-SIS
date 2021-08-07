import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentEnrollmentinfoComponent } from './student-enrollmentinfo.component';

describe('StudentEnrollmentinfoComponent', () => {
  let component: StudentEnrollmentinfoComponent;
  let fixture: ComponentFixture<StudentEnrollmentinfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StudentEnrollmentinfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentEnrollmentinfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
