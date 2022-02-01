import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AbsenceSummaryDetailsComponent } from './absence-summary-details.component';

describe('AbsenceSummaryDetailsComponent', () => {
  let component: AbsenceSummaryDetailsComponent;
  let fixture: ComponentFixture<AbsenceSummaryDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AbsenceSummaryDetailsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AbsenceSummaryDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
