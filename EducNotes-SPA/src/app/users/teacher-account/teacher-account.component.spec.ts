import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TeacherAccountComponent } from './teacher-account.component';

describe('TeacherAccountComponent', () => {
  let component: TeacherAccountComponent;
  let fixture: ComponentFixture<TeacherAccountComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TeacherAccountComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TeacherAccountComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
