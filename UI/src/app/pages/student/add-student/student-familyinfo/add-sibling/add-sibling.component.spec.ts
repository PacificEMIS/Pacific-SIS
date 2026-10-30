import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AddSiblingComponent } from './add-sibling.component';

describe('AddSiblingComponent', () => {
  let component: AddSiblingComponent;
  let fixture: ComponentFixture<AddSiblingComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ AddSiblingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddSiblingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
