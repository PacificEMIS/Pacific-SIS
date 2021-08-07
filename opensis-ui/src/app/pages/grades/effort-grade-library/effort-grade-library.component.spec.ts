import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EffortGradeLibraryComponent } from './effort-grade-library.component';

describe('EffortGradeLibraryComponent', () => {
  let component: EffortGradeLibraryComponent;
  let fixture: ComponentFixture<EffortGradeLibraryComponent>;

  beforeEach(async(() => {
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
