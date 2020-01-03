/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { CallSheetCardComponent } from './callSheet-card.component';

describe('CallSheetCardComponent', () => {
  let component: CallSheetCardComponent;
  let fixture: ComponentFixture<CallSheetCardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CallSheetCardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CallSheetCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
