import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditParentFieldsComponent } from './edit-parent-fields.component';

describe('EditParentFieldsComponent', () => {
  let component: EditParentFieldsComponent;
  let fixture: ComponentFixture<EditParentFieldsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditParentFieldsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditParentFieldsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
