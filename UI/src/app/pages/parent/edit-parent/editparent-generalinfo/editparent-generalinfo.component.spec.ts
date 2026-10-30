import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { EditparentGeneralinfoComponent } from './editparent-generalinfo.component';

describe('EditparentGeneralinfoComponent', () => {
  let component: EditparentGeneralinfoComponent;
  let fixture: ComponentFixture<EditparentGeneralinfoComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ EditparentGeneralinfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditparentGeneralinfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
