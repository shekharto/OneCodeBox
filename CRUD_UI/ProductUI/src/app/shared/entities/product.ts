import {ObjectStateType} from "./enums";
import {productPrice} from "../../shared/entities/productPrice";

export class Product {
  productId: number;
  name: string;
  description: string;
  active: boolean;
  objectState: ObjectStateType;
  productPrices: productPrice[] = [];
  constructor() {
    this.active = false;
  }
}
