import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DuedateDetailsComponent } from './duedate-details.component';

describe('DuedateDetailsComponent', () => {
  let component: DuedateDetailsComponent;
  let fixture: ComponentFixture<DuedateDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DuedateDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DuedateDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
