import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-child-sms-card',
  templateUrl: './child-sms-card.component.html',
  styleUrls: ['./child-sms-card.component.scss']
})
export class ChildSmsCardComponent implements OnInit {
  @Input() children: any;
  @Input() child: any;
  @Output() setSmsChoice = new EventEmitter<any>();
  @Output() saveUserSMS = new EventEmitter();
  smsChoiceChanged = false;

  constructor() { }

  ngOnInit() {
  }

  setSmsSelected(childId, smsId, active) {
    this.smsChoiceChanged = true;
    const smsData = <any>{};
    smsData.childId = childId;
    smsData.smsId = smsId;
    smsData.active = active;
    this.setSmsChoice.emit(smsData);
  }

  saveSms() {
    this.smsChoiceChanged = false;
    this.saveUserSMS.emit();
  }

}
