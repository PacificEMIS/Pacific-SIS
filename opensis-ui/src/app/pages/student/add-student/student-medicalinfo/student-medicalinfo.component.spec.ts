import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentMedicalinfoComponent } from './student-medicalinfo.component';

describe('StudentMedicalinfoComponent', () => {
  let component: StudentMedicalinfoComponent;
  let fixture: ComponentFixture<StudentMedicalinfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StudentMedicalinfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentMedicalinfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
