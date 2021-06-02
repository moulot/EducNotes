import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfigAddServiceComponent } from './config-add-service.component';

describe('ConfigAddServiceComponent', () => {
  let component: ConfigAddServiceComponent;
  let fixture: ComponentFixture<ConfigAddServiceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfigAddServiceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfigAddServiceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
