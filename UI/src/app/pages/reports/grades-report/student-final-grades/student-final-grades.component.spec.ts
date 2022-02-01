import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentFinalGradesComponent } from './student-final-grades.component';

describe('StudentFinalGradesComponent', () => {
  let component: StudentFinalGradesComponent;
  let fixture: ComponentFixture<StudentFinalGradesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StudentFinalGradesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentFinalGradesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
