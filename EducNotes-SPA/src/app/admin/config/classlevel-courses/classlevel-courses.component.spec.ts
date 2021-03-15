import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ClasslevelCoursesComponent } from './classlevel-courses.component';

describe('ClasslevelCoursesComponent', () => {
  let component: ClasslevelCoursesComponent;
  let fixture: ComponentFixture<ClasslevelCoursesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ClasslevelCoursesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ClasslevelCoursesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
