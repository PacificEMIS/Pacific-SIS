import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StandardGradeSetupComponent } from './standard-grade-setup.component';

describe('StandardGradeSetupComponent', () => {
  let component: StandardGradeSetupComponent;
  let fixture: ComponentFixture<StandardGradeSetupComponent>;

  beforeEach(async(() => {
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
