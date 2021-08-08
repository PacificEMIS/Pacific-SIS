import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentSettingsComponent } from './student-settings.component';

describe('StudentSettingsComponent', () => {
  let component: StudentSettingsComponent;
  let fixture: ComponentFixture<StudentSettingsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StudentSettingsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StudentSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
