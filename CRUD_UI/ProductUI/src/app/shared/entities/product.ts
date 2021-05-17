import {ObjectStateType} from "./enums";

export class Product {
  productId: number;
  name: string;
  description: string;
  active: boolean;
  objectState: ObjectStateType;
  constructor() {
    this.active = false;
  }
}
