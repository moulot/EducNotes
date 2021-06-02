import { Component, OnInit } from '@angular/core';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-config',
  templateUrl: './config.component.html',
  styleUrls: ['./config.component.scss']
})
export class ConfigComponent implements OnInit {
  activeTabs = [false, false, false, false, false, false];
  buttons = [false, false, false, false, false, false];

  constructor(private alertify: AlertifyService) { }

  ngOnInit() {
  }

  showTab(index) {
    for (let i = 0; i < this.activeTabs.length; i++) {
      this.activeTabs[i] = false;
      this.buttons[i] = false;
    }
    this.activeTabs[index] = true;
    this.buttons[index] = true;
  }

}
