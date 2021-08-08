import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditSchoolSpecificStandardComponent } from './edit-school-specific-standard.component';

describe('EditSchoolSpecificStandardComponent', () => {
  let component: EditSchoolSpecificStandardComponent;
  let fixture: ComponentFixture<EditSchoolSpecificStandardComponent>;

  beforeEach(async(() => {
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
