import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ApptUpdateComponent } from './appt-update.component';

describe('ApptUpdateComponent', () => {
  let component: ApptUpdateComponent;
  let fixture: ComponentFixture<ApptUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ApptUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ApptUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
