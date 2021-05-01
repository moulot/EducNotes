import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TuitionDataComponent } from './tuition-data.component';

describe('TuitionDataComponent', () => {
  let component: TuitionDataComponent;
  let fixture: ComponentFixture<TuitionDataComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TuitionDataComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TuitionDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
