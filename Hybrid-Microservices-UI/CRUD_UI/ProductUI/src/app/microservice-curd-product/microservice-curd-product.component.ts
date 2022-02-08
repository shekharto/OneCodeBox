import {Component, ElementRef, Inject, Injector, Input, OnInit, ViewChild} from "@angular/core";
import {MatTableDataSource} from '@angular/material/table';
import {Sort} from '@angular/material/sort';
import {SelectionModel} from '@angular/cdk/collections';
import { Location } from '@angular/common';

import {ComponentBase} from './../shared/helpers/componentbase';
import {EventService} from './../shared/service/event.service';
import {Product} from "../shared/entities/product";
import {ProductRepository} from "./repositories/product.repository";
import { EventType } from '../shared/entities/enums';
import {MAT_DIALOG_DATA, MatDialog} from "@angular/material/dialog";
import { DialogProductComponent } from "./dialog-product/dialog-product.component";
import {SortingService} from "../shared/service/sorting.service";

@Component({
  selector: 'app-microservice-curd-product',
  templateUrl: './microservice-curd-product.component.html',
  styleUrls: ['./microservice-curd-product.component.scss']
})
export class MicroserviceCurdProductComponent extends ComponentBase implements OnInit {
  displayedColumns = ['select', 'name', 'description', 'active'];
  sortingDisplayedColumns = [ 'name', 'description', 'active'];
  private _displayProducts: Product[] = [];
  private product: Product;
  selectedProductId: number;
  sortedData: any;
  selection = new SelectionModel<Product>(true, []);
  baseProductData: any;
  directData: Product[] = [];

  constructor(  self: ElementRef, injector: Injector,private productRepository: ProductRepository,
              private location: Location, public sortingService: SortingService,
              private eventService: EventService,
                private  dialog: MatDialog ) {
    super(self, injector);
   }

  ngOnInit() {
    this.loadProduct();
    this.subscribeEvents();
    this.sortingService.sortingParameter = [];
  }

  /* private / public methods */

  loadProduct(): void {
    this.productRepository.getProducts().then(
      data => {
        this.displayProducts =  data;
        this.directData = data;
       }
    )}

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

  getDetails(): void {
    // get all child entities
    this.productRepository.getAllChildEntities().then(
      data1 => {
        this.product =  data1.find(item => item.productId === this.selectedProductId);

        this.dialog.open(DialogProductComponent, {
          data: {
              name: this.product.name,
              description: this.product.description,
              price: this.product.productPrices[0].price

      },
        });
      });
  }

  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected && this.selection.selected.length;
    const numRows =  this.sortedData.data && this.sortedData.data.length;
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

   onClientSort(): void {

  }

  decode(product: Product, property: string, direction: string): any {
    switch (property) {
      case "name":
        return product.name.toLowerCase();
      case "active":
        return product.active;
    }
  }

  applyFilter(event: Event) {
    const filterValue = (event.target as HTMLInputElement).value;
    this.sortedData.filter = filterValue.trim().toLowerCase();
  }

  clearData(event: Event) {
     this.sortedData.filter = "";
  }



  private compare(a: number | string | boolean, b: number | string | boolean, isAsc: boolean) {
    return (a < b ? -1 : 1) * (isAsc ? 1 : -1);
  }

  /* get, set properties */
  get displayProducts(): Product[] {
    return this._displayProducts;
  }

  set displayProducts(values: Product[]) {
    this._displayProducts = values;
    this.sortedData = new MatTableDataSource<Product>(this._displayProducts);

    this.baseProductData = new MatTableDataSource<Product>(this._displayProducts);

  }

}

