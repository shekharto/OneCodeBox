import {Component, Inject, Input, OnInit} from '@angular/core';
import {Product} from "../../shared/entities/product";
import {MatDialog, MAT_DIALOG_DATA} from '@angular/material/dialog';

@Component({
  selector: 'app-dialog-product',
  templateUrl: './dialog-product.component.html',
  styleUrls: ['./dialog-product.component.scss']
})
export class DialogProductComponent implements OnInit {
 // @Input() productDetail: Product;
  price: number;
 constructor(@Inject(MAT_DIALOG_DATA) public data: any) { }

 // constructor() { }

  ngOnInit(): void {

    this.price = this.data.price;
  }



}
