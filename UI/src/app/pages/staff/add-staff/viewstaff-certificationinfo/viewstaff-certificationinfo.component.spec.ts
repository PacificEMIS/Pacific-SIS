import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewstaffCertificationinfoComponent } from './viewstaff-certificationinfo.component';

describe('ViewstaffCertificationinfoComponent', () => {
  let component: ViewstaffCertificationinfoComponent;
  let fixture: ComponentFixture<ViewstaffCertificationinfoComponent>;

  beforeEach(async(() => {
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
