import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfirmTeacherEmailComponent } from './confirm-teacher-email.component';

describe('ConfirmTeacherEmailComponent', () => {
  let component: ConfirmTeacherEmailComponent;
  let fixture: ComponentFixture<ConfirmTeacherEmailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfirmTeacherEmailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfirmTeacherEmailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
