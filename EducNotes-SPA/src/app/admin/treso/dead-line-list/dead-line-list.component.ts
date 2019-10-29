import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DeadLine } from 'src/app/_models/deadline';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';

@Component({
  selector: 'app-dead-line-list',
  templateUrl: './dead-line-list.component.html',
  styleUrls: ['./dead-line-list.component.scss'],
  animations: [SharedAnimations]
})
export class DeadLineListComponent implements OnInit {
  deadLines: DeadLine[];

  constructor(private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.deadLines = data.deadlines;
    });
  }

}
