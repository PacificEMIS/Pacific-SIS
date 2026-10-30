import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { SchoolFieldsComponent } from './school-fields.component';

describe('SchoolFieldsComponent', () => {
  let component: SchoolFieldsComponent;
  let fixture: ComponentFixture<SchoolFieldsComponent>;

  beforeEach(waitForAsync(() => {
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
