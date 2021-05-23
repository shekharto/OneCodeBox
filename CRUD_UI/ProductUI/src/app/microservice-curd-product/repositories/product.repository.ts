import {EventEmitter, Injectable, Injector} from "@angular/core";
import { catchError, map, tap } from 'rxjs/operators';

import {ApiResult} from "../../shared/entities/api-result";
import {Product} from "../../shared/entities/product";
import {ProductService} from "../services/product.service";
import {environment} from "../../../environments/environment";
import {Observable} from "rxjs";
import {BaseRepository} from "../../shared/repositories/base.repository";
import {EventType} from "../../shared/entities/enums";



@Injectable({providedIn: "root"})
export class ProductRepository  extends BaseRepository  {

  constructor(injector: Injector, private service: ProductService) {
    super(injector);
  }

  getProducts(): Promise<Product[]> {
      return this.getMultiple<Product>(this.service, this.service.getProducts.bind(this.service));
  }

  getAllChildEntities(): Promise<Product[]> {
    return this.getMultiple<Product>(this.service, this.service.getAllChildEntities.bind(this.service));
  }

  getProductById(id: number): Promise<Product> {
    return this.getSingle<Product>(this.service, this.service.getProductById.bind(this.service, id));
  }

  deleteProduct(id: number): Promise<any> {
    return this.deleteSingle<any>(this.service, this.service.deleteProduct.bind(this.service, id));
  }

  postProduct(product: Product): Promise<Product> {
    return new Promise<Product>((resolve, reject) => {
      this.service.postProduct(product).then(result => {
        if (result.isSuccess) {
          resolve(product);
        } else {
          console.log("Add product failed.");
          reject(result.message);
        }
      })
        .catch(error => {
          console.log(error.message);
        });
    });
  }

  postProducts(products: Product[]): Promise<Product[]> {
    return new Promise<Product[]>((resolve, reject) => {
      this.service.postProducts(products).then(result => {
        if (result.isSuccess) {
            resolve(products);
          } else {
             console.log("Add product failed.");
            reject(result.message);
        }
      })
        .catch(error => {
            console.log(error.message);
        });
    });
  }

}
