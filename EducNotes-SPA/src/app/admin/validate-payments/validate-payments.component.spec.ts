import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ValidatePaymentsComponent } from './validate-payments.component';

describe('ValidatePaymentsComponent', () => {
  let component: ValidatePaymentsComponent;
  let fixture: ComponentFixture<ValidatePaymentsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ValidatePaymentsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ValidatePaymentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
