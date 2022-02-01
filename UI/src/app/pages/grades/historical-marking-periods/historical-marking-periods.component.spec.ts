import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HistoricalMarkingPeriodsComponent } from './historical-marking-periods.component';

describe('HistoricalMarkingPeriodsComponent', () => {
  let component: HistoricalMarkingPeriodsComponent;
  let fixture: ComponentFixture<HistoricalMarkingPeriodsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ HistoricalMarkingPeriodsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(HistoricalMarkingPeriodsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
