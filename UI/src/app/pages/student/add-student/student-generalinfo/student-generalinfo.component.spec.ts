import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentGeneralinfoComponent } from './student-generalinfo.component';

describe('StudentGeneralinfoComponent', () => {
  let component: StudentGeneralinfoComponent;
  let fixture: ComponentFixture<StudentGeneralinfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StudentGeneralinfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentGeneralinfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
