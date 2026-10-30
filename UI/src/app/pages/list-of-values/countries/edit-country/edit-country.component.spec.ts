import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { EditCountryComponent } from './edit-country.component';

describe('EditCountryComponent', () => {
  let component: EditCountryComponent;
  let fixture: ComponentFixture<EditCountryComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ EditCountryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditCountryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
