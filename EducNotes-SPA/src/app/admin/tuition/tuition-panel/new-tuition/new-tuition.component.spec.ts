/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { NewTuitionComponent } from './new-tuition.component';

describe('NewTuitionComponent', () => {
  let component: NewTuitionComponent;
  let fixture: ComponentFixture<NewTuitionComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NewTuitionComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NewTuitionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
