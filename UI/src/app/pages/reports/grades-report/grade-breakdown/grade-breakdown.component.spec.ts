import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GradeBreakdownComponent } from './grade-breakdown.component';

describe('GradeBreakdownComponent', () => {
  let component: GradeBreakdownComponent;
  let fixture: ComponentFixture<GradeBreakdownComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GradeBreakdownComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GradeBreakdownComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
