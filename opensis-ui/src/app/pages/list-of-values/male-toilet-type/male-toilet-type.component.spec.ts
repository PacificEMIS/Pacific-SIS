import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MaleToiletTypeComponent } from './male-toilet-type.component';

describe('MaleToiletTypeComponent', () => {
  let component: MaleToiletTypeComponent;
  let fixture: ComponentFixture<MaleToiletTypeComponent>;

  beforeEach(async(() => {
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
