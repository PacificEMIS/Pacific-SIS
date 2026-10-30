import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { NoticesComponent } from './notices.component';

describe('NoticesComponent', () => {
  let component: NoticesComponent;
  let fixture: ComponentFixture<NoticesComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ NoticesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NoticesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
