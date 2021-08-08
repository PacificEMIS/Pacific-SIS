import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UsCommonCoreStandardsComponent } from './us-common-core-standards.component';

describe('UsCommonCoreStandardsComponent', () => {
  let component: UsCommonCoreStandardsComponent;
  let fixture: ComponentFixture<UsCommonCoreStandardsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UsCommonCoreStandardsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UsCommonCoreStandardsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
