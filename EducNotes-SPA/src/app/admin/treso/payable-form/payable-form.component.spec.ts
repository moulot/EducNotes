/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { PayableFormComponent } from './payable-form.component';

describe('PayableFormComponent', () => {
  let component: PayableFormComponent;
  let fixture: ComponentFixture<PayableFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PayableFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PayableFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
