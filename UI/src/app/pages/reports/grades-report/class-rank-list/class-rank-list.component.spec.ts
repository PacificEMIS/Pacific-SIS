import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClassRankListComponent } from './class-rank-list.component';

describe('ClassRankListComponent', () => {
  let component: ClassRankListComponent;
  let fixture: ComponentFixture<ClassRankListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ClassRankListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ClassRankListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
