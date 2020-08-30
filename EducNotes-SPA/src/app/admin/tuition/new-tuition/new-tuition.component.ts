import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray, FormControl } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClassService } from 'src/app/_services/class.service';
import { CollapseComponent } from 'ng-uikit-pro-standard';
import { OrderService } from 'src/app/_services/order.service';
import { AuthService } from 'src/app/_services/auth.service';
import { Setting } from 'src/app/_models/setting';
import { environment } from 'src/environments/environment.prod';
import { Utils } from 'src/app/shared/utils';
import { TuitionData } from 'src/app/_models/tuitionData';
import { TuitionChildData } from 'src/app/_models/tuitionChildData';
import { Router } from '@angular/router';
import { PaymentService } from 'src/app/_services/payment.service';

@Component({
  selector: 'app-new-tuition',
  templateUrl: './new-tuition.component.html',
  styleUrls: ['./new-tuition.component.scss']
})
export class NewTuitionComponent implements OnInit {
  @ViewChild('parents', { static: false}) parentsDiv: CollapseComponent;
  @ViewChild('sumparents', {static: false}) sumparentsDiv: CollapseComponent;
  @ViewChild('children', {static: false}) childrenDiv: CollapseComponent;
  @ViewChild('sumchildren', {static: false}) sumchildrenDiv: CollapseComponent;
  @ViewChild('checkout', {static: false}) checkoutDiv: CollapseComponent;
  payCash = environment.payCash;
  payCheque = environment.payCheque;
  payWire = environment.payWire;
  tuitionForm: FormGroup;
  paymentForm: FormGroup;
  levels: any;
  paytypes: any;
  levelOptions: any[] = [];
  sexOptions = [{value: 0, label: 'femme'}, {value: 1, label: 'homme'}];
  nbChildren = 0;
  childDivOn = false;
  checkoutDivOn = false;
  classnames: string[] = [];
  deadline: any;
  classProducts: any;
  settings: Setting[];
  amountDue = 0;
  tuitionFee: number[] = [];
  regFee: number;
  regDeadline: any;
  dpPct: number;
  downPayment: number[] = [];
  tuitionId = environment.tuitionId;
  birthDateMask = Utils.birthDateMask;
  phoneMask = Utils.phoneMask;
  today: string;
  tuitionValidity: string;
  daysToValidateReg: any;
  orderAmount: number;
  totalRegFee: number;
  tuitionData = <TuitionData>{};
  showTuitionForm = true;
  showPaymentForm = false;
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

  constructor(private fb: FormBuilder, private alertify: AlertifyService, private router: Router,
    private authService: AuthService, private classService: ClassService, private orderService: OrderService,
    private paymentService: PaymentService) { }

  ngOnInit() {
    this.createTuitionForm();
    this.createPaymentForm();
    this.settings = this.authService.settings;
    this.regFee = Number(this.settings.find(s => s.name === 'RegFee').value);
    this.regDeadline = this.settings.find(s => s.name === 'RegistrationDeadLine').value;
    this.daysToValidateReg = this.settings.find(s => s.name === 'DaysToValidateReg').value;
    this.addChildItem('', '', null, '', null);
    this.getClassLevels();
    this.getTuitionData();
    this.getPaymentData();

  }

  getTuitionData() {
    this.orderService.getTuitionData().subscribe((data: any) => {
      const deadlines = data.deadlines;
      this.deadline = deadlines.find(d => d.productId === this.tuitionId && d.seq === 1);
      this.dpPct = this.deadline.percentage;
      this.classProducts = data.classProducts;
      this.today = data.today;
      this.tuitionValidity = data.tuitionValidity;
    }, error => {
      this.alertify.error(error);
    });
  }

