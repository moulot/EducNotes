import { Component, OnInit } from '@angular/core';
import { Utils } from 'src/app/shared/utils';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { environment } from 'src/environments/environment';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router, ActivatedRoute } from '@angular/router';
import { PaymentService } from 'src/app/_services/payment.service';
import { TuitionChildData } from 'src/app/_models/tuitionChildData';
import { TuitionData } from 'src/app/_models/tuitionData';
import { OrderService } from 'src/app/_services/order.service';

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
  order: any;
  childid: any;

  constructor(private fb: FormBuilder, private alertify: AlertifyService,
    private router: Router, private orderService: OrderService,
    private paymentService: PaymentService,   private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.order = data['order'];
    });
    this.route.params.subscribe(params => {
      this.childid = params['id'];
    });
    this.createPaymentForm();
    this.getPaymentData();
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

  createPaymentForm() {
    this.paymentForm = this.fb.group({
      typeid: [null, Validators.required],
      opDate: ['', Validators.required],
      amount: ['', Validators.required],
      bankid: [0],
      numCheque: [''],
      refDoc: [''],
      note: ['']
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

  savePayment() {
    this.wait = true;
    const paydata = <any>{};
    paydata.finOpDate = Utils.inputDateDDMMYY(this.paymentForm.value.opDate, '/');
    paydata.orderId = this.orderid;
    paydata.invoiceId = this.invoiceid;
    paydata.paymentTypeId = this.paymentForm.value.typeid;
    paydata.amount = this.paymentForm.value.amount;
    paydata.numCheque = this.paymentForm.value.numCheque;
    paydata.refDoc = this.paymentForm.value.refDoc;
    paydata.note = this.paymentForm.value.note;
    paydata.bankId = this.paymentForm.value.bankid;
    this.paymentService.addFinOp(paydata).subscribe(() => {
      this.alertify.success('paiment effectué avec succès');
      this.router.navigate(['/tuitions']);
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
