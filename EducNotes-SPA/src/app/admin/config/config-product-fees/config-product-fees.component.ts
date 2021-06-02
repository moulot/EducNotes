import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Utils } from 'src/app/shared/utils';
import { AdminService } from 'src/app/_services/admin.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-config-product-fees',
  templateUrl: './config-product-fees.component.html',
  styleUrls: ['./config-product-fees.component.scss']
})
export class ConfigProductFeesComponent implements OnInit {
  tuitionId = environment.tuitionId;
  myDatePickerOptions = Utils.myDatePickerOptions;
  prodFeesForm: FormGroup;
  tuitionFees: any;
  dueDates: any;
  levels: any;
  wait = false;

  constructor(private fb: FormBuilder, private alertify: AlertifyService,
    private adminService: AdminService) { }

  ngOnInit() {
    this.createProdFeesForm();
    this.getTuitionFees();
  }

  createProdFeesForm() {
    this.prodFeesForm = this.fb.group({
      productId: [this.tuitionId, Validators.required],
      levelFees: this.fb.array([]),
      dueDates: this.fb.array([])
    }, {validators: this.formValidator});
  }

  formValidator(g: FormGroup) {
    let totalPct = 0;
    let totalSeq = 0;
    const dueDates = g.get('dueDates').value;
    const levelFees = g.get('levelFees').value;
    let formNOK = false;

    // check classlevel product fees
    let feesNOK = false;
    for (let i = 0; i < levelFees.length; i++) {
      const fee = levelFees[i].price;
      if (fee === null || fee === '') {
        feesNOK = true;
        formNOK = true;
        break;
      }
    }

    // check the percentage
    let pctNOK = false;
    // let pct0NOK = false;
    for (let i = 0; i < dueDates.length; i++) {
      const pct = dueDates[i].percent;
      totalPct += pct;
      if (pct <= 0) {
        pctNOK = true;
        formNOK = true;
      }
      const seq = dueDates[i].seq;
      totalSeq += seq;
    }
    if (totalPct !== 100) {
      pctNOK = true;
      formNOK = true;
    }

    // checks sequences sum : n(n + 1)/2
    let seqNOK = false;
    const n = dueDates.length;
    const sum = n * (n + 1) / 2;
    if (sum !== totalSeq) {
      seqNOK = true;
      formNOK = true;
    } else {

    }

    return {'formNOK': formNOK, 'feesNOK': feesNOK, 'pctNOK': pctNOK, 'seqNOK': seqNOK};
  }

  addLevelFeeItems() {
    const levelFees = this.prodFeesForm.get('levelFees') as FormArray;
    this.tuitionFees.levelFees.forEach(x => {
      levelFees.push(this.fb.group({
        id: x.id,
        levelid: x.levelId,
        name: x.levelName,
        price: x.price
      }));
    });
  }

  addDueDateItems() {
    const dueDates = this.prodFeesForm.get('dueDates') as FormArray;
    this.dueDates.forEach(x => {
      dueDates.push(this.fb.group({
        id: x.id,
        prodid: x.productId,
        dueDate: x.strDueDate,
        name: x.deadLineName,
        seq: x.seq,
        percent: x.percentage * 100,
        toBeDeleted: [false]
      }));
    });
  }

  addDueDateItem(id, dueDate, name, seq, pct): void {
    const dueDates = this.prodFeesForm.get('dueDates') as FormArray;
    dueDates.push(this.createDueDateItem(id, dueDate, name, seq, pct));
  }

  createDueDateItem(id, dueDate, name, seq, pct): FormGroup {
    return this.fb.group({
      id: [id],
      dueDate: [dueDate, Validators.required],
      name: [name, Validators.required],
      seq: [seq, Validators.required],
      percent: [pct, Validators.required],
      toBeDeleted: [false]
    });
  }

  removeDueDatetem(index) {
    const dueDates = this.prodFeesForm.get('dueDates') as FormArray;
    dueDates.removeAt(index);
  }

  resetDueDateItem(index) {
    const dueDates = this.prodFeesForm.get('dueDates') as FormArray;
    dueDates.at(index).get('toBeDeleted').setValue(false);
  }

  addDueDate() {
    this.addDueDateItem(0, null, '', 0, 0);
  }

  getTuitionFees() {
    this.wait = true;
    this.adminService.getTuitionFees().subscribe((data: any) => {
      this.tuitionFees = data.tuitionFees;
      this.dueDates = data.dueDates;
      this.addLevelFeeItems();
      this.addDueDateItems();
      this.wait = false;
    }, () => {
      this.alertify.error('problème pour récupérer les données');
      this.wait = false;
    });
  }

  removeAllDueDates() {
    const dueDates = this.prodFeesForm.get('dueDates') as FormArray;
    while (dueDates.length > 0) {
      dueDates.removeAt(0);
    }
  }

  sortDueDates() {
    const dueDates = this.prodFeesForm.value.dueDates.sort((a, b) => {
      const date1 = a.dueDate.split('/');
      const date2 = b.dueDate.split('/');
      const num1 = Number(date1[2] + date1[1] + date1[0]);
      const num2 = Number(date2[2] + date2[1] + date2[0]);
      return num1 < num2 ? -1 : 1;
    });
    this.removeAllDueDates();
    for (let i = 0; i < dueDates.length; i++) {
      const elt = dueDates[i];
      this.addDueDateItem(elt.id, elt.dueDate, elt.name, i + 1, elt.percent);
    }
  }

  save() {
    this.wait = true;
    let levelProducts = [];
    let prodDeadlines = [];
    const prodid = this.prodFeesForm.value.productId;

    for (let i = 0; i < this.prodFeesForm.value.levelFees.length; i++) {
      const elt = this.prodFeesForm.value.levelFees[i];
      const levelFee = {id: elt.id, productId: prodid, classLevelId: elt.levelid, price: elt.price};
      levelProducts = [...levelProducts, levelFee];
    }

    for (let i = 0; i < this.prodFeesForm.value.dueDates.length; i++) {
      const elt = this.prodFeesForm.value.dueDates[i];
      const dueDate = {id: elt.id, productId: prodid, strDueDate: elt.dueDate, deadLineName: elt.name,
        percentage: elt.percent, seq: elt.seq, toBeDeleted: elt.toBeDeleted};
      prodDeadlines = [...prodDeadlines, dueDate];
    }

    const dueDateFees = <any>{};
    dueDateFees.levelProducts = levelProducts;
    dueDateFees.productDeadlines = prodDeadlines;
    this.adminService.saveLevelTuitionFees(dueDateFees).subscribe(() => {
      this.wait = false;
      Utils.smoothScrollToTop(40);
      this.alertify.success('les données ont été enregistrées');
    }, error => {
      this.alertify.error(error);
      this.wait = false;
    });
  }

}
