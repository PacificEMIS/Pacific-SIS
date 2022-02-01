import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RolloverComponent } from './rollover.component';

describe('RolloverComponent', () => {
  let component: RolloverComponent;
  let fixture: ComponentFixture<RolloverComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ RolloverComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RolloverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
