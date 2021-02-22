import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ParentAccountComponent } from './parent-account.component';

describe('ParentAccountComponent', () => {
  let component: ParentAccountComponent;
  let fixture: ComponentFixture<ParentAccountComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ParentAccountComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ParentAccountComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
