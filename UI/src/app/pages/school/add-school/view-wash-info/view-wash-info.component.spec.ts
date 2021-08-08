import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewWashInfoComponent } from './view-wash-info.component';

describe('ViewWashInfoComponent', () => {
  let component: ViewWashInfoComponent;
  let fixture: ComponentFixture<ViewWashInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ViewWashInfoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewWashInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
