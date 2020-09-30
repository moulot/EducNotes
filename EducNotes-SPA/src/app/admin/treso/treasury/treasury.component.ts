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
  lateAmount: string;
  lateAmount7Days: any;
  lateAmount15Days: any;
  lateAmount30Days: any;
  lateAmount60Days: any;
  lateAmount60DaysPlus: any;
  amountByDeadline: any;


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
    this.orderService.getBalanceData().subscribe((data: any) => {
      this.invoiced = data.invoiced;
      this.cashed = data.cashed;
      this.tuitionBalance = data.openBalance;
      this.toBeValidated = data.toBeValidated;
      this.lateAmount = data.lateAmount;
      this.lateAmount7Days = data.lateAmount7Days;
      this.lateAmount15Days = data.lateAmount15Days;
      this.lateAmount30Days = data.lateAmount30Days;
      this.lateAmount60Days = data.lateAmount60Days;
      this.lateAmount60DaysPlus = data.lateAmount60DaysPlus;
    }, error => {
      this.alertify.error(error);
    });
  }

  getAmountByDeadline() {
    this.orderService.getOrderAmountByDeadline().subscribe(data => {
      this.amountByDeadline = data;
    }, error => {
      this.alertify.error(error);
    });
  }

  counter(i: number) {
    return new Array(i);
  }

}
