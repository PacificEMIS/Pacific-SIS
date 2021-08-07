import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewAssignmentDetailsComponent } from './view-assignment-details.component';

describe('ViewAssignmentDetailsComponent', () => {
  let component: ViewAssignmentDetailsComponent;
  let fixture: ComponentFixture<ViewAssignmentDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ViewAssignmentDetailsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewAssignmentDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
