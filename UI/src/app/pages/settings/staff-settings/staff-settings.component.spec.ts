import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { StaffSettingsComponent } from './staff-settings.component';

describe('StaffSettingsComponent', () => {
  let component: StaffSettingsComponent;
  let fixture: ComponentFixture<StaffSettingsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ StaffSettingsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StaffSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
