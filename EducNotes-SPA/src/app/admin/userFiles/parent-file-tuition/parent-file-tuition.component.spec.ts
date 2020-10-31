import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ParentFileTuitionComponent } from './parent-file-tuition.component';

describe('ParentFileTuitionComponent', () => {
  let component: ParentFileTuitionComponent;
  let fixture: ComponentFixture<ParentFileTuitionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ParentFileTuitionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ParentFileTuitionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
