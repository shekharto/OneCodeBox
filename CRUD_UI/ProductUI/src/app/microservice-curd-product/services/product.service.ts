import {Injectable} from "@angular/core";
import {HttpClient} from "@angular/common/http";
import {ApiResult} from "../../shared/entities/api-result";
import {Product} from "../../shared/entities/product";
import {BaseService} from "../../shared/service/base.service";

@Injectable({providedIn: "root"})
export class ProductService extends BaseService {
  url: string;

  constructor(private httpClient: HttpClient) {
    super();
    this.url = `http://localhost:1098/Product`;
  }

  getProducts(): Promise<ApiResult<Product>> {
    return this.httpClient.get<ApiResult<Product>>(this.url).toPromise();
  }

  getProductById(id: number): Promise<ApiResult<Product>> {
    let apiUrl = `${this.url}/${id}`;
    return this.httpClient.get<ApiResult<Product>>(apiUrl).toPromise();
  }

  deleteProduct(id: number): Promise<ApiResult<Product>> {
    let apiUrl = `${this.url}/${id}`;
    return this.httpClient.delete<ApiResult<Product>>(apiUrl).toPromise();
  }

  postProducts(product: Product[]): Promise<ApiResult<Product>> {
    const apiUrl = this.url; // = `${this.getServiceUrl()}/${UserSecurityRoutes.Root}/${UserSecurityRoutes.UserRole}/${UserSecurityRoutes.UserRoleMapping}`;
    return this.handleHttp<Product>(this.httpClient.post<ApiResult<Product>>(apiUrl, product)).toPromise();
  }

  postProduct(product: Product): Promise<ApiResult<Product>> {
    const apiUrl = this.url; // = `${this.getServiceUrl()}/${UserSecurityRoutes.Root}/${UserSecurityRoutes.UserRole}/${UserSecurityRoutes.UserRoleMapping}`;
    return this.handleHttp<Product>(this.httpClient.post<ApiResult<Product>>(apiUrl, product)).toPromise();
  }

}
