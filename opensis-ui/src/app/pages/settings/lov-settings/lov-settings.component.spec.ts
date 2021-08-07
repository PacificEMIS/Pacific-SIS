import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LovSettingsComponent } from './lov-settings.component';

describe('LovSettingsComponent', () => {
  let component: LovSettingsComponent;
  let fixture: ComponentFixture<LovSettingsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LovSettingsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LovSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
