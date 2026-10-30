import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { SchoolFieldsCategoryComponent } from './school-fields-category.component';

describe('SchoolFieldsCategoryComponent', () => {
  let component: SchoolFieldsCategoryComponent;
  let fixture: ComponentFixture<SchoolFieldsCategoryComponent>;

  beforeEach(waitForAsync(() => {
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
