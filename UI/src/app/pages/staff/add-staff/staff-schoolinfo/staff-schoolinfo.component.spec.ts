import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffSchoolinfoComponent } from './staff-schoolinfo.component';

describe('StaffSchoolinfoComponent', () => {
  let component: StaffSchoolinfoComponent;
  let fixture: ComponentFixture<StaffSchoolinfoComponent>;

  beforeEach(async(() => {
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
