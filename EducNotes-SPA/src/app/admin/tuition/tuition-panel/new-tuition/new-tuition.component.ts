import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClassService } from 'src/app/_services/class.service';
import { CollapseComponent } from 'ng-uikit-pro-standard';
import { OrderService } from 'src/app/_services/order.service';
import { AuthService } from 'src/app/_services/auth.service';
import { Setting } from 'src/app/_models/setting';
import { environment } from 'src/environments/environment.prod';

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

  constructor(private fb: FormBuilder, private alertify: AlertifyService, private authService: AuthService,
    private classService: ClassService, private orderService: OrderService) { }

  ngOnInit() {
    this.settings = this.authService.settings;
    this.regFee = Number(this.settings.find(s => s.name === 'RegFee').value);
    this.regDeadline = this.settings.find(s => s.name === 'RegistrationDeadLine').value;
    this.createTuitionForm();
    this.addChildItem('moulot', 'sandrine', null, '', 10);
    this.addChildItem('moulot', 'inna', null, '', 3);
    this.getClassLevels();
    this.getTuitionData();
  }

  getTuitionData() {
    this.orderService.getTuitionData().subscribe((data: any) => {
      const deadlines = data.deadlines;
      this.deadline = deadlines.find(d => d.productId === this.tuitionId && d.seq === 1);
      console.log(this.deadline);
      this.dpPct = this.deadline.percentage;
      this.classProducts = data.classProducts;
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
      fLastName: ['moulot'],
      fFirstName: ['georges'],
      fEmail: ['georges.moulot@albatrostechnologies.com'],
      fCell: ['07.10.44.46'],
      mLastName: ['moulot'],
      mFirstName: ['Jacqueline'],
      mEmail: ['jacqueline.moulot@manager.com'],
      mCell: ['12345678'],
      children: this.fb.array([])
    });
  }

  addChildItem(lname, fname, classlevel, dob, sex): void {
    const children = this.tuitionForm.get('children') as FormArray;
    children.push(this.createChildItem(lname, fname, classlevel, dob, sex));
    this.nbChildren++;
    this.classnames = [...this.classnames, ''];
    this.tuitionFee = [...this.tuitionFee, 0];
    this.downPayment = [...this.downPayment, 0];
  }

  createChildItem(lname, fname, classlevel, dob, sex): FormGroup {
    return this.fb.group({
      lname: lname,
      fname: fname,
      classlevel: classlevel,
      dob: dob,
      sex: sex
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
          // debugger;
          this.childrenDiv.hide();
          this.sumchildrenDiv.show();
          // this.checkoutDiv.hide();
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
          // this.checkoutDiv.hide();
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
  }

  setAmountDue() {
    this.amountDue = 0;
    for (let i = 0; i < this.tuitionFee.length; i++) {
      const dp = this.downPayment[i];
      this.amountDue = this.amountDue + Number(dp) + this.regFee;
    }
  }

  saveTuition() {

  }

}
