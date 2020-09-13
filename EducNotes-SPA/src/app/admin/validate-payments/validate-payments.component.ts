import { Component, OnInit } from '@angular/core';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute, Router } from '@angular/router';
import { PaymentService } from 'src/app/_services/payment.service';
import { FormBuilder, FormGroup, FormArray } from '@angular/forms';

@Component({
  selector: 'app-validate-payments',
  templateUrl: './validate-payments.component.html',
  styleUrls: ['./validate-payments.component.scss']
})
export class ValidatePaymentsComponent implements OnInit {
  payments: any;
  paymentsForm: FormGroup;
  statusOptions: any[] = [];

  constructor(private fb: FormBuilder, private alertify: AlertifyService,
    private router: Router, private paymentService: PaymentService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.createPaymentsForm();
    this.route.data.subscribe(data => {
      this.payments = data['payments'];
      // console.log(this.payments);
      for (let i = 0; i < this.payments.length; i++) {
        const pay = this.payments[i];

        // set the product name : tuition and children names
        const linepays = pay.linePayments;
        let prodname = '';
        for (let j = 0; j < linepays.length; j++) {
          const lpay = linepays[j];
          if (prodname === '') {
            prodname += lpay.productName + ' de ' + lpay.childFirstName;
          } else {
            prodname += ' et ' + lpay.childFirstName;
          }
        }
        this.addPaymentItem(pay.id, pay.amount, pay.strFinOpDate, pay.paymentTypeName, prodname, pay.chequeNum,
          pay.chequeBankName, pay.docRef, pay.fromBankName, pay.received, pay.depositedToBank, pay.rejected, pay.cashed);
      }
    });
  }

  createPaymentsForm() {
    this.paymentsForm = this.fb.group({
      payments: this.fb.array([])
    });
  }

  addPaymentItem(id, amnt, finOpDate, payType, prodname, chqnum, chqBankName, docRef,
    fromBank, received, toBank, rejected, cashed): void {
    const payments = this.paymentsForm.get('payments') as FormArray;
    payments.push(this.createPaymentItem(id, amnt, finOpDate, payType, prodname, chqnum, chqBankName,
      docRef, fromBank, received, toBank, rejected, cashed));
  }

  createPaymentItem(id, amnt, finOpDate, payType, prodname, chqnum, chqBankName, docRef,
    fromBank, received, toBank, rejected, cashed): FormGroup {
    return this.fb.group({
      id: id,
      amount: amnt,
      date: finOpDate,
      payType: payType,
      prodname: prodname,
      chequenum: chqnum,
      chqBankName: chqBankName,
      docRef: docRef,
      fromBank: fromBank,
      received: received,
      toBank: toBank,
      rejected: rejected,
      cashed: cashed,
      status: false
    });
  }

  validatePayments() {

  }

}
