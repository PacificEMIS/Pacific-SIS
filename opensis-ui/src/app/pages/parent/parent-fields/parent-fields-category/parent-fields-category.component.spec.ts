import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ParentFieldsCategoryComponent } from './parent-fields-category.component';

describe('ParentFieldsCategoryComponent', () => {
  let component: ParentFieldsCategoryComponent;
  let fixture: ComponentFixture<ParentFieldsCategoryComponent>;

  beforeEach(async(() => {
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
