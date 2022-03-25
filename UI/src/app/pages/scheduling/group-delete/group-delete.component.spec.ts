import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupDeleteComponent } from './group-delete.component';

describe('GroupDeleteComponent', () => {
  let component: GroupDeleteComponent;
  let fixture: ComponentFixture<GroupDeleteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GroupDeleteComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GroupDeleteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
