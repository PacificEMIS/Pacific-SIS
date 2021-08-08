import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NoticeCardsComponent } from './notice-cards.component';

describe('NoticeCardsComponent', () => {
  let component: NoticeCardsComponent;
  let fixture: ComponentFixture<NoticeCardsComponent>;

  beforeEach(async(() => {
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
