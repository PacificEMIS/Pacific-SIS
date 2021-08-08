import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditparentAddressinfoComponent } from './editparent-addressinfo.component';

describe('EditparentAddressinfoComponent', () => {
  let component: EditparentAddressinfoComponent;
  let fixture: ComponentFixture<EditparentAddressinfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditparentAddressinfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditparentAddressinfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
