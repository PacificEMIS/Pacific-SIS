import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SchoolClassificationComponent } from './school-classification.component';

describe('SchoolClassificationComponent', () => {
  let component: SchoolClassificationComponent;
  let fixture: ComponentFixture<SchoolClassificationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SchoolClassificationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SchoolClassificationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
