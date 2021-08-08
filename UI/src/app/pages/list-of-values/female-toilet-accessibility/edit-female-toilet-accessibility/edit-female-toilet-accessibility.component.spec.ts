import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditFemaleToiletAccessibilityComponent } from './edit-female-toilet-accessibility.component';

describe('EditFemaleToiletAccessibilityComponent', () => {
  let component: EditFemaleToiletAccessibilityComponent;
  let fixture: ComponentFixture<EditFemaleToiletAccessibilityComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditFemaleToiletAccessibilityComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EditFemaleToiletAccessibilityComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
