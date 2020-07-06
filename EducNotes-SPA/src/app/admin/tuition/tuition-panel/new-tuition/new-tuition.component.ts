import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
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
  tuitionForm: FormGroup;
  levels: any;
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

  constructor(private fb: FormBuilder, private alertify: AlertifyService, private router: Router,
    private authService: AuthService, private classService: ClassService,
    private orderService: OrderService) { }

  ngOnInit() {
    this.createTuitionForm();
    this.settings = this.authService.settings;
    this.regFee = Number(this.settings.find(s => s.name === 'RegFee').value);
    this.regDeadline = this.settings.find(s => s.name === 'RegistrationDeadLine').value;
    this.daysToValidateReg = this.settings.find(s => s.name === 'DaysToValidateReg').value;
    this.addChildItem('moulot', 'sandrine', 10, '31/06/2006', 0);
    this.addChildItem('moulot', 'inna', 3, '14/06/2011', 0);
    this.getClassLevels();
    this.getTuitionData();
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

  createTuitionForm() {
    this.tuitionForm = this.fb.group({
      fLastName: ['moulot', Validators.required],
      fFirstName: ['georges', Validators.required],
      fEmail: ['gmoulot@hotmail.com', Validators.required],
      fCell: ['12345678', Validators.required],
      // fSendEmail: [false],
      mLastName: ['moulot', Validators.required],
      mFirstName: ['jacqueline', Validators.required],
      mEmail: ['jm@hotmail.com', Validators.required],
      mCell: ['34566554', Validators.required],
      // mSendEmail: [false],
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
    const tuitionData = <TuitionData>{};
    tuitionData.fLastName = this.tuitionForm.value.fLastName;
    tuitionData.fFirstName = this.tuitionForm.value.fFirstName;
    tuitionData.fCell = this.tuitionForm.value.fCell;
    tuitionData.fEmail = this.tuitionForm.value.fEmail;
    // tuitionData.fSendEmail = this.tuitionForm.value.fSendEmail;
    tuitionData.fActive = this.tuitionForm.hasError('fathererror') === true ? false : true;
    tuitionData.mLastName = this.tuitionForm.value.mLastName;
    tuitionData.mFirstName = this.tuitionForm.value.mFirstName;
    tuitionData.mCell = this.tuitionForm.value.mCell;
    tuitionData.mEmail = this.tuitionForm.value.mEmail;
    // tuitionData.mSendEmail = this.tuitionForm.value.mSendEmail;
    tuitionData.mActive = this.tuitionForm.hasError('mothererror') === true ? false : true;
    tuitionData.orderAmount = this.orderAmount;
    tuitionData.dueAmount = this.amountDue;
    tuitionData.deadline = Utils.inputDateDDMMYY(this.regDeadline, '/');
    tuitionData.validity = Utils.inputDateDDMMYY(this.tuitionValidity, '/');
    tuitionData.children = [];
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
      tuitionData.children = [...tuitionData.children, child];
    }

    this.orderService.addNewTuition(tuitionData).subscribe(() => {
      this.router.navigate(['/tuition']);
      this.alertify.success('l\'inscription a bien été enregistré');
    }, error => {
      this.alertify.error(error);
    });
  }

}