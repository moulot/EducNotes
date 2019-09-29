/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { BooknoteComponent } from './booknote.component';

describe('BooknoteComponent', () => {
  let component: BooknoteComponent;
  let fixture: ComponentFixture<BooknoteComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BooknoteComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BooknoteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
