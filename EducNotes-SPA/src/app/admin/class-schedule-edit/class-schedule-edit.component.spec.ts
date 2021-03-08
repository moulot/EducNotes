import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ClassScheduleEditComponent } from './class-schedule-edit.component';

describe('ClassScheduleEditComponent', () => {
  let component: ClassScheduleEditComponent;
  let fixture: ComponentFixture<ClassScheduleEditComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ClassScheduleEditComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ClassScheduleEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
