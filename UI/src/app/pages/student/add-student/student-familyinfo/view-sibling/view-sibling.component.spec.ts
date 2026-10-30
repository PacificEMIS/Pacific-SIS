import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ViewSiblingComponent } from './view-sibling.component';

describe('ViewSiblingComponent', () => {
  let component: ViewSiblingComponent;
  let fixture: ComponentFixture<ViewSiblingComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ViewSiblingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewSiblingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
