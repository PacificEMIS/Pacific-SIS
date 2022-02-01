import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdvanceReportComponent } from './advance-report.component';

describe('AdvanceReportComponent', () => {
  let component: AdvanceReportComponent;
  let fixture: ComponentFixture<AdvanceReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AdvanceReportComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AdvanceReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
