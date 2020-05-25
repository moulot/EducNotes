import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import { Setting } from 'src/app/_models/setting';
import { Utils } from 'src/app/shared/utils';
import { OrderService } from 'src/app/_services/order.service';

@Component({
  selector: 'app-tuition-panel',
  templateUrl: './tuition-panel.component.html',
  styleUrls: ['./tuition-panel.component.scss']
})
export class TuitionPanelComponent implements OnInit {
  settings: Setting[];
  regDate: Date;
  regActive = false;
  nbTuitionPays: number;
  tuitionBalance: string;

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
    this.orderService.getTuitionData().subscribe((data: any) => {
      this.tuitionBalance = data.openBalance;
    })
  }

  counter(i: number) {
    return new Array(i);
  }

}
