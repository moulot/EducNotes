import { Component, OnInit } from '@angular/core';
import { Setting } from 'src/app/_models/setting';
import { AuthService } from 'src/app/_services/auth.service';
import { OrderService } from 'src/app/_services/order.service';
import { Utils } from 'src/app/shared/utils';

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

  constructor(private authService: AuthService, private orderService: OrderService) { }

  ngOnInit() {
    this.settings = this.authService.settings;
    const regDate = this.settings.find(s => s.name === 'RegistrationDate').value;
    this.regDate = Utils.inputDateDDMMYY(regDate, '/');
    const today = new Date();
    if (today >= this.regDate) {
      this.regActive = true;
    }
    this.nbTuitionPays = Number(this.settings.find(s => s.name === 'NbTuitionPayments').value);
    this.orderService.getBalanceData().subscribe((data: any) => {
      this.invoiced = data.invoiced;
      this.cashed = data.cashed;
      this.tuitionBalance = data.openBalance;
      this.toBeValidated = data.toBeValidated;
    });
  }

  counter(i: number) {
    return new Array(i);
  }

}
