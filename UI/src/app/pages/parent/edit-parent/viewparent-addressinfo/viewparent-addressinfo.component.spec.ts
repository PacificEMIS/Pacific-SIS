import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ViewparentAddressinfoComponent } from './viewparent-addressinfo.component';

describe('ViewparentAddressinfoComponent', () => {
  let component: ViewparentAddressinfoComponent;
  let fixture: ComponentFixture<ViewparentAddressinfoComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ViewparentAddressinfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewparentAddressinfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
