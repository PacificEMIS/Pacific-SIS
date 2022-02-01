import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InstituteReportComponent } from './institute-report.component';

describe('InstituteReportComponent', () => {
  let component: InstituteReportComponent;
  let fixture: ComponentFixture<InstituteReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ InstituteReportComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InstituteReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
