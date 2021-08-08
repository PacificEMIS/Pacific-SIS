import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffLogininfoComponent } from './staff-logininfo.component';

describe('StaffLogininfoComponent', () => {
  let component: StaffLogininfoComponent;
  let fixture: ComponentFixture<StaffLogininfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StaffLogininfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StaffLogininfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
