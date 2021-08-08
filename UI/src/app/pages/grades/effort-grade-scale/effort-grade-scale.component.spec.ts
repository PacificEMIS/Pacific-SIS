import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EffortGradeScaleComponent } from './effort-grade-scale.component';

describe('EffortGradeScaleComponent', () => {
  let component: EffortGradeScaleComponent;
  let fixture: ComponentFixture<EffortGradeScaleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EffortGradeScaleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EffortGradeScaleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
