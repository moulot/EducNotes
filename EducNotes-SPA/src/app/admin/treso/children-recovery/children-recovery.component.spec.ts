import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ChildrenRecoveryComponent } from './children-recovery.component';

describe('ChildrenRecoveryComponent', () => {
  let component: ChildrenRecoveryComponent;
  let fixture: ComponentFixture<ChildrenRecoveryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChildrenRecoveryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChildrenRecoveryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
