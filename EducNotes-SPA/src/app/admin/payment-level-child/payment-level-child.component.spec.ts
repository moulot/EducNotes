import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PaymentLevelChildComponent } from './payment-level-child.component';

describe('PaymentLevelChildComponent', () => {
  let component: PaymentLevelChildComponent;
  let fixture: ComponentFixture<PaymentLevelChildComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PaymentLevelChildComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PaymentLevelChildComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
