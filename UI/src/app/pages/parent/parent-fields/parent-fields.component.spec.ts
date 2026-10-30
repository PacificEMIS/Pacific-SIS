import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ParentFieldsComponent } from './parent-fields.component';

describe('ParentFieldsComponent', () => {
  let component: ParentFieldsComponent;
  let fixture: ComponentFixture<ParentFieldsComponent>;

  beforeEach(waitForAsync(() => {
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
