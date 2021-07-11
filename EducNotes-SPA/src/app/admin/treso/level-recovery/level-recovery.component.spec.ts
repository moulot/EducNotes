import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LevelRecoveryComponent } from './level-recovery.component';

describe('LevelRecoveryComponent', () => {
  let component: LevelRecoveryComponent;
  let fixture: ComponentFixture<LevelRecoveryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LevelRecoveryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LevelRecoveryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
