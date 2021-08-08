import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MaleToiletAccessibilityComponent } from './male-toilet-accessibility.component';

describe('MaleToiletAccessibilityComponent', () => {
  let component: MaleToiletAccessibilityComponent;
  let fixture: ComponentFixture<MaleToiletAccessibilityComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MaleToiletAccessibilityComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MaleToiletAccessibilityComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
