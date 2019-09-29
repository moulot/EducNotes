import { Component, OnInit, Input } from '@angular/core';
import { Agenda } from 'src/app/_models/agenda';

@Component({
  selector: 'app-agenda-item',
  templateUrl: './agenda-item.component.html',
  styleUrls: ['./agenda-item.component.css']
})
export class AgendaItemComponent implements OnInit {
  @Input() items: any;

  constructor() { }

  ngOnInit() {
  }

}
