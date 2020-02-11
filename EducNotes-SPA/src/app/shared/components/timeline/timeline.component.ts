import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from '../../animations/shared-animations';

@Component({
  selector: 'app-timeline',
  templateUrl: './timeline.component.html',
  styleUrls: ['./timeline.component.scss'],
  animations: [SharedAnimations]
})
export class TimelineComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
