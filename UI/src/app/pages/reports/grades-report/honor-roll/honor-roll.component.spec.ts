import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HonorRollComponent } from './honor-roll.component';

describe('HonorRollComponent', () => {
  let component: HonorRollComponent;
  let fixture: ComponentFixture<HonorRollComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ HonorRollComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(HonorRollComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
