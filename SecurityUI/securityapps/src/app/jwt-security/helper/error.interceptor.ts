import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { AuthenticationService} from "../repositories-services/authentication.service";

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private authenticationService: AuthenticationService) {
  }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(catchError(err => {
      if ([401, 403].includes(err.status) && this.authenticationService.currentUserValue) {
        // auto logout if 401 and 403 response returned from api
        this.authenticationService.logout();
      }

      const error = (err && err.error && err.error.message) || err.statusText;
      console.error(err); // print on console
      return throwError(error);

    }));

  }


}
