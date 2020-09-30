import { Component, OnInit } from '@angular/core';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { OrderService } from 'src/app/_services/order.service';

@Component({
  selector: 'app-recovery-list',
  templateUrl: './recovery-list.component.html',
  styleUrls: ['./recovery-list.component.scss']
})
export class RecoveryListComponent implements OnInit {
  levelRecovery: any;
  childRecovery: any;

  constructor(private orderService: OrderService, private alertify: AlertifyService) { }

  ngOnInit() {
  }

  getLevelRecovery() {
    this.orderService.getLevelRecovery().subscribe(data => {
      this.levelRecovery = data;
    }, error => {
      this.alertify.error(error);
    });
  }

  getChildRecovery() {
    this.orderService.getChildRecovery().subscribe(data => {
      this.childRecovery = data;
    }, error => {
      this.alertify.error(error);
    });
  }

}
