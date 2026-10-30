import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { EditParentComponent } from './edit-parent.component';

describe('EditParentComponent', () => {
  let component: EditParentComponent;
  let fixture: ComponentFixture<EditParentComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ EditParentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditParentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
