import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-schedule-day',
  templateUrl: './schedule-day.component.html',
  styleUrls: ['./schedule-day.component.css']
})
export class ScheduleDayComponent implements OnInit {
  @Input() items = [];

  constructor() { }

  ngOnInit() {
  }

}
