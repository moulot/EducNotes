/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { PayableAtsComponent } from './payableAts.component';

describe('PayableAtsComponent', () => {
  let component: PayableAtsComponent;
  let fixture: ComponentFixture<PayableAtsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PayableAtsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PayableAtsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
