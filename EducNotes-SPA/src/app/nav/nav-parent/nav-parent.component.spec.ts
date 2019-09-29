/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { NavParentComponent } from './nav-parent.component';

describe('NavParentComponent', () => {
  let component: NavParentComponent;
  let fixture: ComponentFixture<NavParentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NavParentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NavParentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
