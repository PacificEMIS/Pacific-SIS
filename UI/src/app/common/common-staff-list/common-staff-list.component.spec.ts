import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CommonStaffListComponent } from './common-staff-list.component';

describe('CommonStaffListComponent', () => {
  let component: CommonStaffListComponent;
  let fixture: ComponentFixture<CommonStaffListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CommonStaffListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CommonStaffListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
