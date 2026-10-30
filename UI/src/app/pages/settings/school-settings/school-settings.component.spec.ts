import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { SchoolSettingsComponent } from './school-settings.component';

describe('SchoolSettingsComponent', () => {
  let component: SchoolSettingsComponent;
  let fixture: ComponentFixture<SchoolSettingsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ SchoolSettingsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SchoolSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
