import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditEffortGradeScaleComponent } from './edit-effort-grade-scale.component';

describe('EditEffortGradeScaleComponent', () => {
  let component: EditEffortGradeScaleComponent;
  let fixture: ComponentFixture<EditEffortGradeScaleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditEffortGradeScaleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditEffortGradeScaleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
