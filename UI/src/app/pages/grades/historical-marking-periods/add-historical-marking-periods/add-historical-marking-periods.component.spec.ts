import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddHistoricalMarkingPeriodsComponent } from './add-historical-marking-periods.component';

describe('AddHistoricalMarkingPeriodsComponent', () => {
  let component: AddHistoricalMarkingPeriodsComponent;
  let fixture: ComponentFixture<AddHistoricalMarkingPeriodsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddHistoricalMarkingPeriodsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddHistoricalMarkingPeriodsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
