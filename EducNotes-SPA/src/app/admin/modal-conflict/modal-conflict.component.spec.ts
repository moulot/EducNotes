import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ModalConflictComponent } from './modal-conflict.component';

describe('ModalConflictComponent', () => {
  let component: ModalConflictComponent;
  let fixture: ComponentFixture<ModalConflictComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ModalConflictComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ModalConflictComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
