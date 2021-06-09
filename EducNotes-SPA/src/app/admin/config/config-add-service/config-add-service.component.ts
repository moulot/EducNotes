import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-config-add-service',
  templateUrl: './config-add-service.component.html',
  styleUrls: ['./config-add-service.component.scss']
})
export class ConfigAddServiceComponent implements OnInit {
  product: any;
  serviceForm: FormGroup;
  editionMode = false;
  wait = false;

  constructor(private fb: FormBuilder, private alertify: AlertifyService,
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

    this.createServiceForm();
  }

  createServiceForm() {
    this.serviceForm = this.fb.group({
      name: ['', Validators.required],
      typeid: [this.product.typeid, Validators.required],
      isPaidCash: [this.product.isPaidCash, Validators.required],
      price: [this.product.price],
      isByLevel: [this.product.isByLevel],
      
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
