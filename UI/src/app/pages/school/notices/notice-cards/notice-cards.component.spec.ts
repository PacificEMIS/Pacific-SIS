import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { NoticeCardsComponent } from './notice-cards.component';

describe('NoticeCardsComponent', () => {
  let component: NoticeCardsComponent;
  let fixture: ComponentFixture<NoticeCardsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ NoticeCardsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NoticeCardsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
