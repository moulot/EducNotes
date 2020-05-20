import { Component, OnInit } from '@angular/core';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { UserService } from 'src/app/_services/user.service';
import { Utils } from 'src/app/shared/utils';
import { environment } from 'src/environments/environment';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { AuthService } from 'src/app/_services/auth.service';
import { OrderToValidate } from 'src/app/_models/orderToValidate';
import { OrderlineToValidate } from 'src/app/_models/orderlineToValidate';

@Component({
  selector: 'app-user-account',
  templateUrl: './user-account.component.html',
  styleUrls: ['./user-account.component.scss']
})
export class UserAccountComponent implements OnInit {
  parent: any = [];
  selectedSms = [];
  registration: any;
  nextRegistration: any;
  orderCreated = environment.orderCreated;
  regForm: FormGroup;
  settings: any;
  regNbChild: number;


  constructor(private alertify: AlertifyService, private route: ActivatedRoute,
    private fb: FormBuilder, private userService: UserService, private authService: AuthService) { }

  ngOnInit() {
    this.settings = this.authService.settings;
    this.createRegForm();
    this.route.data.subscribe((data: any) => {
      const account = data['account'];
      this.parent = account.parent;
      this.selectedSms = account.activeSms;
      this.registration = this.parent.registration;
      for (let i = 0; i < this.registration.lines.length; i++) {
        const elt = this.registration.lines[i];
        this.addLineItem(elt.id, elt.childLastName, elt.childFirstName, elt.childClassName, elt.strAmountTTC);
      }
      this.regNbChild = this.registration.lines.length;
      this.nextRegistration = this.parent.nextRegistration;
    });
  }

  createRegForm() {
    this.regForm = this.fb.group({
      lines: this.fb.array([])
    }, {validator: this.formValidator});
  }

  formValidator(g: FormGroup) {
    const lines = g.get('lines') as FormArray;
    let checked = false;
    for (let i = 0; i < lines.length; i++) {
      const elt = lines.controls[i];
      if (elt.value.selected === true) {
        checked = true;
        break;
      }
    }
    if (checked === false) {
      return {'formNOK': true};
    }
  }

  addLineItem(id, lastName, firstName, className, amountTTC): void {
    const lines = this.regForm.get('lines') as FormArray;
    lines.push(this.createLineItem(id, lastName, firstName, className, amountTTC, false));
  }

  createLineItem(id, lastName, firstName, className, amountTTC, selected): FormGroup {
    return this.fb.group({
      lineId: id,
      lastName: lastName,
      firstName: firstName,
      className: className,
      amountTTC: amountTTC,
      selected: selected
    });
  }

  setSmsChoice(data) {
    const childId = data.childId;
    const smsId = data.smsId;
    const active = data.active;
    const smsData = <any>{};
    smsData.childId = childId;
    smsData.smsId = smsId;
    const smsFound = this.selectedSms.findIndex(s => s.smsId === smsId && s.childId === childId);
    if (smsFound === -1) {
      if (active === true) {
        this.selectedSms = [...this.selectedSms, smsData];
      }
    } else {
      if (active === false) {
        this.selectedSms.splice(smsFound , 1);
      }
    }
  }

  saveUserSMS() {
    this.userService.saveUserSms(this.parent.id, this.selectedSms).subscribe(() => {
      Utils.smoothScrollToTop();
      this.alertify.success('le choix des sms est validé');
    }, error => {
      this.alertify.error(error);
    });
  }

  validateReg() {
    const order = <OrderToValidate>{};
    order.orderlineIds = [];
    order.orderId = this.registration.id;
    for (let i = 0; i < this.regForm.value.lines.length; i++) {
      const elt = this.regForm.value.lines[i];
      let cancelled = true;
      if (elt.selected === true) {
        cancelled = false;
      }
      order.orderlineIds = [...order.orderlineIds, {orderlineId: elt.lineId, cancelled: cancelled}];
    }
    console.log(order);
  }

}
