import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { CertificateDetailsComponent } from './certificate-details.component';

describe('CertificateDetailsComponent', () => {
  let component: CertificateDetailsComponent;
  let fixture: ComponentFixture<CertificateDetailsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ CertificateDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CertificateDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
