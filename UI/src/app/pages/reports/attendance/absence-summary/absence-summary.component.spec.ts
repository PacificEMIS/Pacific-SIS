import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AbsenceSummaryComponent } from './absence-summary.component';

describe('AbsenceSummaryComponent', () => {
  let component: AbsenceSummaryComponent;
  let fixture: ComponentFixture<AbsenceSummaryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AbsenceSummaryComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AbsenceSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
