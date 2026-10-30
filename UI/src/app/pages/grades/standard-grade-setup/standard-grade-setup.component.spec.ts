import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { StandardGradeSetupComponent } from './standard-grade-setup.component';

describe('StandardGradeSetupComponent', () => {
  let component: StandardGradeSetupComponent;
  let fixture: ComponentFixture<StandardGradeSetupComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ StandardGradeSetupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StandardGradeSetupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
