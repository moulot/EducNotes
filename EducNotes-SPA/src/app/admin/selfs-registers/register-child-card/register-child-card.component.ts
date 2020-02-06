import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-register-child-card',
  templateUrl: './register-child-card.component.html',
  styleUrls: ['./register-child-card.component.scss']
})
export class RegisterChildCardComponent implements OnInit {
  @Input() child: any;
  @Input() prodSelected: any;
  @Output() edit = new EventEmitter();
  @Output() confirm = new EventEmitter();

  constructor() { }

  ngOnInit() {
  }

  editChild(child) {
    this.edit.emit(child);
  }

  confirmChild(child) {
    this.confirm.emit(child);
  }

}
