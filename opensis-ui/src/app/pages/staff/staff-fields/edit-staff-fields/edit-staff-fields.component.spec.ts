import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditStaffFieldsComponent } from './edit-staff-fields.component';

describe('EditStaffFieldsComponent', () => {
  let component: EditStaffFieldsComponent;
  let fixture: ComponentFixture<EditStaffFieldsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditStaffFieldsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditStaffFieldsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
