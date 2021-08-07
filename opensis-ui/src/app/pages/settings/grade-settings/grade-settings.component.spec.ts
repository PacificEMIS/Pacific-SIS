import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { GradeSettingsComponent } from './grade-settings.component';

describe('GradeSettingsComponent', () => {
  let component: GradeSettingsComponent;
  let fixture: ComponentFixture<GradeSettingsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ GradeSettingsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GradeSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
