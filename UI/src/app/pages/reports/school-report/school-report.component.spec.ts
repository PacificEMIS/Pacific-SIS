import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SchoolReportComponent } from './school-report.component';

describe('SchoolReportComponent', () => {
  let component: SchoolReportComponent;
  let fixture: ComponentFixture<SchoolReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SchoolReportComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SchoolReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
