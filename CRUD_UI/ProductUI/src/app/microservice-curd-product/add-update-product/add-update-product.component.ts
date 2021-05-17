import {Component, Input, OnInit} from '@angular/core';
import {Location} from '@angular/common';
import {ActivatedRoute} from '@angular/router';

import {Product} from "../../shared/entities/product";
import {ProductRepository} from "../repositories/product.repository";
import {EventType, ObjectStateType} from "../../shared/entities/enums";
import {EventService} from '../../shared/service/event.service';

@Component({
  selector: 'app-add-update-product',
  templateUrl: './add-update-product.component.html',
  styleUrls: ['./add-update-product.component.scss']
})
export class AddUpdateProductComponent implements OnInit {
  product: Product;
  products: Product[] = [];
  selectedId: number;
  private _value: string;

  constructor (private location: Location,
               private route: ActivatedRoute,
               private productRepository: ProductRepository,
               private eventService: EventService) { }

  ngOnInit(): void {
    this.selectedId = 0;
    this.product = new Product();
    this.selectedId= Number(this.route.snapshot.paramMap.get('selectedProductId'))
    if (this.selectedId && this.selectedId > 0) {
      this.getProductDetails(this.selectedId);
    }
  }

  /* private / public methods */

  getProductDetails(id: number): void {
    this.productRepository.getProductById(id).then( data  => {
      this.product = data;
    });
  }

  saveProduct(): void {
    if (this.selectedId && this.selectedId > 0) {
      this.product.objectState = ObjectStateType.Modified;
      this.product.productId = this.selectedId;
    } else {
      this.product.objectState = ObjectStateType.Added;
    }
      this.productRepository.postProduct(this.product).then( () => {
      this.eventService.getNotifier<Product>(EventType.NewProductAdded).publish(this.product);
      this.goBack();
    });
  }

  goBack(): void {
    this.location.back();
  }

  /* getter and setter */

  @Input()
  get value(): string {
    return this._value ? this._value : "";
  }

  set value(value: string) {
    if (value !== this._value) {
      this._value = value;
    }
  }

}
