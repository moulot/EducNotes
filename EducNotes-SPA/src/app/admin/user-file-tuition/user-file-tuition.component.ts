import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { environment } from 'src/environments/environment';
import { OrderService } from 'src/app/_services/order.service';
import { PaymentService } from 'src/app/_services/payment.service';

@Component({
  selector: 'app-user-file-tuition',
  templateUrl: './user-file-tuition.component.html',
  styleUrls: ['./user-file-tuition.component.scss']
})
export class UserFileTuitionComponent implements OnInit {
  @Input() id: any;
  @Input() showBtn: Boolean = true;
  tuition: any;
  payCash = environment.payCash;
  payCheque = environment.payCheque;
  payWire = environment.payWire;
  payMobile = environment.payMobile;

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
    this.router.navigate(['addFinOp', this.id]);
  }

}
