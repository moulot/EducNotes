import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-classday-schedule',
  templateUrl: './classday-schedule.component.html',
  styleUrls: ['./classday-schedule.component.scss']
})
export class ClassdayScheduleComponent implements OnInit {
  @Input() items = [];

  constructor() { }

  ngOnInit() {
  }

}
