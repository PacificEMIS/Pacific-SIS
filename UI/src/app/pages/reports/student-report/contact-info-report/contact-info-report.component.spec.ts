import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ContactInfoReportComponent } from './contact-info-report.component';

describe('ContactInfoReportComponent', () => {
  let component: ContactInfoReportComponent;
  let fixture: ComponentFixture<ContactInfoReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ContactInfoReportComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ContactInfoReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
