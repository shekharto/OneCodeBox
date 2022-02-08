import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MicroserviceCurdProductComponent } from './microservice-curd-product.component';

describe('MicroserviceCurdProductComponent', () => {
  let component: MicroserviceCurdProductComponent;
  let fixture: ComponentFixture<MicroserviceCurdProductComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MicroserviceCurdProductComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MicroserviceCurdProductComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
