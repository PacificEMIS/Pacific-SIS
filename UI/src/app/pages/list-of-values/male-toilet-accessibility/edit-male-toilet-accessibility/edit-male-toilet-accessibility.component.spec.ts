import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditMaleToiletAccessibilityComponent } from './edit-male-toilet-accessibility.component';

describe('EditMaleToiletAccessibilityComponent', () => {
  let component: EditMaleToiletAccessibilityComponent;
  let fixture: ComponentFixture<EditMaleToiletAccessibilityComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditMaleToiletAccessibilityComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EditMaleToiletAccessibilityComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
