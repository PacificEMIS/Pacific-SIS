import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { StaffFieldsComponent } from './staff-fields.component';

describe('StaffFieldsComponent', () => {
  let component: StaffFieldsComponent;
  let fixture: ComponentFixture<StaffFieldsComponent>;

  beforeEach(waitForAsync(() => {
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
