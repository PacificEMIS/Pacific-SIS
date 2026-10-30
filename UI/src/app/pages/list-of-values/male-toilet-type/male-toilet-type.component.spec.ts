import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { MaleToiletTypeComponent } from './male-toilet-type.component';

describe('MaleToiletTypeComponent', () => {
  let component: MaleToiletTypeComponent;
  let fixture: ComponentFixture<MaleToiletTypeComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ MaleToiletTypeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MaleToiletTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
