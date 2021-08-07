import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewGeneralInfoComponent } from './view-general-info.component';

describe('ViewGeneralInfoComponent', () => {
  let component: ViewGeneralInfoComponent;
  let fixture: ComponentFixture<ViewGeneralInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ViewGeneralInfoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewGeneralInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
