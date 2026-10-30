import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { EditSchoolSpecificStandardComponent } from './edit-school-specific-standard.component';

describe('EditSchoolSpecificStandardComponent', () => {
  let component: EditSchoolSpecificStandardComponent;
  let fixture: ComponentFixture<EditSchoolSpecificStandardComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ EditSchoolSpecificStandardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditSchoolSpecificStandardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
