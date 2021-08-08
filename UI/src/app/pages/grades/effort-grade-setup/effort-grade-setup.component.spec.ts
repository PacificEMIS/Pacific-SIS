import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EffortGradeSetupComponent } from './effort-grade-setup.component';

describe('EffortGradeSetupComponent', () => {
  let component: EffortGradeSetupComponent;
  let fixture: ComponentFixture<EffortGradeSetupComponent>;

  beforeEach(async(() => {
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
