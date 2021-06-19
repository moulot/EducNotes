import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ProductService } from 'src/app/_services/product.service';

@Component({
  selector: 'app-config-add-service',
  templateUrl: './config-add-service.component.html',
  styleUrls: ['./config-add-service.component.scss']
})
export class ConfigAddServiceComponent implements OnInit {
  product: any;
  serviceForm: FormGroup;
  editionMode = false;
  typeOptions = [];
  trueFalseOptions = [{value: false, label: 'NON'}, {value: true, label: 'OUI'}];
  wait = false;

  constructor(private fb: FormBuilder, private alertify: AlertifyService, private productService: ProductService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.product = data['product'];
      console.log(this.product);
      if (this.product) {
        this.editionMode = true;
      } else {
        this.initValues();
      }
    });

    this.getProductTypes();
    this.createServiceForm();
  }

  createServiceForm() {
    this.serviceForm = this.fb.group({
      name: [this.product.name, Validators.required],
      typeId: [this.product.productTypeId, Validators.required],
      price: [this.product.price],
      isPaidCash: [this.product.isPaidCash, Validators.required],
      isByLevel: [this.product.isByLevel]
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

  // addProductItem(id, name): void {
  //   const services = this.serviceForm.get('services') as FormArray;
  //   services.push(this.createServiceItem(id, name));
  // }

  // createServiceItem(id, name, typeid): FormGroup {
  //   return this.fb.group({
  //     id: [id],
  //     name: [name, Validators.required],
  //     toBeDeleted: [false]
  //   });
  // }

  // removeServicetem(index) {
  //   const services = this.serviceForm.get('services') as FormArray;
  //   const id = services.at(index).get('id').value;
  //   if (id === 0) {
  //     services.removeAt(index);
  //   } else {
  //     services.at(index).get('toBeDeleted').setValue(true);
  //   }
  // }

  saveService() {

  }

}
