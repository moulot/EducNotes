import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ScheduleAddConflictComponent } from './schedule-add-conflict.component';

describe('ScheduleAddConflictComponent', () => {
  let component: ScheduleAddConflictComponent;
  let fixture: ComponentFixture<ScheduleAddConflictComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ScheduleAddConflictComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ScheduleAddConflictComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
