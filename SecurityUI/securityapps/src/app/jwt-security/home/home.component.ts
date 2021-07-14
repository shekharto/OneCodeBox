import { Component, OnInit } from '@angular/core';

import {AuthenticationService} from '../repositories-services/authentication.service';
import {AuthenticateResponse} from "../entities/authenticateResponse";
import {moduleType} from "../../../Shared/enums/enums";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  title: string;
  user: AuthenticateResponse;
  modules =[];

  constructor(private authenticationService: AuthenticationService) {
    this.authenticationService.user.subscribe(x => this.user = x);
  }

  ngOnInit(): void {
    this.title = "Shekhar";
    this.subMenuAccess();
  }

  private subMenuAccess(): void {
    let taskList = this.authenticationService.getUserTaskArray();

    // this is about enable/display the relevant module menu on nav-bar
    // search the tasklist and enable the relevant module on homepage
    for (let element in moduleType) {
      if (taskList.some(task => task.toLowerCase().startsWith(element))) {
        this.modules.push(moduleType[element]);
      }
    }
  }

  logout() {
    this.authenticationService.logout();
  }

}
