/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { LevelClassesComponent } from './level-classes.component';

describe('LevelClassesComponent', () => {
  let component: LevelClassesComponent;
  let fixture: ComponentFixture<LevelClassesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LevelClassesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LevelClassesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
