import { AuthenticationService  } from "../repositories-services/authentication.service";
import {AuthenticateResponse} from "../entities/authenticateResponse";
import {environment} from "@environments/environment";
import {map} from "rxjs/operators";

export function appInitializer(authenticationService: AuthenticationService) {
 return () => new Promise(resolve => {
    // attempt to refresh token on app start up to auto authenticate
   // this is useful when we copy and paste the url in browser and user is already logged in ... it should open the app.
    authenticationService.refreshToken()
      .subscribe()
      .add(resolve);
  }).catch(error => {
   console.log(error.message);
 });

}
