import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditReportCardGradesDetailsComponent } from './edit-report-card-grades-details.component';

describe('EditReportCardGradesDetailsComponent', () => {
  let component: EditReportCardGradesDetailsComponent;
  let fixture: ComponentFixture<EditReportCardGradesDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditReportCardGradesDetailsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EditReportCardGradesDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
