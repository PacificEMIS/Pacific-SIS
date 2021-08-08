import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditEnrollmentCodeComponent } from './edit-enrollment-code.component';

describe('EditEnrollmentCodeComponent', () => {
  let component: EditEnrollmentCodeComponent;
  let fixture: ComponentFixture<EditEnrollmentCodeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditEnrollmentCodeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditEnrollmentCodeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
