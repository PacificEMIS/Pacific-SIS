import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GradebookConfigurationComponent } from './gradebook-configuration.component';

describe('GradebookConfigurationComponent', () => {
  let component: GradebookConfigurationComponent;
  let fixture: ComponentFixture<GradebookConfigurationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GradebookConfigurationComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GradebookConfigurationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
