import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditCommonToiletAccessibilityComponent } from './edit-common-toilet-accessibility.component';

describe('EditCommonToiletAccessibilityComponent', () => {
  let component: EditCommonToiletAccessibilityComponent;
  let fixture: ComponentFixture<EditCommonToiletAccessibilityComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditCommonToiletAccessibilityComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EditCommonToiletAccessibilityComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
