import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SearchStudentComponentForEditReportCardGrades } from './search-student.component';

describe('SearchStudentComponent', () => {
  let component: SearchStudentComponentForEditReportCardGrades;
  let fixture: ComponentFixture<SearchStudentComponentForEditReportCardGrades>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SearchStudentComponentForEditReportCardGrades ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SearchStudentComponentForEditReportCardGrades);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
