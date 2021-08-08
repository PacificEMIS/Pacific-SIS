import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ParentFieldsComponent } from './parent-fields.component';

describe('ParentFieldsComponent', () => {
  let component: ParentFieldsComponent;
  let fixture: ComponentFixture<ParentFieldsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ParentFieldsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ParentFieldsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
