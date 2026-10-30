import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { SiblingsinfoComponent } from './siblingsinfo.component';

describe('SiblingsinfoComponent', () => {
  let component: SiblingsinfoComponent;
  let fixture: ComponentFixture<SiblingsinfoComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ SiblingsinfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SiblingsinfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
