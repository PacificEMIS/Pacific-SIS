import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SchoolSpecificStandardsComponent } from './school-specific-standards.component';

describe('SchoolSpecificStandardsComponent', () => {
  let component: SchoolSpecificStandardsComponent;
  let fixture: ComponentFixture<SchoolSpecificStandardsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SchoolSpecificStandardsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SchoolSpecificStandardsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
