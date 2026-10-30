import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { EditStudentFieldsComponent } from './edit-student-fields.component';

describe('EditStudentFieldsComponent', () => {
  let component: EditStudentFieldsComponent;
  let fixture: ComponentFixture<EditStudentFieldsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ EditStudentFieldsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditStudentFieldsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
