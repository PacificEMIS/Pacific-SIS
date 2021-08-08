import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentFamilyinfoComponent } from './student-familyinfo.component';

describe('StudentFamilyinfoComponent', () => {
  let component: StudentFamilyinfoComponent;
  let fixture: ComponentFixture<StudentFamilyinfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StudentFamilyinfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentFamilyinfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
