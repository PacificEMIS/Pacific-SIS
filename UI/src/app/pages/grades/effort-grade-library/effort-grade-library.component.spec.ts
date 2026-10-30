import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { EffortGradeLibraryComponent } from './effort-grade-library.component';

describe('EffortGradeLibraryComponent', () => {
  let component: EffortGradeLibraryComponent;
  let fixture: ComponentFixture<EffortGradeLibraryComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ EffortGradeLibraryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EffortGradeLibraryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