  getClassLevels() {
    this.classService.getLevels().subscribe(data => {
      this.levels = data;
      for (let i = 0; i < this.levels.length; i++) {
        const elt = this.levels[i];
        const level = {value: elt.id, label: elt.name};
        this.levelOptions = [...this.levelOptions, level];
      }
    }, error => {
      this.alertify.error(error);
    });
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

  createTuitionForm() {
    this.tuitionForm = this.fb.group({
      fLastName: ['', Validators.required],
      fFirstName: ['', Validators.required],
      fEmail: ['', Validators.required],
      fCell: ['', Validators.required],
      mLastName: ['', Validators.required],
      mFirstName: ['', Validators.required],
      mEmail: ['', Validators.required],
      mCell: ['', Validators.required],
      children: this.fb.array([])
    }, {validator: this.formValidator});
  }

  formValidator(g: FormGroup) {
    const children = g.get('children') as FormArray;
    let childerror = false;
    for (let i = 0; i < children.length; i++) {
      const elt = children.controls[i];
      if (elt.value.lname === '' || elt.value.fname === '' || elt.value.classlevelId === null || elt.value.dob === '' ||
        elt.value.sex === null) {
        childerror = true;
        break;
      }
    }

    let parenterror = false;
    const flname = g.get('fLastName').value;
    const ffname = g.get('fFirstName').value;
    const fcell = g.get('fCell').value;
    const femail = g.get('fEmail').value;
    const mlname = g.get('mLastName').value;
    const mfname = g.get('mFirstName').value;
    const mcell = g.get('mCell').value;
    const memail = g.get('mEmail').value;
    if (flname === '' && ffname === '' && fcell === '' && femail === '' && mlname === ''
      && mfname === '' && mcell === '' && memail === '') {
      parenterror = true;
    }

    // is the father OK?
    let fathererror = false;
    if (flname !== '' || ffname !== '' || fcell !== '' || femail !== '') {
      if (flname === '' || ffname === '' || fcell === '' || femail === '') {
        fathererror = true;
        // parenterror = true;
      }
    }

    // is the mother OK?
    let mothererror = false;
    if (mlname !== '' || mfname !== '' || mcell !== '' || memail !== '') {
      if (mlname === '' || mfname === '' || mcell === '' || memail === '') {
        mothererror = true;
        // parenterror = true;
      }
    }

    // did we select at least one parent to cope with the registration?
    // let emailerror = false;
    // if (fsendemail === false && msendemail === false) {
    //   emailerror = true;
    //   parenterror = true;
    // }

    if (childerror === false && parenterror === true) {
      return {'childNOK': false, 'parentNOK': true, 'formNOK': true,
        'fatherNOK': fathererror, 'motherNOK': mothererror}; // , 'sendemailNOK': emailerror};
    } else if (childerror === true && parenterror === false) {
      return {'childNOK': true, 'parentNOK': false, 'formNOK': true,
        'fatherNOK': fathererror, 'motherNOK': mothererror}; // , 'sendemailNOK': emailerror};
    } else if (childerror === true && parenterror === true) {
      return {'childNOK': true, 'parentNOK': true, 'formNOK': true,
        'fatherNOK': fathererror, 'motherNOK': mothererror}; // , 'sendemailNOK': emailerror};
    }

    return null;
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

    // let docerror = false;
    // const refDoc = g.get('refDoc').value;
    // if ((paytype !== '' && refDoc === '') || (paytype === '' && refDoc !== '')) {
    //   docerror = true;
    // }

    if (chequeerror === true) {
      return {'chequeNOK': true};
    }

    return null;
  }

  addChildItem(lname, fname, classlevelId, dob, sex): void {
    const children = this.tuitionForm.get('children') as FormArray;
    children.push(this.createChildItem(lname, fname, classlevelId, dob, sex));
    this.nbChildren++;
    this.classnames = [...this.classnames, ''];
    this.tuitionFee = [...this.tuitionFee, 0];
    this.downPayment = [...this.downPayment, 0];
  }

  createChildItem(lname, fname, classlevelId, dob, sex): FormGroup {
    return this.fb.group({
      lname: [lname, Validators.required],
      fname: [fname, Validators.required],
      classlevelId: [classlevelId, Validators.required],
      dob: [dob, Validators.required],
      sex: [sex, Validators.required]
    });
  }

  removeChildItem(index) {
    const children = this.tuitionForm.get('children') as FormArray;
    children.removeAt(index);
    this.nbChildren--;
    this.classnames.splice(index, 1);
    this.tuitionFee.splice(index, 1);
    this.downPayment.splice(index, 1);
    this.setAmountDue();
    this.setOrderAmount();
  }

  addChild() {
    this.addChildItem('', '', null, '', null);
  }

  openDiv(index) {
    switch (index) {
      case 0:
        this.parentsDiv.show();
        this.sumparentsDiv.hide();
        Promise.resolve().then(() => {
          this.childrenDiv.hide();
          this.sumchildrenDiv.show();
          this.checkoutDivOn = false;
        });
        break;
      case 1:
        this.childDivOn = true;
        this.parentsDiv.hide();
        this.sumparentsDiv.show();
        Promise.resolve().then(() => {
          this.childrenDiv.show();
          this.sumchildrenDiv.hide();
          this.checkoutDivOn = false;
        });
        break;
      case 2:
        this.checkoutDivOn = true;
        this.sumchildrenDiv.show();
        Promise.resolve().then(() => {
          this.parentsDiv.hide();
          this.sumparentsDiv.show();
          this.childrenDiv.hide();
        });
        break;
      default:
        break;
    }
  }

  setClassLevel(value, index) {
    const clname = (this.levelOptions.find(c => c.value === value)).label;
    this.classnames[index] = clname;
    // set child figures and update his/her amount and finally amount due
    const price = this.classProducts.find(c => c.productId === this.tuitionId && c.classLevelId === value).price;
    this.tuitionFee[index] = price;
    const dp = this.dpPct * price;
    this.downPayment[index] = dp;
    this.setAmountDue();
    this.setOrderAmount();
  }

  setAmountDue() {
    this.amountDue = 0;
    for (let i = 0; i < this.downPayment.length; i++) {
      const dp = this.downPayment[i];
      this.amountDue = this.amountDue + Number(dp) + this.regFee;
    }
  }

  setOrderAmount() {
    this.orderAmount = 0;
    this.totalRegFee = 0;
    for (let i = 0; i < this.tuitionFee.length; i++) {
      const fee = this.tuitionFee[i];
      this.orderAmount = this.orderAmount + Number(fee) + this.regFee;
      this.totalRegFee = this.totalRegFee + this.regFee;
    }
  }

  saveTuition() {
    this.setTuitionData();
    this.orderService.addNewTuition(this.tuitionData).subscribe(() => {
      this.router.navigate(['/tuition']);
      this.alertify.success('l\'inscription a bien été enregistrée');
    }, error => {
      this.alertify.error(error);
    });
  }

  saveTuitionAndPayment() {
    this.setTuitionData();
    this.orderService.addNewTuition(this.tuitionData).subscribe((data: any) => {
      this.alertify.success('l\'inscription a bien été enregistrée');
      this.showTuitionForm = false;
      this.showPaymentForm = true;
      this.orderid = data.orderId;
      this.invoiceid = data.invoiceId;
    }, error => {
      this.alertify.error(error);
    }, () => {
      const amount = this.paymentForm.get('amount') as FormControl;
      amount.setValue(this.tuitionData.dueAmount);
      this.showTuitionForm = false;
      this.showPaymentForm = true;
    });
}

  savePayment() {
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
    });
  }

  setTuitionData() {
    this.tuitionData.fLastName = this.tuitionForm.value.fLastName;
    this.tuitionData.fFirstName = this.tuitionForm.value.fFirstName;
    this.tuitionData.fCell = this.tuitionForm.value.fCell.replace(/\./g, '');
    this.tuitionData.fEmail = this.tuitionForm.value.fEmail;
    // tuitionData.fSendEmail = this.tuitionForm.value.fSendEmail;
    this.tuitionData.fActive = this.tuitionForm.hasError('fathererror') === true ? false : true;
    this.tuitionData.mLastName = this.tuitionForm.value.mLastName;
    this.tuitionData.mFirstName = this.tuitionForm.value.mFirstName;
    this.tuitionData.mCell = this.tuitionForm.value.mCell.replace(/\./g, '');
    this.tuitionData.mEmail = this.tuitionForm.value.mEmail;
    // tuitionData.mSendEmail = this.tuitionForm.value.mSendEmail;
    this.tuitionData.mActive = this.tuitionForm.hasError('mothererror') === true ? false : true;
    this.tuitionData.orderAmount = this.orderAmount;
    this.tuitionData.dueAmount = this.amountDue;
    this.tuitionData.deadline = Utils.inputDateDDMMYY(this.regDeadline, '/');
    this.tuitionData.validity = Utils.inputDateDDMMYY(this.tuitionValidity, '/');
    this.tuitionData.children = [];
    for (let i = 0; i < this.tuitionForm.value.children.length; i++) {
      const elt = this.tuitionForm.value.children[i];
      const child = <TuitionChildData>{};
      child.lastName = elt.lname;
      child.firstName = elt.fname;
      child.classlevelId = elt.classlevelId;
      child.dateOfBirth = Utils.inputDateDDMMYY(elt.dob, '/');
      child.sex = elt.sex;
      child.tuitionFee = this.tuitionFee[i];
      child.regFee = this.regFee;
      child.downPayment = this.downPayment[i];
      this.tuitionData.children = [...this.tuitionData.children, child];
    }
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
