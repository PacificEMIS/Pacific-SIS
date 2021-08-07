import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CommonToiletAccessibilityComponent } from './common-toilet-accessibility.component';

describe('CommonToiletAccessibilityComponent', () => {
  let component: CommonToiletAccessibilityComponent;
  let fixture: ComponentFixture<CommonToiletAccessibilityComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CommonToiletAccessibilityComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CommonToiletAccessibilityComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
