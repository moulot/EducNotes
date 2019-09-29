import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-agenda-panel',
  templateUrl: './agenda-panel.component.html',
  styleUrls: ['./agenda-panel.component.css']
})
export class AgendaPanelComponent implements OnInit {
  listActive: boolean;
  scheduleActive: boolean;

  constructor() { }

  ngOnInit() {
    this.listActive = true;
  }

  activateList() {
    this.listActive = true;
    this.scheduleActive = false;
  }

  activateSchedule() {
    this.listActive = false;
    this.scheduleActive = true;
  }

}
