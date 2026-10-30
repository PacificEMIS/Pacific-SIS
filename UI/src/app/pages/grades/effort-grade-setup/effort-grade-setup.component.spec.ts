import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { EffortGradeSetupComponent } from './effort-grade-setup.component';

describe('EffortGradeSetupComponent', () => {
  let component: EffortGradeSetupComponent;
  let fixture: ComponentFixture<EffortGradeSetupComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ EffortGradeSetupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EffortGradeSetupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
