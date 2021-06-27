import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { environment } from 'src/environments/environment';
import { OrderService } from 'src/app/_services/order.service';
import { PaymentService } from 'src/app/_services/payment.service';

@Component({
  selector: 'app-child-file-tuition',
  templateUrl: './child-file-tuition.component.html',
  styleUrls: ['./child-file-tuition.component.scss']
})
export class ChildFileTuitionComponent implements OnInit {
  @Input() id: any;
  @Input() showBtn: Boolean = true;
  tuition: any;
  showInfos = true;
  payCash = environment.payCash;
  payCheque = environment.payCheque;
  payWire = environment.payWire;
  payMobile = environment.payMobile;
  totalPaid: number;
  invoiced: string;
  cashed: string;
  tuitionBalance: number;
  toBeValidated: number;
  amountByDeadline: any;
  lateAmountsDue: any;
  wait = false;
  dataReady = 0;

  constructor( private route: ActivatedRoute, private alertify: AlertifyService, private router: Router,
    private orderService: OrderService, private paymentService: PaymentService) { }

  ngOnInit() {
    this.getTuition(this.id);
    this.getBalanceData(this.id);
    this.getAmountByDeadline(this.id);
  }

  getTuition(id) {
    this.wait = true;
    this.orderService.getTuitionFromChild(id).subscribe(data => {
      this.dataReady++;
      if (this.dataReady === 3) {
        this.wait = false;
      }
      this.tuition = data;
      this.totalPaid = this.tuition.amountInvoiced - this.tuition.balance;
    }, error => {
      this.alertify.error(error);
    });
  }

  getBalanceData(childId) {
    this.wait = true;
    this.orderService.getChildBalanceData(childId).subscribe((data: any) => {
      this.dataReady++;
      if (this.dataReady === 3) {
        this.wait = false;
      }
      this.invoiced = data.invoiced;
      this.cashed = data.cashed;
      this.toBeValidated = data.toBeValidated;
      this.tuitionBalance = data.openBalance - this.toBeValidated;
    }, error => {
      this.alertify.error(error);
      this.wait = false;
    });
  }

  getAmountByDeadline(childId) {
    this.wait = true;
    this.orderService.getAmountByChildByDeadline(childId).subscribe(data => {
      this.dataReady++;
      if (this.dataReady === 3) {
        this.wait = false;
      }
      this.amountByDeadline = data;
    }, error => {
      this.alertify.error(error);
      this.wait = false;
    });
  }

  addPayment() {
    this.router.navigate(['addFinOp', this.id]);
  }

  toggleInfos() {
    this.showInfos = !this.showInfos;
  }

}
