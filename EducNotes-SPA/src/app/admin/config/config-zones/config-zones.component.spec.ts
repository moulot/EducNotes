import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfigZonesComponent } from './config-zones.component';

describe('ConfigZonesComponent', () => {
  let component: ConfigZonesComponent;
  let fixture: ComponentFixture<ConfigZonesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfigZonesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfigZonesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
