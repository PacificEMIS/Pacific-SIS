import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SchoolLevelComponent } from './school-level.component';

describe('SchoolLevelComponent', () => {
  let component: SchoolLevelComponent;
  let fixture: ComponentFixture<SchoolLevelComponent>;

  beforeEach(async(() => {
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
