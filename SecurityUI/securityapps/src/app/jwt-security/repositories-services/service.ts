import {Injectable, Injector} from "@angular/core";
import {HttpClient} from "@angular/common/http";

import { environment } from '@environments/environment';
import {AuthenticateResponse} from "../entities/authenticateResponse";
import {AuthenticateRequest} from "../entities/authenticateRequest";
import {Observable} from "rxjs";

@Injectable({providedIn: "root"})
export class JwtService {
  url: string;

  constructor(private httpClient: HttpClient) {
   // super();
     this.url = `${environment.apiUrl}/users`;
  }


  postRevokeToken(): Promise<any> {
    let apiUrl = `${this.url}/revokeToken`;
    return this.httpClient.post<any>(apiUrl, {}, { withCredentials: true}).toPromise();
  }

}
