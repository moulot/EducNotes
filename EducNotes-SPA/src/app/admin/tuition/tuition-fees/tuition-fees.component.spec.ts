import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TuitionFeesComponent } from './tuition-fees.component';

describe('TuitionFeesComponent', () => {
  let component: TuitionFeesComponent;
  let fixture: ComponentFixture<TuitionFeesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TuitionFeesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TuitionFeesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
