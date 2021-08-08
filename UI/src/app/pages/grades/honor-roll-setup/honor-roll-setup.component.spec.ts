import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HonorRollSetupComponent } from './honor-roll-setup.component';

describe('HonorRollSetupComponent', () => {
  let component: HonorRollSetupComponent;
  let fixture: ComponentFixture<HonorRollSetupComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ HonorRollSetupComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(HonorRollSetupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
