import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'ProductUI';
  isToggle = false;
  constructor() { }

  ngOnInit() {
  }

  public toggle(): void {
    // this.isToggle = !this.isToggle;
  }

}
