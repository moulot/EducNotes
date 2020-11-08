import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserValidationComponent } from './user-validation.component';

describe('UserValidationComponent', () => {
  let component: UserValidationComponent;
  let fixture: ComponentFixture<UserValidationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserValidationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserValidationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
