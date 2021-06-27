import { Component, OnInit } from '@angular/core';
import { Setting } from 'src/app/_models/setting';
import { AuthService } from 'src/app/_services/auth.service';
import { OrderService } from 'src/app/_services/order.service';
import { Utils } from 'src/app/shared/utils';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-treasury',
  templateUrl: './treasury.component.html',
  styleUrls: ['./treasury.component.scss']
})
export class TreasuryComponent implements OnInit {
  settings: Setting[];
  regDate: Date;
  regActive = false;
  invoiced: string;
  cashed: string;
  tuitionBalance: number;
  toBeValidated: number;
  amountByDeadline: any;
  lateAmountsDue: any;
  wait = false;
  dataReady = 0;

  constructor(private authService: AuthService, private orderService: OrderService, private alertify: AlertifyService,
    private router: Router) { }

  ngOnInit() {
    this.settings = this.authService.settings;
    const regDate = this.settings.find(s => s.name === 'RegistrationDate').value;
    this.regDate = Utils.inputDateDDMMYY(regDate, '/');
    const today = new Date();
    if (today >= this.regDate) {
      this.regActive = true;
    }
    // this.nbTuitionPays = Number(this.settings.find(s => s.name === 'NbTuitionPayments').value);
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
      this.toBeValidated = data.toBeValidated;
      this.tuitionBalance = data.openBalance - this.toBeValidated;
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

  goToDetails(duedate, invoiced, paid) {
    console.log(duedate);
    this.router.navigate(['dueDateDetails', duedate, invoiced, paid]);
  }

  counter(i: number) {
    return new Array(i);
  }

}
