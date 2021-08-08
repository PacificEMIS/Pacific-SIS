import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditParentComponent } from './edit-parent.component';

describe('EditParentComponent', () => {
  let component: EditParentComponent;
  let fixture: ComponentFixture<EditParentComponent>;

  beforeEach(async(() => {
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
