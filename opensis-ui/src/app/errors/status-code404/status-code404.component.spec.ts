import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StatusCode404Component } from './status-code404.component';

describe('StatusCode404Component', () => {
  let component: StatusCode404Component;
  let fixture: ComponentFixture<StatusCode404Component>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StatusCode404Component ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StatusCode404Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
