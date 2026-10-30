import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { StaffSchoolinfoComponent } from './staff-schoolinfo.component';

describe('StaffSchoolinfoComponent', () => {
  let component: StaffSchoolinfoComponent;
  let fixture: ComponentFixture<StaffSchoolinfoComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ StaffSchoolinfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StaffSchoolinfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
