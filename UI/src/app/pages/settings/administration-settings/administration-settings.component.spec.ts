import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdministrationSettingsComponent } from './administration-settings.component';

describe('AdministrationSettingsComponent', () => {
  let component: AdministrationSettingsComponent;
  let fixture: ComponentFixture<AdministrationSettingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AdministrationSettingsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AdministrationSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
