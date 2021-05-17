import {Component, ElementRef, Injector, Input, OnInit, ViewChild} from "@angular/core";
import {MatTableDataSource} from '@angular/material/table';
import {Sort} from '@angular/material/sort';
import {SelectionModel} from '@angular/cdk/collections';
import { Location } from '@angular/common';

import {ComponentBase} from './../shared/helpers/componentbase';
import {EventService} from './../shared/service/event.service';
import {Product} from "../shared/entities/product";
import {ProductRepository} from "./repositories/product.repository";
import { EventType } from '../shared/entities/enums';

@Component({
  selector: 'app-microservice-curd-product',
  templateUrl: './microservice-curd-product.component.html',
  styleUrls: ['./microservice-curd-product.component.scss']
})
export class MicroserviceCurdProductComponent extends ComponentBase implements OnInit {
  displayedColumns = ['select', 'name', 'description', 'active'];
  private _displayProducts: Product[] = [];
  selectedProductId: number;
  sortedData: any;
  selection = new SelectionModel<Product>(true, []);

  constructor(  self: ElementRef, injector: Injector,private productRepository: ProductRepository,
              private location: Location,
              private eventService: EventService) {
    super(self, injector);
   }

  ngOnInit() {
    this.loadProduct();
    this.subscribeEvents();
  }

  /* private / public methods */

  loadProduct(): void {
    this.productRepository.getProducts().then(
      data => {
        this.displayProducts =  data;
       }
    )
  }

  deleteProduct(): void {
  this.productRepository.deleteProduct(this.selectedProductId).then(
          result => {
            if (result) {
              this.loadProduct();
            } else {
              console.log("Add product failed.");
             }
          });
    }

  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.sortedData.data.length;
    return numSelected === numRows;
  }

  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggle() {
     this.isAllSelected() ?
      this.selection.clear() : this.displayProducts.forEach(item => {
        this.selection.select(item);
      });
     }

  /** The label for the checkbox on the passed row */
  checkboxLabel(row?: Product): string {
    if (!row) {
      return `${this.isAllSelected() ? 'select' : 'deselect'} all`;
    }
    if (this.selection.selected.length > 0) {
      this.selectedProductId = this.selection.selected[0].productId
    }
    return `${this.selection.isSelected(row) ? 'deselect' : 'select'} row ${row.name + 1}`;
  }

  private subscribeEvents(): void {
    this.registerSubscriber(this.eventService.getNotifier<Product>(EventType.NewProductAdded).subscribe(() => this.loadProduct()));
  }

    goBack(): void {
      this.location.back();
    }

  sortData(sort: Sort) {
    const data = this.displayProducts.slice();
    if (!sort.active || sort.direction === '') {
      this.sortedData = data;
      return;
    }

    this.sortedData = data.sort((a, b) => {
      const isAsc = sort.direction === 'asc';
      switch (sort.active) {
        case 'name': return this.compare(a.name, b.name, isAsc);
        case 'description': return this.compare(a.description, b.description, isAsc);
        case 'active': return this.compare(a.active, b.active, isAsc);
        default: return 0;
      }
    });
  }

  private compare(a: number | string | boolean, b: number | string | boolean, isAsc: boolean) {
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.sortedData.filter = filterValue.trim().toLowerCase();
  }

  clearData(event: Event) {
     this.sortedData.filter = "";
  }


  /* get, set properties */
  get displayProducts(): Product[] {
    return this._displayProducts;
  }

  set displayProducts(values: Product[]) {
    this._displayProducts = values;
    this.sortedData = new MatTableDataSource<Product>(this._displayProducts);

  }

}
