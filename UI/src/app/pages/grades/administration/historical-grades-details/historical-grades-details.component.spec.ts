import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HistoricalGradesDetailsComponent } from './historical-grades-details.component';

describe('HistoricalGradesDetailsComponent', () => {
  let component: HistoricalGradesDetailsComponent;
  let fixture: ComponentFixture<HistoricalGradesDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ HistoricalGradesDetailsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(HistoricalGradesDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
