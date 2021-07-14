import {EventEmitter, Injectable, Injector} from "@angular/core";
import { catchError, map, tap } from 'rxjs/operators';

import {Login} from "../entities/login";
import {AuthenticateRequest} from "../entities/authenticateRequest";
import {AuthenticateResponse} from "../entities/authenticateResponse";

import {JwtService} from "./service";
import {Observable} from "rxjs";

@Injectable({providedIn: "root"})
export class JwtRepository {

  constructor(injector: Injector, private service: JwtService) {
  }


/*  postAuthenticateUser(authenticateRequest: AuthenticateRequest): Promise<AuthenticateResponse> {
    return new Promise<AuthenticateResponse>( (resolve, reject) => {
      // manipulate to AuthenticateRequest
      this.service.postAuthenticateUser(authenticateRequest).then(result => {
          if (result.jwtToken && result.jwtToken.length > 0) {
            resolve(result);
          } else {
            console.log("Authentication failed.");
            reject("Authentication failed.");
          }
      }).catch(error => {
        console.log(error.message);
      });
    });
  }*/

  postRevokeToken(): Promise<any> {
    return new Promise<AuthenticateResponse>( (resolve, reject) => {
      this.service.postRevokeToken().then(result => {
        if (result && result.message.length > 0) {
          resolve(result);
        } else {
          console.log("Revoke token failed.");
          reject("Revoke token failed.");
        }
      }).catch(error => {
        console.log(error.message);
      });
    });
  }




}
