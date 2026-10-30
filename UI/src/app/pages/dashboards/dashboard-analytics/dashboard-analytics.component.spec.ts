import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { DashboardAnalyticsComponent } from './dashboard-analytics.component';

describe('DashboardAnalyticsComponent', () => {
  let component: DashboardAnalyticsComponent;
  let fixture: ComponentFixture<DashboardAnalyticsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [DashboardAnalyticsComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardAnalyticsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
