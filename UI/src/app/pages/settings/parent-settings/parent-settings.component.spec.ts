import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ParentSettingsComponent } from './parent-settings.component';

describe('ParentSettingsComponent', () => {
  let component: ParentSettingsComponent;
  let fixture: ComponentFixture<ParentSettingsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ParentSettingsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ParentSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
