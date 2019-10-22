import { Component, OnInit } from '@angular/core';
import { ProductType } from 'src/app/_models/productType';
import { TresoService } from 'src/app/_services/treso.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { environment } from 'src/environments/environment';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { Router, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-product-form',
  templateUrl: './product-form.component.html',
  styleUrls: ['./product-form.component.scss'],
  animations :  [SharedAnimations]
})
export class ProductFormComponent implements OnInit {

  productTypes: ProductType[];
  productForm: FormGroup;
  formModel: any;
  editMode = 'add';
  schoolServicetypeId = environment.schoolServiceId;

  constructor(private tresoService: TresoService, private alertify: AlertifyService,
    private fb: FormBuilder, private router: Router, private route: ActivatedRoute) { }

  ngOnInit() {

    this.route.data.subscribe(data => {
      if (data.product) {
        this.formModel = data.product;
        this.editMode = 'edit';
      } else {
        this.initParams();
      }
    });

    this.createProductForm();
  }

  initParams() {
    this.formModel = {
      name: '',
      comment: ''
    };
  }

  createProductForm() {
    this.productForm = this.fb.group({
       name: [this.formModel.name, Validators.required],
       comment: [this.formModel.comment]});
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
    const dataTosave =  Object.assign({}, this.productForm.value);
    dataTosave.productTypeId = this.schoolServicetypeId;
    this.tresoService.createProduct(dataTosave).subscribe(() => {
     this.alertify.success('enregistrement terminé...');
     this.router.navigate(['/productsList']);
    }, error => {
      this.alertify.error(error);
    });
  }

  editProduct() {
    this.tresoService.editProduct(this.formModel.id , this.productForm.value).subscribe(() => {
      this.alertify.success('modification  éffectuée..');
      this.router.navigate(['/productsList']);
    }, error => {
      this.alertify.error(error);
    });
  }

}
