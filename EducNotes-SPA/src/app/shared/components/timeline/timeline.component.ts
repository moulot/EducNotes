import { Component, OnInit, Input } from '@angular/core';
import { SharedAnimations } from '../../animations/shared-animations';

@Component({
  selector: 'app-timeline',
  templateUrl: './timeline.component.html',
  styleUrls: ['./timeline.component.scss'],
  animations: [SharedAnimations]
})
export class TimelineComponent implements OnInit {
  @Input() events: any;
  @Input()  isParentConnected: boolean;

  constructor() { }

  ngOnInit() {
  }

}
