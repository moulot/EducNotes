/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from 'src/app/nav/SideBar/node_modules/@angular/core/testing';
import { By } from 'src/app/nav/SideBar/node_modules/@angular/platform-browser';
import { DebugElement } from 'src/app/nav/SideBar/node_modules/@angular/core';

import { SideBarComponent } from './sideBar.component';

describe('SideBarComponent', () => {
  let component: SideBarComponent;
  let fixture: ComponentFixture<SideBarComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SideBarComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SideBarComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
