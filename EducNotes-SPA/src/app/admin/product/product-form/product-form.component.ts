import { Component, OnInit } from '@angular/core';
import { TresoService } from 'src/app/_services/treso.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { environment } from 'src/environments/environment';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { Router, ActivatedRoute } from '@angular/router';
import { Periodicity } from 'src/app/_models/periodicity';
import { PayableAt } from 'src/app/_models/payable-at';
import { Product } from 'src/app/_models/product';
import { Utils } from 'src/app/shared/utils';
import { ClassService } from 'src/app/_services/class.service';
import { element } from 'protractor';

@Component({
  selector: 'app-product-form',
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.scss'],
  animations: [SharedAnimations]
})
export class ProductFormComponent implements OnInit {

  productTypes: any[] = [];
  productForm: FormGroup;
  formModel: any;
  editMode = 'add';
  periodicities;
  payableAts;
  showDeadLines = false;
  showPeriodicities = false;
  showPeriodeOrDeadLine = false;
  showLevels = false;
  showTotalAmount = false;
  showRecap = false;
  recoveryTypes;
  billlingTypes;
  levels;
  deadLines;
  byDeadLineTypeId = environment.byDeadLineTypeId;
  schoolServicetypeId = environment.schoolServiceId;
  myDatePickerOptions = Utils.myDatePickerOptions;


  constructor(private tresoService: TresoService, private alertify: AlertifyService,
    private fb: FormBuilder, private router: Router, private route: ActivatedRoute,
    private classService: ClassService) { }

  ngOnInit() {
    this.recoveryTypes = this.tresoService.recouvrementTypes;
    this.billlingTypes = this.tresoService.billingTypes;
    this.route.data.subscribe(data => {
      if (data.product) {
        this.formModel = data.product;
        this.editMode = 'edit';
      } else {
        this.initParams();
      }
    });

    this.createProductForm();
    this.getPayableAts();
    this.getPeriodicities();
    this.getProductTypes();
    this.getLevels();
    this.getDeadlines();
  }

  initParams() {
    this.formModel = {
      name: '',
      comment: '',
      productTypeId: null,
      bylevel: false,
      deadLineCount: ''
    };
  }

  createProductForm() {
    this.productForm = this.fb.group({
      name: [this.formModel.name, Validators.required],
      productTypeId: [this.formModel.productTypeId, Validators.required],
      isByLevel: [null],
      recoveryTypeId: [null, Validators.required],
      payableAtId: [null, Validators.required],
      periodicityId: [null],
      price: [null],
      deadLineCount: [null],
      comment: [this.formModel.comment],
      deadlines: this.fb.array([
        this.addDeadlineFormGroup()
      ])
    });
  }

  addDeadlineFormGroup(): FormGroup {
    return this.fb.group({
      id: [null],
      percentage: ['']

    });
  }

  onNext2() {
    this.showTotalAmount = true;
  }

  onNext3() {
    if (this.productForm.value.isByLevel === 2) {
      // montant unique
      // besoin de charger tous les niveaux
      this.showLevels = true;
    } else {
      this.showRecap = true;
    }
  }


  // onNext() {
  //   if (this.productForm.value.bylevel === 2) {
  //     // montant unique
  //     // besoin de charger tous les niveaux
  //     this.showLevels = true;
  //   } else {
  //     this.showRecap = true;
  //   }
  // }
  getSelectedRecoveryType(event: number) {
    this.showPeriodeOrDeadLine = true;
    if (event === this.byDeadLineTypeId) {
      // afficher le champ du nombre d'échéance
      this.showPeriodicities = false;
    } else {
      this.showPeriodicities = true;

    }
  }

  onNext1() {
    const recoveryId = this.productForm.value.recoveryTypeId;
    if (recoveryId === this.byDeadLineTypeId) {
      //  le champ du nombre d'échéance est sélectionné
      (<FormArray>this.productForm.get('deadlines')).clear();
      for (let i = 0; i < this.productForm.value.deadLineCount; i++) {
        (<FormArray>this.productForm.get('deadlines')).push(this.addDeadlineFormGroup());
      }
      this.showDeadLines = true;
      // document.querySelector('#deadlinesDiv').scrollIntoView({ behavior: 'smooth', block: 'center' });
      // document.getElementById('deadlinesDiv').scrollIntoView();

    } else {
      // affichage direct du montant du service
      this.showTotalAmount = true;
      // document.querySelector('#totalAmontDiv').scrollIntoView({ behavior: 'smooth', block: 'center' });
      // document.getElementById('totalAmontDiv').scrollIntoView();


    }

  }


  showTotalAmountForm() {
    this.showTotalAmount = true;
  }

  recap() {
    this.showRecap = true;
    this.showLevels = false;
  }

  save() {
    if (this.editMode === 'add') {
      this.createProduct();
    }

    if (this.editMode === 'edit') {
      this.editProduct();
    }
  }

  createProduct() {
    const dataTosave = Object.assign({}, this.productForm.value);
    // dataTosave.productTypeId = this.schoolServicetypeId;
    if (dataTosave.isBylevel === 2) {
      dataTosave.isByLevel = true;
    }

    if (dataTosave.isBylevel !== 2) {
      dataTosave.isByLevel = false;
    }

    if (dataTosave.recoveryTypeId === 1) {
      dataTosave.isPeriodic = false;
    }

    if (dataTosave.recoveryTypeId !== 1) {
      dataTosave.isPeriodic = true;
    }


    this.tresoService.createProduct(dataTosave).subscribe(() => {
      this.alertify.success('enregistrement terminé...');
      this.router.navigate(['/productsList']);
    }, error => {
      this.alertify.error(error);
    });
  }

  editProduct() {
    this.tresoService.editProduct(this.formModel.id, this.productForm.value).subscribe(() => {
      this.alertify.success('modification  éffectuée..');
      this.router.navigate(['/productsList']);
    }, error => {
      this.alertify.error(error);
    });
  }

  getPeriodicities() {
    this.tresoService.getPeriodicities().subscribe((res: Periodicity[]) => {
      this.periodicities = [];
      for (let i = 0; i < res.length; i++) {
        const el = { value: res[i].id, label: res[i].name };
        this.periodicities = [...this.periodicities, el];
      }
    });
  }

  getPayableAts() {
    this.tresoService.getPayableAts().subscribe((res: PayableAt[]) => {
      this.payableAts = [];
      for (let i = 0; i < res.length; i++) {
        const el = { value: res[i].id, label: res[i].name + ' (' + res[i].dayCount + ')' };
        this.payableAts = [...this.payableAts, el];
      }
    });
  }
  getProductTypes() {
    this.tresoService.getProductTypes().subscribe((res: Product[]) => {
      for (let i = 0; i < res.length; i++) {
        const el = { value: res[i].id, label: res[i].name };
        this.productTypes = [...this.productTypes, el];
      }
    });
  }

  getLevels() {
    this.classService.getLevels().subscribe((res) => {
      this.levels = res;
    });
  }

  getDeadlines() {
    this.deadLines = [];
    this.tresoService.getDeadlines().subscribe((res: any[]) => {
      for (let i = 0; i < res.length; i++) {
        const ele = {value : res[i].id, label : res[i].name + '(' + res[i].dueDate + ')' };
        this.deadLines = [...this.deadLines, ele];
      }
    });
  }

}
