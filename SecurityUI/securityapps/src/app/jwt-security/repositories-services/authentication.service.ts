import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import {catchError, map} from 'rxjs/operators';
import {JwtHelperService} from "@auth0/angular-jwt";

import {JwtRepository} from "../repositories-services/repository";
import { environment } from '@environments/environment';
import {AuthenticateRequest} from "../entities/authenticateRequest";
import { Login } from '../entities/login';
import {AuthenticateResponse} from "../entities/authenticateResponse";
import {JwtTokenModel} from "../../../Shared/entites/jwt-token.model";
import {UserTaskType} from "../../../Shared/enums/user-task-type";

@Injectable({ providedIn: 'root'})
export class AuthenticationService  {
  helper: JwtHelperService = new JwtHelperService();
  private userSubject: BehaviorSubject<AuthenticateResponse>;
  public user: Observable<AuthenticateResponse>;

  constructor( private router: Router, private http: HttpClient, private repository: JwtRepository) {

    this.userSubject = new BehaviorSubject<AuthenticateResponse>(null);
    this.user = this.userSubject.asObservable();
  }

  public get currentUserValue(): AuthenticateResponse {
    return this.userSubject.value;
  }


  /**
   * Decode a JWT token(string) into a JWTToken model object
   */
  decodeToken(): JwtTokenModel {
      let user = this.currentUserValue;
       return this.helper.decodeToken(user.jwtToken);
  }

  getUserRoleNames(): string[] {
    let jwtToken = this.EnsureTokenValidity(null);
    if (jwtToken) {
      return jwtToken.role.split(",");
    } else {
      console.log("error in role array...")
      return [];
    }
  }

  getUserTaskIds(): number[] {
    let jwtToken = this.EnsureTokenValidity(null);
    if (jwtToken) {
      return jwtToken.tasks.length > 0 ? jwtToken.tasks.split(",").map(task => parseInt(task, 10)) : [];
    } else {
      console.log("error in task array...")
      return [];
    }
  }

  getUserTaskArray(): string[] {
    let jwtToken = this.EnsureTokenValidity(null);
    if (jwtToken) {
      return jwtToken.tasks.length > 0 ? jwtToken.tasks.split(",").map(task => UserTaskType[parseInt(task, 10)]) : [];
    } else {
      console.log("error in task array...")
      return [];
    }
  }

  EnsureTokenValidity(workingToken: JwtTokenModel)  {
    // Check for null/empty/false
        if (!!!(workingToken)) {
          // return token from cookie
          return this.decodeToken();
        }
        return workingToken;
    }

  login(login: Login) {
      let authenticateRequest = new AuthenticateRequest(login.loginId, login.password);
      return this.http.post<AuthenticateResponse>(`${environment.apiUrl}/users/authenticate`, authenticateRequest , { withCredentials: true })
          .pipe(map(user => {
             this.userSubject.next(user);
            this.startRefreshTokenTimer();
            return user;
          }));
  }

  refreshToken() {
    return this.http.post<AuthenticateResponse>(`${environment.apiUrl}/users/refreshToken`, {} , { withCredentials: true })
      .pipe(map(user => {
        this.userSubject.next(user);
        this.startRefreshTokenTimer();
        return user;
    }));
  }

  logout() {
    this.repository.postRevokeToken().then(data => {
      this.stopRefreshTokenTimer();
      this.userSubject.next(null);
      this.router.navigate(['/jwtLogin']);
    });
  }

/*   login(login: Login) {
    let authenticateRequest = new AuthenticateRequest(login.loginId, login.password)

    return this.repository.postAuthenticateUser(authenticateRequest).then( data => {
     this.userSubject.next(data);
     this.startRefreshTokenTimer();
      alert("My Token: " + data.jwtToken);
     return data;
   });
  }*/


  // helper methods

  private refreshTokenTimeout;

  private startRefreshTokenTimer() {
    // parse json jwt token.
    const jwttoken = JSON.parse(atob(this.currentUserValue.jwtToken.split('.')[1]));

    // set a timeout to refresh the token a minute before it expire
    const expire = new Date(jwttoken.exp * 1000);
    const timeout = expire.getTime() - Date.now() - (60 * 1000);
    this.refreshTokenTimeout = setTimeout(() => this.refreshToken().subscribe(), timeout);
  }

  private stopRefreshTokenTimer() {
    clearTimeout(this.refreshTokenTimeout);
  }
}
