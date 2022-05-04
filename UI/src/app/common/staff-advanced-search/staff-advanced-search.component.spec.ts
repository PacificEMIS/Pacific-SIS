import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffAdvancedSearchComponent } from './staff-advanced-search.component';

describe('StaffAdvancedSearchComponent', () => {
  let component: StaffAdvancedSearchComponent;
  let fixture: ComponentFixture<StaffAdvancedSearchComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StaffAdvancedSearchComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StaffAdvancedSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
