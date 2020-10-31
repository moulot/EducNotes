import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ChildFileTuitionComponent } from './child-file-tuition.component';

describe('UserFileTuitionComponent', () => {
  let component: ChildFileTuitionComponent;
  let fixture: ComponentFixture<ChildFileTuitionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChildFileTuitionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChildFileTuitionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
