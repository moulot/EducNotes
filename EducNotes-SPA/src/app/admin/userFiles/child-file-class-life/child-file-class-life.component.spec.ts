import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ChildFileClassLifeComponent } from './child-file-class-life.component';

describe('ChildFileClassLifeComponent', () => {
  let component: ChildFileClassLifeComponent;
  let fixture: ComponentFixture<ChildFileClassLifeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChildFileClassLifeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChildFileClassLifeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
