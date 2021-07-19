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
  priceByOptions = [{value: 0, label: 'Unique'}, {value: 1, label: 'Par niveaux'}, {value: 2, label: 'Par zones'}];
  myDatePickerOptions = Utils.myDatePickerOptions;
  wait = false;
  levelProducts: any;
  showLevels = false;
  showZones = false;
  showUniquePrice = false;
  showDueDates = false;
  showPeriodicities = false;
  zones: any;
  periodicities: any;
  payableAts: any;
  periodicityOptions = [];
  payableAtOptions = [];

  constructor(private fb: FormBuilder, private alertify: AlertifyService, private productService: ProductService,
    private route: ActivatedRoute, private classService: ClassService, private adminService: AdminService,
    private router: Router) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.product = data['product'];
      if (this.product) {
        this.editionMode = true;
        if (this.product.isByLevel) {
          this.showLevels = true;
          this.showUniquePrice = false;
          this.showZones = false;
        }
        if (!this.product.isPaidCash) {
          this.showDueDates = true;
          this.showZones = false;
          this.showUniquePrice = false;
        }
        if (this.product.isByZone) {
          this.showZones = true;
          this.showDueDates = false;
          this.showUniquePrice = false;
        }
        if (this.product.isPeriodic) {
          this.showPeriodicities = true;
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
    this.getZonePrices();
    this.getProductData();
  }

  createServiceForm() {
    const paidCash = this.product.isPaidCash ? 1 : 0;
    const priceBy = this.product.isByLevel ? 1 : this.product.isByZone ? 2 : 0;
    this.serviceForm = this.fb.group({
      name: [this.product.name, Validators.required],
      typeId: [this.product.productTypeId, Validators.required],
      periodicityId: [this.product.periodicityId],
      isPeriodic: [this.product.isPeriodic],
      startDate: [this.product.serviceStartDate],
      price: [this.product.price],
      isPaidCash: [paidCash, Validators.required],
      priceBy: [priceBy],
      duedates: this.fb.array([]),
      levelPrices: this.fb.array([]),
      zones: this.fb.array([])
    }, {validators: this.formValidator});
  }

  formValidator(g: FormGroup) {
    let totalPct = 0;
    const duedates = g.get('duedates').value;
    const levelPrices = g.get('levelPrices').value;
    const zones = g.get('zones').value;
    const prodPrice = g.get('price').value;
    const priceBy = g.get('priceBy').value;
    let formNOK = false;

    // check classlevel product fees
    let feesNOK = false;
    let feeNOK = false;
    if (priceBy === 1) {
      for (let i = 0; i < levelPrices.length; i++) {
        const fee = levelPrices[i].price;
        if (fee === null || fee === '' || fee === 0) {
          feesNOK = true;
          formNOK = true;
          break;
        }
      }
    } else if (priceBy === 0) {
      if (prodPrice <= 0) {
        feeNOK = true;
        formNOK = true;
      }
    }

    // check zone prices
    let zonesNOK = false;
    if (priceBy === 2) {
      for (let i = 0; i < zones.length; i++) {
        const fee = zones[i].price;
        if (fee === null || fee === '' || fee === 0) {
          zonesNOK = true;
          formNOK = true;
          break;
        }
      }
    }

    // check the percentage
    let pctNOK = false;
    let datesNOK = false;
    let labelsNOK = false;
    for (let i = 0; i < duedates.length; i++) {
      const pct = duedates[i].pct;
      totalPct += pct;
      if (pct <= 0) {
        pctNOK = true;
        formNOK = true;
      }

      const date = duedates[i].dueDate;
      if (!date) {
        formNOK = true;
        datesNOK = true;
      }

      const label = duedates[i].deadlineName;
      if (!label || label === '') {
        formNOK = true;
        labelsNOK = true;
      }
    }

    if (duedates.length > 1 && totalPct !== 100) {
      pctNOK = true;
      formNOK = true;
    }

    const isPeriodic = g.get('isPeriodic').value;
    const periodicityId = g.get('periodicityId').value;
    let periodicityNOK = false;
    if (isPeriodic === true && !periodicityId)
    {
      periodicityNOK = true;
      formNOK = true;
    }

    return {'formNOK': formNOK, 'feesNOK': feesNOK, 'feeNOK': feeNOK, 'pctNOK': pctNOK,
            'datesNOK': datesNOK, 'labelsNOK': labelsNOK, 'zonesNOK': zonesNOK, 'periodicityNOK': periodicityNOK};
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
      pct: [pct, Validators.required],
      toBeDeleted: [false]
    });
  }

  removeDueDateItem(index) {
    const duedates = this.serviceForm.get('duedates') as FormArray;
    duedates.removeAt(index);
    this.sortDueDates();
  }

  resetDueDateItem(index) {
    const coursetypes = this.serviceForm.get('duedates') as FormArray;
    coursetypes.at(index).get('toBeDeleted').setValue(false);
  }

  addZonePriceItem(zoneid, name, price): void {
    const zones = this.serviceForm.get('zones') as FormArray;
    zones.push(this.createZonePriceItem(zoneid, name, price));
  }

  createZonePriceItem(zoneid, name, price): FormGroup {
    return this.fb.group({
      zoneId: zoneid,
      zoneName: name,
      price: price
    });
  }

  removeZoneItem(index) {
    const zones = this.serviceForm.get('zones') as FormArray;
    zones.removeAt(index);
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

  togglePeriodic(value) {
    if (value.checked) {
      this.showPeriodicities = true;
    } else {
      this.showPeriodicities = false;
      this.serviceForm.get('periodicityId').setValue('');
    }
  }

  togglePaidCash(value) {
    if (value === 1) {
      this.showDueDates = false;
      this.removeAllDueDates();
    } else {
      this.showDueDates = true;
      const dueDates = this.serviceForm.get('duedates') as FormArray;
      if(dueDates.length === 0) {
        this.addDueDate();
      }
      this.serviceForm.get('isPeriodic').setValue(false);
      this.serviceForm.get('periodicityId').setValue('');
      this.showPeriodicities = false;
    }
  }

  togglePriceBy(value) {
    if (value === 1) {
      this.showLevels = true;
      this.showUniquePrice = false;
      this.showZones = false;
    } else if (value === 2) {
      this.showZones = true;
      this.showLevels = false;
      this.showUniquePrice = false;
    } else {
      this.showUniquePrice = true;
      this.showLevels = false;
      this.showZones = false;
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
    this.productService.getLevelPrices(this.product.id).subscribe(data => {
      this.levelProducts = data;
      for (let i = 0; i < this.levelProducts.length; i++) {
        const elt = this.levelProducts[i];
        this.addLevelPriceItem(elt.classLevelId, elt.levelName, elt.price);
      }
    }, () => {
      this.alertify.error('problème pour récupérer les données');
    });
  }

  getZonePrices() {
    this.productService.getZonePrices(this.product.id).subscribe(data => {
      this.zones = data;
      for (let i = 0; i < this.zones.length; i++) {
        const elt = this.zones[i];
        this.addZonePriceItem(elt.zoneId, elt.zoneName, elt.price);
      }
    }, () => {
      this.alertify.error('problème pour récupérer les données');
    });
  }

  getProductData() {
    this.productService.getProductData().subscribe((data: any) => {
      this.periodicities = data.periodicities;
      for (let i = 0; i < this.periodicities.length; i++) {
        const elt = this.periodicities[i];
        const periodicity = {value: elt.id, label: elt.name};
        this.periodicityOptions = [...this.periodicityOptions, periodicity];
      }
      this.payableAts = data.payableAts;
      for (let i = 0; i < this.payableAts.length; i++) {
        const elt = this.payableAts[i];
        const payableAt = {value: elt.id, label: elt.name};
        this.payableAtOptions = [...this.payableAtOptions, payableAt];
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
    let zonePrices = [];
    let dueDates = [];
    const prodid = this.product.id;

    for (let i = 0; i < this.serviceForm.value.levelPrices.length; i++) {
      const elt = this.serviceForm.value.levelPrices[i];
      const levelPrice = {id: 0, productId: prodid, classLevelId: elt.levelId, price: elt.price};
      levelPrices = [...levelPrices, levelPrice];
    }

    for (let i = 0; i < this.serviceForm.value.zones.length; i++) {
      const elt = this.serviceForm.value.zones[i];
      const zonePrice = {id: 0, productId: prodid, zoneId: elt.zoneId, price: elt.price};
      zonePrices = [...zonePrices, zonePrice];
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
    service.periodicityId = this.serviceForm.value.isPeriodic ? this.serviceForm.value.periodicityId : null;
    service.isPeriodic = this.serviceForm.value.isPeriodic === true ? true : false;
    service.isByLevel = this.serviceForm.value.priceBy === 1 ? true : false;
    service.isByZone = this.serviceForm.value.priceBy === 2 ? true : false;
    service.levelPrices = levelPrices;
    service.zonePrices = zonePrices;
    service.dueDates = dueDates;
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
