import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CopyAssignmentComponent } from './copy-assignment.component';

describe('CopyAssignmentComponent', () => {
  let component: CopyAssignmentComponent;
  let fixture: ComponentFixture<CopyAssignmentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CopyAssignmentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CopyAssignmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
