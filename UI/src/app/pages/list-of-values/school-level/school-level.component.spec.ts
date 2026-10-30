import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { SchoolLevelComponent } from './school-level.component';

describe('SchoolLevelComponent', () => {
  let component: SchoolLevelComponent;
  let fixture: ComponentFixture<SchoolLevelComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ SchoolLevelComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SchoolLevelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
