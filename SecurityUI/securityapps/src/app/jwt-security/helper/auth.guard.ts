import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';

import {AuthenticationService} from "../repositories-services/authentication.service";


@Injectable({ providedIn: 'root'})
export class AuthGuard implements CanActivate {

  constructor( private router: Router,
               private authenticationService: AuthenticationService) {  }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
    const user = this.authenticationService.currentUserValue;
    if (user) {
      // user is logged-in.. return true
      return true;
    } else {
      // not logged in so redirect to login page with the return url
      this.router.navigate(['/jwtLogin'], { queryParams: { returnUrl: state.url} });
      return false;
    }
  }

}
