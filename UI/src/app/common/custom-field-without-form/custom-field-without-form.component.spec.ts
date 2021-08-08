import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomFieldWithoutFormComponent } from './custom-field-without-form.component';

describe('CustomFieldWithoutFormComponent', () => {
  let component: CustomFieldWithoutFormComponent;
  let fixture: ComponentFixture<CustomFieldWithoutFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CustomFieldWithoutFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomFieldWithoutFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
