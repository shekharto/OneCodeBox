import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GenerateZipFileComponent } from './generate-zip-file.component';

describe('GenerateZipFileComponent', () => {
  let component: GenerateZipFileComponent;
  let fixture: ComponentFixture<GenerateZipFileComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ GenerateZipFileComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(GenerateZipFileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
