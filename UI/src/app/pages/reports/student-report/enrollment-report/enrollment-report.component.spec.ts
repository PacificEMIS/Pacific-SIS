import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EnrollmentReportComponent } from './enrollment-report.component';

describe('EnrollmentReportComponent', () => {
  let component: EnrollmentReportComponent;
  let fixture: ComponentFixture<EnrollmentReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EnrollmentReportComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EnrollmentReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
