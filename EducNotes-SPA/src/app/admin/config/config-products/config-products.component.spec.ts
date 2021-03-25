import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfigProductsComponent } from './config-products.component';

describe('ConfigProductsComponent', () => {
  let component: ConfigProductsComponent;
  let fixture: ComponentFixture<ConfigProductsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfigProductsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfigProductsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
