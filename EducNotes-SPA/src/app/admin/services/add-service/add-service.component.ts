import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Utils } from 'src/app/shared/utils';
import { AdminService } from 'src/app/_services/admin.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClassService } from 'src/app/_services/class.service';
import { ProductService } from 'src/app/_services/product.service';

@Component({
  selector: 'app-add-service',
  templateUrl: './add-service.component.html',
  styleUrls: ['./add-service.component.scss']
})
export class AddServiceComponent implements OnInit {
  product: any;
  serviceForm: FormGroup;
  editionMode = false;
  typeOptions = [];
  trueFalseOptions = [{value: 0, label: 'NON'}, {value: 1, label: 'OUI'}];
  myDatePickerOptions = Utils.myDatePickerOptions;
  wait = false;
  levelProducts: any;
  showLevels = false;
  showUniquePrice = false;
  showDueDates = false;

  constructor(private fb: FormBuilder, private alertify: AlertifyService, private productService: ProductService,
    private route: ActivatedRoute, private classService: ClassService, private adminService: AdminService,
    private router: Router) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.product = data['product'];
      // console.log(this.product);
      if (this.product) {
        this.editionMode = true;
        if (this.product.isByLevel) {
          this.showLevels = true;
          this.showUniquePrice = false;
        }
        if (!this.product.isPaidCash) {
          this.showDueDates = true;
        }
      } else {
        this.initValues();
      }
      this.createServiceForm();
      for (let i = 0; i < this.product.dueDates.length; i++) {
        const elt = this.product.dueDates[i];
        this.addDueDateItem(elt.id, elt.productId, elt.strDueDate, elt.deadLineName, elt.percentage * 100);
      }
    });

    this.getProductTypes();
    this.getClassLevels();
  }

  createServiceForm() {
    const paidCash = this.product.isPaidCash ? 1 : 0;
    const byLevel = this.product.isByLevel ? 1 : 0;
    this.serviceForm = this.fb.group({
      name: [this.product.name, Validators.required],
      typeId: [this.product.productTypeId, Validators.required],
      price: [this.product.price],
      isPaidCash: [paidCash, Validators.required],
      isByLevel: [byLevel],
      duedates: this.fb.array([]),
      levelPrices: this.fb.array([])
    }, {validators: this.formValidator});
  }

  formValidator(g: FormGroup) {
    let totalPct = 0;
    const duedates = g.get('duedates').value;
    const levelPrices = g.get('levelPrices').value;
    const prodPrice = g.get('price').value;
    const isByLevel = g.get('isByLevel').value;
    let formNOK = false;

    // check classlevel product fees
    let feesNOK = false;
    let feeNOK = false;
    if (isByLevel === 1) {
      for (let i = 0; i < levelPrices.length; i++) {
        const fee = levelPrices[i].price;
        console.log(fee);
        if (fee === null || fee === '' || fee === 0) {
          feesNOK = true;
          formNOK = true;
          break;
        }
      }
    } else {
      if (prodPrice <= 0) {
        feeNOK = true;
        formNOK = true;
      }
    }

    // check the percentage
    let pctNOK = false;
    let datesNOK = false;
    let labelsNOK = false;
  // let pct0NOK = false;
    for (let i = 0; i < duedates.length; i++) {
      const pct = duedates[i].pct;
      totalPct += pct;
      if (pct <= 0) {
        pctNOK = true;
        formNOK = true;
      }
      // const seq = duedates[i].seq;
      // totalSeq += seq;

      const date = duedates[i].dueDate;
      if (!date) {
        formNOK = true;
        datesNOK = true;
      }

      const label = duedates[i].deadlineName;
      if (!label) {
        formNOK = true;
        labelsNOK = true;
      }
    }
    if (totalPct !== 100) {
      pctNOK = true;
      formNOK = true;
    }

    // checks sequences sum : n(n + 1)/2
    // let seqNOK = false;
    // const n = duedates.length;
    // const sum = n * (n + 1) / 2;
    // if (sum !== totalSeq) {
    //   seqNOK = true;
    //   formNOK = true;
    // } else {

    // }

    return {'formNOK': formNOK, 'feesNOK': feesNOK, 'feeNOK': feeNOK, 'pctNOK': pctNOK, 'datesNOK': datesNOK, 'labelsNOK': labelsNOK}; // , 'seqNOK': seqNOK};
  }

  addDueDateItem(id, prodid, duedate, deadlinename, pct): void {
    const duedates = this.serviceForm.get('duedates') as FormArray;
    duedates.push(this.createDueDateItem(id, prodid, duedate, deadlinename, pct));
  }

  createDueDateItem(id, prodid, duedate, deadlinename, pct): FormGroup {
    return this.fb.group({
      id: [id],
      productId: [prodid],
      dueDate: [duedate, Validators.required],
      deadlineName: [deadlinename, Validators.required],
      // seq: [seq, Validators.required],
      pct: [pct, Validators.required],
      toBeDeleted: [false]
    });
  }

  removeDueDateItem(index) {
    const duedates = this.serviceForm.get('duedates') as FormArray;
    duedates.removeAt(index);
    // const dates = this.serviceForm.value.duedates;
    // for (let i = 0; i < dates.length; i++) {
    //   console.log(this.serviceForm.value.duedates[i].seq);
    //   duedates.at(index).get('dueDate').setValue(i + 1);
    // }
    this.sortDueDates();
  }

  resetDueDateItem(index) {
    const coursetypes = this.serviceForm.get('duedates') as FormArray;
    coursetypes.at(index).get('toBeDeleted').setValue(false);
  }

  addLevelPriceItem(levelid, levelname, price): void {
    const prices = this.serviceForm.get('levelPrices') as FormArray;
    prices.push(this.createLevelPriceItem(levelid, levelname, price));
  }

  createLevelPriceItem(levelid, levelname, price): FormGroup {
    return this.fb.group({
      levelId: levelid,
      levelname: levelname,
      price: [price, Validators.required]
    });
  }

  initValues() {
    this.product = {
      id: 0,
      name: '',
      typeiI: null,
      isPaidCash: true,
      price: null,
      dueDates: null
    };
  }

  togglePaidCash(value) {
    if (value) {
      this.showDueDates = false;
      this.removeAllDueDates();
    } else {
      this.showDueDates = true;
      this.addDueDate();
    }
  }

  toggleByLevel(value) {
    if (value) {
      this.showLevels = true;
      this.showUniquePrice = false;
    } else {
      this.showLevels = false;
      this.showUniquePrice = true;
    }
  }

  addDueDate() {
    const duedates = this.serviceForm.get('duedates').value;
    this.addDueDateItem(0, this.product.id, null, '', 0);
  }

  getProductTypes() {
    this.productService.getProductTypes().subscribe((data: any) => {
      for (let i = 0; i < data.length; i++) {
        const elt = data[i];
        const type = {value: elt.id, label: elt.name};
        this.typeOptions = [...this.typeOptions, type];
      }
    }, () => {
      this.alertify.error('problème pour récupérer les données');
    });
  }

  getClassLevels() {
    this.classService.getLevelProductPrices(this.product.id).subscribe(data => {
      this.levelProducts = data;
      for (let i = 0; i < this.levelProducts.length; i++) {
        const elt = this.levelProducts[i];
        this.addLevelPriceItem(elt.classLevelId, elt.levelName, elt.price);
      }
    }, () => {
      this.alertify.error('problème pour récupérer les données');
    });
  }

  removeAllDueDates() {
    const dueDates = this.serviceForm.get('duedates') as FormArray;
    while (dueDates.length > 0) {
      dueDates.removeAt(0);
    }
  }

  sortDueDates() {
    let duedatesOK = true;
    const dates = this.serviceForm.value.duedates;
    for (let i = 0; i < dates.length; i++) {
      const elt = dates[i];
      if (!elt.dueDate) {
        duedatesOK = false;
        break;
      }
    }

    if (duedatesOK) {
      const dueDates = this.serviceForm.value.duedates.sort((a, b) => {
        const date1 = a.dueDate.split('/');
        const date2 = b.dueDate.split('/');
        const num1 = Number(date1[2] + date1[1] + date1[0]);
        const num2 = Number(date2[2] + date2[1] + date2[0]);
        return num1 < num2 ? -1 : 1;
      });
      this.removeAllDueDates();
      for (let i = 0; i < dueDates.length; i++) {
        const elt = dueDates[i];
        this.addDueDateItem(elt.id, this.product.id, elt.dueDate, elt.deadlineName, elt.pct);
      }
    }
  }

  saveService() {
    this.wait = true;
    let levelPrices = [];
    let dueDates = [];
    const prodid = this.product.id;

    for (let i = 0; i < this.serviceForm.value.levelPrices.length; i++) {
      const elt = this.serviceForm.value.levelPrices[i];
      const levelPrice = {id: 0, productId: prodid, classLevelId: elt.levelId, price: elt.price};
      levelPrices = [...levelPrices, levelPrice];
    }

    for (let i = 0; i < this.serviceForm.value.duedates.length; i++) {
      const elt = this.serviceForm.value.duedates[i];
      const dueDate = {id: elt.id, productId: prodid, strDueDate: elt.dueDate, deadLineName: elt.deadlineName,
        percentage: elt.pct};
      dueDates = [...dueDates, dueDate];
    }

    const service = <any>{};
    service.id = prodid;
    service.name = this.serviceForm.value.name;
    service.productTypeId = this.serviceForm.value.typeId;
    service.isPaidCash = this.serviceForm.value.isPaidCash === 1 ? true : false;
    service.isByLevel = this.serviceForm.value.isByLevel === 1 ? true : false;
    service.levelPrices = levelPrices;
    service.dueDates = dueDates;
    // console.log(service);
    this.adminService.saveService(service).subscribe(() => {
      this.wait = false;
      this.router.navigate(['services']);
      this.alertify.success('les données ont été enregistrées');
    }, error => {
      this.alertify.error(error);
      this.wait = false;
    });
  }

}
