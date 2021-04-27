import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfigCourseTypeComponent } from './config-course-type.component';

describe('ConfigCourseTypeComponent', () => {
  let component: ConfigCourseTypeComponent;
  let fixture: ComponentFixture<ConfigCourseTypeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfigCourseTypeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfigCourseTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
