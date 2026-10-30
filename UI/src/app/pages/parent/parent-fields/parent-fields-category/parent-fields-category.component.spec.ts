import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ParentFieldsCategoryComponent } from './parent-fields-category.component';

describe('ParentFieldsCategoryComponent', () => {
  let component: ParentFieldsCategoryComponent;
  let fixture: ComponentFixture<ParentFieldsCategoryComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ParentFieldsCategoryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ParentFieldsCategoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
