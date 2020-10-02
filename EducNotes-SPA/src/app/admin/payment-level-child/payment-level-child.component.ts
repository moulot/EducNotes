import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { OrderService } from 'src/app/_services/order.service';

@Component({
  selector: 'app-payment-level-child',
  templateUrl: './payment-level-child.component.html',
  styleUrls: ['./payment-level-child.component.scss']
})
export class PaymentLevelChildComponent implements OnInit {
  children: any;

  constructor(private alertify: AlertifyService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe((data: any) => {
      this.children = data['children'];
      // console.log(this.children);
    });
  }

}
