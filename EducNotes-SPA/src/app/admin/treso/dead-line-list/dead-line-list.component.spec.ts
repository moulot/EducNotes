/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { DeadLineListComponent } from './dead-line-list.component';

describe('DeadLineListComponent', () => {
  let component: DeadLineListComponent;
  let fixture: ComponentFixture<DeadLineListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DeadLineListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DeadLineListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
