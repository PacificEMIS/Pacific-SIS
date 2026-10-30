import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { EditSchoolFieldsComponent } from './edit-school-fields.component';

describe('EditSchoolFieldsComponent', () => {
  let component: EditSchoolFieldsComponent;
  let fixture: ComponentFixture<EditSchoolFieldsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ EditSchoolFieldsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditSchoolFieldsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
