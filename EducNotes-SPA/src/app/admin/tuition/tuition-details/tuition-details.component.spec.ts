import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TuitionDetailsComponent } from './tuition-details.component';

describe('TuitionDetailsComponent', () => {
  let component: TuitionDetailsComponent;
  let fixture: ComponentFixture<TuitionDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TuitionDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TuitionDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
