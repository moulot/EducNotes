import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RecoveryListComponent } from './recovery-list.component';

describe('RecoveryListComponent', () => {
  let component: RecoveryListComponent;
  let fixture: ComponentFixture<RecoveryListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RecoveryListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RecoveryListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
