import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SchoolFieldsComponent } from './school-fields.component';

describe('SchoolFieldsComponent', () => {
  let component: SchoolFieldsComponent;
  let fixture: ComponentFixture<SchoolFieldsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SchoolFieldsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SchoolFieldsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
