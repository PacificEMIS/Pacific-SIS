import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffFieldsComponent } from './staff-fields.component';

describe('StaffFieldsComponent', () => {
  let component: StaffFieldsComponent;
  let fixture: ComponentFixture<StaffFieldsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StaffFieldsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StaffFieldsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
