import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffFieldsCategoryComponent } from './staff-fields-category.component';

describe('StaffFieldsCategoryComponent', () => {
  let component: StaffFieldsCategoryComponent;
  let fixture: ComponentFixture<StaffFieldsCategoryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StaffFieldsCategoryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StaffFieldsCategoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
