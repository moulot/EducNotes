import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { Utils } from 'src/app/shared/utils';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClassService } from 'src/app/_services/class.service';

@Component({
  selector: 'app-config-products',
  templateUrl: './config-products.component.html',
  styleUrls: ['./config-products.component.scss']
})
export class ConfigProductsComponent implements OnInit {
  productForm: FormGroup;
  products: any;

  constructor(private fb: FormBuilder, private alertify: AlertifyService,
    private classService: ClassService) { }

  ngOnInit() {
    this.createProductForm();
  }

  createProductForm() {
    this.productForm = this.fb.group({
      products: this.fb.array([])
    });
  }

  addProductItems() {
    const products = this.productForm.get('products') as FormArray;
    this.products.forEach(x => {
      products.push(this.fb.group({
        id: x.id,
        name: x.name
      }));
    });
  }

  save() {
    let levelcourses = [];
    for (let i = 0; i < this.productForm.value.levels.length; i++) {
      const level = this.productForm.value.levels[i];
      const item = <any>{};
      item.levelId = level.id;
      item.courses = [];
      for (let j = 0; j < level.courses.length; j++) {
        const elt = level.courses[j];
        if (elt.selected) {
          const course = <any>{};
          course.id = elt.id;
          course.selected = elt.selected;
          item.courses = [...item.courses, course];
        }
      }
      levelcourses = [...levelcourses, item];
    }
    this.classService.saveLevelCourses(levelcourses).subscribe(() => {
      Utils.smoothScrollToTop(40);
      this.alertify.success('les cours ont bien été enregistrés. merci');
    }, error => {
      this.alertify.error(error);
    });
}

}
