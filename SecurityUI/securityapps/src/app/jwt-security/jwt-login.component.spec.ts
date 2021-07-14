import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JwtLoginComponent } from './jwt-login.component.ts';

describe('JwtSecurityComponent', () => {
  let component: JwtLoginComponent;
  let fixture: ComponentFixture<JwtLoginComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ JwtLoginComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(JwtLoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
