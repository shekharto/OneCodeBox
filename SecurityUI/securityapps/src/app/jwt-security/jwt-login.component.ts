import { NgModule, Component, OnInit  } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { first } from 'rxjs/operators';

import {Login} from "./entities/login";
import {JwtRepository} from "./repositories-services/repository";
import {AuthenticationService} from "../jwt-security/repositories-services/authentication.service";

@Component({
  selector: 'app-jwt-security',
  templateUrl: './jwt-login.component.html',
  styleUrls: ['./jwt-login.component.scss']
})
export class JwtLoginComponent implements OnInit    {
  login: Login;
  returnUrl: string;
  error = '';

  constructor(private repository: JwtRepository,
              private authenticationService: AuthenticationService,
              private route: ActivatedRoute,
              private router: Router) {
   this.login  = new  Login();
   // for testing
    this.login.loginId = "admin";
    this.login.password = "admin";

   // redirect to home if already logged in
    if (this.authenticationService.currentUserValue) {
      this.router.navigate(['/']);
    }
  }

  ngOnInit(): void {
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/jwtlogin/jwthome';
  }

    userAuthentication(): void {
     this.authenticationService.login(this.login)
        .subscribe({
        next: () => {
         this.router.navigate([this.returnUrl]);
        },
        error: error => {
          this.error = error;
        }
      })

    /*let authenticateRequest = new AuthenticateRequest(this.login.loginId, this.login.password)
    this.repository.postAuthenticateUser(authenticateRequest).then( data => {
      alert("My Token: " + data.jwtToken);
    });*/
  }

}
