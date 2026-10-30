import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { StudentFieldsComponent } from './student-fields.component';

describe('StudentFieldsComponent', () => {
  let component: StudentFieldsComponent;
  let fixture: ComponentFixture<StudentFieldsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ StudentFieldsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentFieldsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
