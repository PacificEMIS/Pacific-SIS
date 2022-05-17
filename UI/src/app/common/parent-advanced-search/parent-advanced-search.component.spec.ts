import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ParentAdvancedSearchComponent } from './parent-advanced-search.component';

describe('ParentAdvancedSearchComponent', () => {
  let component: ParentAdvancedSearchComponent;
  let fixture: ComponentFixture<ParentAdvancedSearchComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ParentAdvancedSearchComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ParentAdvancedSearchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
