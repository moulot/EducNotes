import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserFileTuitionComponent } from './user-file-tuition.component';

describe('UserFileTuitionComponent', () => {
  let component: UserFileTuitionComponent;
  let fixture: ComponentFixture<UserFileTuitionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserFileTuitionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserFileTuitionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
