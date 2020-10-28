import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-absence-day',
  templateUrl: './absence-day.component.html',
  styleUrls: ['./absence-day.component.scss']
})
export class AbsenceDayComponent implements OnInit {
  @Input() dayData: any;

  constructor() { }

  ngOnInit() {
    
  }

}
