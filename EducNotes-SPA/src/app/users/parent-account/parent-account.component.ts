import { Component, Input, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Utils } from 'src/app/shared/utils';
import { OrderlineToValidate } from 'src/app/_models/orderlineToValidate';
import { OrderToValidate } from 'src/app/_models/orderToValidate';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-parent-account',
  templateUrl: './parent-account.component.html',
  styleUrls: ['./parent-account.component.scss']
})
export class ParentAccountComponent implements OnInit {
  @Input() parentid: any;
  parent: any = [];
  selectedSms = [];
  registration: any;
  nextRegistration: any;
  regForm: FormGroup;
  settings: any;
  regNbChild: number;
  daysToValidateReg: number;
  regFee: number;
  downPayment: any;
  feeDetails: any[] = [];
  ChildToBeConfirmed = 0;
  showInfos = [true, false, false];

  constructor(private alertify: AlertifyService, private route: ActivatedRoute, private authService: AuthService,
    private fb: FormBuilder, private userService: UserService) { }

  ngOnInit() {
    this.settings = this.authService.settings;
    this.daysToValidateReg = this.settings.find(s => s.name === 'DaysToValidateReg').value;
    this.regFee = this.settings.find(s => s.name === 'RegFee').value;
    // this.downPayment = Number(this.regFee);
    this.createRegForm();
    this.userService.getParentAccount(this.parentid).subscribe((data: any) => {
      const account = data;
      this.parent = account.parent;
      this.selectedSms = account.activeSms;
      this.registration = this.parent.registration;
      // children to be updated and confirmed
      for (let i = 0; i < this.parent.children.length; i++) {
        const elt = this.parent.children[i];
        if (elt.child.validated) {
          this.ChildToBeConfirmed += 1;
        }
      }
      for (let i = 0; i < this.registration.lines.length; i++) {
        const elt = this.registration.lines[i];
        this.addLineItem(elt.id, elt.childLastName, elt.childFirstName, elt.classLevelName, elt.childClassName, elt.strAmountTTC);
        this.downPayment = this.downPayment + Number(elt.amountTTC);
      }
      this.setDuePayment(this.registration.lines);
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

  addLineItem(id, lastName, firstName, levelName, className, amountTTC): void {
    const lines = this.regForm.get('lines') as FormArray;
    lines.push(this.createLineItem(id, lastName, firstName, levelName, className, amountTTC, false));
  }

  createLineItem(id, lastName, firstName, levelName, className, amountTTC, selected): FormGroup {
    return this.fb.group({
      lineId: id,
      lastName: lastName,
      firstName: firstName,
      levelName: levelName,
      className: className,
      amountTTC: amountTTC,
      selected: selected
    });
  }

  orderRemoveLines() {
    const lines = this.regForm.get('lines') as FormArray;
    while (lines.length > 0) {
      lines.removeAt(0);
    }
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
      Utils.smoothScrollToTop(10);
      this.alertify.success('le choix des sms est validé');
    }, error => {
      this.alertify.error(error);
    });
  }

  validateReg() {
    const order = <OrderToValidate>{};
    let ids: OrderlineToValidate[] = [];
    order.orderId = this.registration.id;
    for (let i = 0; i < this.regForm.value.lines.length; i++) {
      const elt = this.regForm.value.lines[i];
      let cancelled = true;
      if (elt.selected === true) {
        cancelled = false;
      }
      ids = [...ids, {orderlineId: elt.lineId, cancelled: cancelled}];
    }
    order.orderlineIds = ids;
    this.userService.validateRegistration(order).subscribe(updatedOrder => {
      this.registration = updatedOrder;
      this.orderRemoveLines();
      for (let i = 0; i < this.registration.lines.length; i++) {
        const elt = this.registration.lines[i];
        this.addLineItem(elt.id, elt.childLastName, elt.childFirstName, elt.classLevelName, elt.childClassName, elt.strAmountTTC);
      }
      this.setDuePayment(this.registration.lines);
      this.regNbChild = this.registration.lines.length;
    }, error => {
      this.alertify.error(error);
    });
  }

  setDuePayment(lines) {
    this.downPayment = 0;
    for (let i = 0; i < lines.length; i++) {
      const elt = lines[i];
      const payment = elt.payments.find(p => p.seq === 1);
      const firstDP = payment.amount;
      this.downPayment = Number(this.downPayment) + Number(this.regFee) + Number(firstDP);
      const total = Number(this.regFee) + Number(firstDP);
      const childfee = {child: elt.childFirstName, pct: payment.percent, amnt: firstDP,
        tuitionfee: this.regFee, total: total};
      this.feeDetails = [...this.feeDetails, childfee];
    }
  }

  toggleInfos(index) {
    this.showInfos[index] = !this.showInfos[index];
  }
}
