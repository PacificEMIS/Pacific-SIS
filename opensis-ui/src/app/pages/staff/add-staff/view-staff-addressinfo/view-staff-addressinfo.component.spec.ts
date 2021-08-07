import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewStaffAddressinfoComponent } from './view-staff-addressinfo.component';

describe('ViewStaffAddressinfoComponent', () => {
  let component: ViewStaffAddressinfoComponent;
  let fixture: ComponentFixture<ViewStaffAddressinfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ViewStaffAddressinfoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewStaffAddressinfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
