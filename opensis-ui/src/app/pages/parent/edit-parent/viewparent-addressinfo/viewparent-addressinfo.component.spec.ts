import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewparentAddressinfoComponent } from './viewparent-addressinfo.component';

describe('ViewparentAddressinfoComponent', () => {
  let component: ViewparentAddressinfoComponent;
  let fixture: ComponentFixture<ViewparentAddressinfoComponent>;

  beforeEach(async(() => {
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
