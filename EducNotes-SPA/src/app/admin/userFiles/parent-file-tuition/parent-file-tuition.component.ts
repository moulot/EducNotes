import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { OrderService } from 'src/app/_services/order.service';
import { PaymentService } from 'src/app/_services/payment.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-parent-file-tuition',
  templateUrl: './parent-file-tuition.component.html',
  styleUrls: ['./parent-file-tuition.component.scss']
})
export class ParentFileTuitionComponent implements OnInit {
  @Input() id: any;
  @Input() childid: any;
  @Input() childFName: any;
  tuition: any;
  showInfos = true;
  payCash = environment.payCash;
  payCheque = environment.payCheque;
  payWire = environment.payWire;
  payMobile = environment.payMobile;
  @Input() showBtn: Boolean = true;

  constructor( private route: ActivatedRoute, private alertify: AlertifyService, private router: Router,
    private orderService: OrderService, private paymentService: PaymentService) { }

  ngOnInit() {
    this.getTuition(this.id);
  }

  getTuition(id) {
    this.orderService.getTuitionFromChild(id).subscribe(data => {
      this.tuition = data;
    }, error => {
      this.alertify.error(error);
    });
  }

  addPayment() {
    this.router.navigate(['addFinOp', this.childid]);
  }

  toggleInfos() {
    this.showInfos = !this.showInfos;
  }

}
