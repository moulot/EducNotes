import { Component, OnInit } from '@angular/core';
import { Utils } from 'src/app/shared/utils';
import { FormGroup, Validators, FormBuilder, FormArray } from '@angular/forms';
import { environment } from 'src/environments/environment';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router, ActivatedRoute } from '@angular/router';
import { PaymentService } from 'src/app/_services/payment.service';
import { TuitionData } from 'src/app/_models/tuitionData';

@Component({
  selector: 'app-add-payment',
  templateUrl: './add-payment.component.html',
  styleUrls: ['./add-payment.component.scss']
})
export class AddPaymentComponent implements OnInit {
  paymentForm: FormGroup;
  myDatePickerOptions = Utils.myDatePickerOptions;
  bankOptions = [];
  banks: any;
  showPayDataBox = false;
  showCashData = false;
  showChequeData = false;
  showWireData = false;
  orderid: any;
  invoiceid: any;
  wait = false;
  tuitionData = <TuitionData>{};
  payCash = environment.payCash;
  payCheque = environment.payCheque;
  payWire = environment.payWire;
  paytypes: any;
  userFile: any;
  childid: any;
  showInfos = false;

  constructor(private fb: FormBuilder, private alertify: AlertifyService,
    private router: Router, private paymentService: PaymentService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe((data: any) => {
      this.userFile = data['file'];
    });
    this.route.params.subscribe(params => {
      this.childid = params['id'];
    });
    this.createPaymentForm();
    this.getPaymentData();
    this.addPaymentItem(0, 0, this.userFile.orderLineId, '', '', 0, '', 0);
    this.orderid = this.userFile.orderId;
  }

  getPaymentData() {
    this.paymentService.getPaymentData().subscribe((data: any) => {
      this.paytypes = data.paymentTypes;
      this.banks = data.banks;
      for (let i = 0; i < this.banks.length; i++) {
        const elt = this.banks[i];
        const bank = {value: elt.id, label: elt.name};
        this.bankOptions = [...this.bankOptions, bank];
      }
    }, error => {
      this.alertify.error(error);
    });
  }

  toggleInfos() {
    this.showInfos = !this.showInfos;
  }

  createPaymentForm() {
    this.paymentForm = this.fb.group({
      typeid: [null, Validators.required],
      opDate: ['', Validators.required],
      amount: ['', Validators.required],
      bankid: [0],
      numCheque: [''],
      refDoc: [''],
      note: [''],
      payments: this.fb.array([])
    }, {validator: this.paymentValidator});
  }

  paymentValidator(g: FormGroup) {
    let chequeerror = false;
    const paytype = g.get('typeid').value;
    const numcheque = g.get('numCheque').value;
    if ((Number(paytype) === environment.payCheque && numcheque === '')) {
      chequeerror = true;
    }

    if (chequeerror === true) {
      return {'chequeNOK': true};
    }

    return null;
  }

  addPaymentItem(id, finOpId, orderlineId, lname, fname, prodid, prodname, lineamnt): void {
    const courses = this.paymentForm.get('payments') as FormArray;
    courses.push(this.createPaymentItem(id, finOpId, orderlineId, lname, fname, prodid, prodname, lineamnt));
  }

  createPaymentItem(id, finOpId, orderlineId, lname, fname, prodid, prodname, amnt): FormGroup {
    return this.fb.group({
      id: id,
      finOpId: finOpId,
      orderLineId: orderlineId,
      childlname: lname,
      childfname: fname,
      prodid: prodid,
      prodname: prodname,
      amount: amnt
    });
  }

  savePayment() {
    this.wait = true;
    const paydata = <any>{};
    paydata.finOpDate = Utils.inputDateDDMMYY(this.paymentForm.value.opDate, '/');
    paydata.finOpTypeId = environment.finOpTypePayment;
    paydata.orderId = this.orderid;
    paydata.invoiceId = this.invoiceid;
    paydata.paymentTypeId = this.paymentForm.value.typeid;
    paydata.amount = this.paymentForm.value.amount;
    paydata.numCheque = this.paymentForm.value.numCheque;
    paydata.refDoc = this.paymentForm.value.refDoc;
    paydata.note = this.paymentForm.value.note;
    paydata.bankId = this.paymentForm.value.bankid;
    const elt = this.paymentForm.value.payments[0];
    const payments = [{id: elt.id, finOpId: elt.finOpId, orderLineId: elt.orderLineId, childLastName: elt.childlname,
      childFirstName: elt.childfname, productId: elt.prodid, productName: elt.prodname, amount: paydata.amount}];
    paydata.payments = payments;
    this.paymentService.addTuitionPayment(paydata).subscribe(() => {
      this.alertify.success('paiment effectué avec succès');
      this.router.navigate(['/userFile', this.childid]);
    }, error => {
      this.alertify.error(error);
      this.wait = false;
    });
  }

  selectPayType() {
    this.showPayDataBox = true;
    const typeid = this.paymentForm.value.typeid;
    this.showCashData = false;
    this.showChequeData = false;
    this.showWireData = false;
    switch (Number(typeid)) {
      case this.payCash:
        this.showCashData = true;
        break;
      case this.payCheque:
        this.showChequeData = true;
        break;
      case this.payWire:
        this.showWireData = true;
        break;
      default:
        break;
    }
  }
}
