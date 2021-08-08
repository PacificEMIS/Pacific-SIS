import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentLogininfoComponent } from './student-logininfo.component';

describe('StudentLogininfoComponent', () => {
  let component: StudentLogininfoComponent;
  let fixture: ComponentFixture<StudentLogininfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StudentLogininfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentLogininfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
