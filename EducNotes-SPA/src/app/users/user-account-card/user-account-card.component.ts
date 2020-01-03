import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-user-account-card',
  templateUrl: './user-account-card.component.html',
  styleUrls: ['./user-account-card.component.scss']
})
export class UserAccountCardComponent implements OnInit {
  @Input() child: any;

  constructor() { }

  ngOnInit() {
  }

}
