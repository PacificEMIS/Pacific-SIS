import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ViewstaffCertificationinfoComponent } from './viewstaff-certificationinfo.component';

describe('ViewstaffCertificationinfoComponent', () => {
  let component: ViewstaffCertificationinfoComponent;
  let fixture: ComponentFixture<ViewstaffCertificationinfoComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ViewstaffCertificationinfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewstaffCertificationinfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
