import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffAddressinfoComponent } from './staff-addressinfo.component';

describe('StaffAddressinfoComponent', () => {
  let component: StaffAddressinfoComponent;
  let fixture: ComponentFixture<StaffAddressinfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StaffAddressinfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StaffAddressinfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
