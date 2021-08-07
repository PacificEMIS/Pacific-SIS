import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FemaleToiletAccessibilityComponent } from './female-toilet-accessibility.component';

describe('FemaleToiletAccessibilityComponent', () => {
  let component: FemaleToiletAccessibilityComponent;
  let fixture: ComponentFixture<FemaleToiletAccessibilityComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FemaleToiletAccessibilityComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FemaleToiletAccessibilityComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
