import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffCertificationinfoComponent } from './staff-certificationinfo.component';

describe('StaffCertificationinfoComponent', () => {
  let component: StaffCertificationinfoComponent;
  let fixture: ComponentFixture<StaffCertificationinfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StaffCertificationinfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StaffCertificationinfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
