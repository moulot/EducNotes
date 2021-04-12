import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { OrderService } from 'src/app/_services/order.service';

@Component({
  selector: 'app-recovery',
  templateUrl: './recovery.component.html',
  styleUrls: ['./recovery.component.scss']
})
export class RecoveryComponent implements OnInit {
  lateAmountsDue: any;

  constructor(private orderService: OrderService, private alertify: AlertifyService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe((data: any) => {
      this.lateAmountsDue = data['lateAmounts'];
    });
    // this.getRecoveryData();
  }

  // getRecoveryData() {
  //   this.orderService.getRecoveryData().subscribe((data: any) => {
  //     this.lateAmountsDue = data;
  //   }, error => {
  //     this.alertify.error(error);
  //   });
  // }

}
