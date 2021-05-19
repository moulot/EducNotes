import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfigProductFeesComponent } from './config-product-fees.component';

describe('ConfigProductFeesComponent', () => {
  let component: ConfigProductFeesComponent;
  let fixture: ComponentFixture<ConfigProductFeesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfigProductFeesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfigProductFeesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
