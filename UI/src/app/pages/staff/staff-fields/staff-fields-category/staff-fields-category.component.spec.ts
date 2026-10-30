import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { StaffFieldsCategoryComponent } from './staff-fields-category.component';

describe('StaffFieldsCategoryComponent', () => {
  let component: StaffFieldsCategoryComponent;
  let fixture: ComponentFixture<StaffFieldsCategoryComponent>;

  beforeEach(waitForAsync(() => {
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
