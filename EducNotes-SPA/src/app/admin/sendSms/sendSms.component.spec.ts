/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { SendSmsComponent } from './sendSms.component';

describe('SendSmsComponent', () => {
  let component: SendSmsComponent;
  let fixture: ComponentFixture<SendSmsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SendSmsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SendSmsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
