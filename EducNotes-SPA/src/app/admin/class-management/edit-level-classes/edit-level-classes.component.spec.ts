import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EditLevelClassesComponent } from './edit-level-classes.component';

describe('EditLevelClassesComponent', () => {
  let component: EditLevelClassesComponent;
  let fixture: ComponentFixture<EditLevelClassesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EditLevelClassesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EditLevelClassesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
