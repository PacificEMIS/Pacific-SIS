import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddDropReportComponent } from './add-drop-report.component';

describe('AddDropReportComponent', () => {
  let component: AddDropReportComponent;
  let fixture: ComponentFixture<AddDropReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddDropReportComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddDropReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
