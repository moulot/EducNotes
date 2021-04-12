import { Component, OnInit } from '@angular/core';
import { Setting } from 'src/app/_models/setting';
import { AuthService } from 'src/app/_services/auth.service';
import { OrderService } from 'src/app/_services/order.service';
import { Utils } from 'src/app/shared/utils';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-treasury',
  templateUrl: './treasury.component.html',
  styleUrls: ['./treasury.component.scss']
})
export class TreasuryComponent implements OnInit {
  settings: Setting[];
  regDate: Date;
  regActive = false;
  nbTuitionPays: number;
  invoiced: string;
  cashed: string;
  tuitionBalance: string;
  toBeValidated: string;
  amountByDeadline: any;
  lateAmountsDue: any;
  wait = false;
  dataReady = 0;

  constructor(private authService: AuthService, private orderService: OrderService,
    private alertify: AlertifyService) { }

  ngOnInit() {
    this.settings = this.authService.settings;
    const regDate = this.settings.find(s => s.name === 'RegistrationDate').value;
    this.regDate = Utils.inputDateDDMMYY(regDate, '/');
    const today = new Date();
    if (today >= this.regDate) {
      this.regActive = true;
    }
    this.nbTuitionPays = Number(this.settings.find(s => s.name === 'NbTuitionPayments').value);
    this.getBalanceData();
    this.getAmountByDeadline();
  }

  getBalanceData() {
    this.wait = true;
    this.orderService.getBalanceData().subscribe((data: any) => {
      this.dataReady++;
      if (this.dataReady === 2) {
        this.wait = false;
      }
      this.invoiced = data.invoiced;
      this.cashed = data.cashed;
      this.tuitionBalance = data.openBalance;
      this.toBeValidated = data.toBeValidated;
    }, error => {
      this.alertify.error(error);
    });
  }

  getAmountByDeadline() {
    this.wait = true;
    this.orderService.getOrderAmountByDeadline().subscribe(data => {
      this.dataReady++;
      if (this.dataReady === 2) {
        this.wait = false;
      }
      this.amountByDeadline = data;
    }, error => {
      this.alertify.error(error);
    });
  }

  counter(i: number) {
    return new Array(i);
  }

}
