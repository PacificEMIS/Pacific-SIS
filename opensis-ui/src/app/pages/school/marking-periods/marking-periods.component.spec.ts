import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MarkingPeriodsComponent } from './marking-periods.component';

describe('MarkingPeriodsComponent', () => {
  let component: MarkingPeriodsComponent;
  let fixture: ComponentFixture<MarkingPeriodsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MarkingPeriodsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MarkingPeriodsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
