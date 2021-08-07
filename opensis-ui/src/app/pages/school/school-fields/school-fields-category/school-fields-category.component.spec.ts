import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SchoolFieldsCategoryComponent } from './school-fields-category.component';

describe('SchoolFieldsCategoryComponent', () => {
  let component: SchoolFieldsCategoryComponent;
  let fixture: ComponentFixture<SchoolFieldsCategoryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SchoolFieldsCategoryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SchoolFieldsCategoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
