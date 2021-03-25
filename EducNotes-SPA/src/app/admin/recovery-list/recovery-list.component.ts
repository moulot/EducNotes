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
  toggleLevelChild = true;

  constructor(private orderService: OrderService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.getLevelLatePayments();
    this.getChildRecovery();
  }

  getLevelLatePayments() {
    this.orderService.getLevelLatePayments().subscribe(data => {
      this.levelRecovery = data;
    }, error => {
      this.alertify.error(error);
    });
  }

  getChildRecovery() {
    this.orderService.getChildLatePayments().subscribe(data => {
      this.childRecovery = data;
    }, error => {
      this.alertify.error(error);
    });
  }

  toggleView() {
    this.toggleLevelChild = !this.toggleLevelChild;
  }

}
