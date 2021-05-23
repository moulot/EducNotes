import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Utils } from 'src/app/shared/utils';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClassService } from 'src/app/_services/class.service';

@Component({
  selector: 'app-config-course-type',
  templateUrl: './config-course-type.component.html',
  styleUrls: ['./config-course-type.component.scss']
})
export class ConfigCourseTypeComponent implements OnInit {
  courseTypeForm: FormGroup;
  courseTypes: any;

  constructor(private fb: FormBuilder, private alertify: AlertifyService,
    private classService: ClassService) { }

  ngOnInit() {
    this.createCourseTypeForm();
    this.getCourseTypes();
  }

  createCourseTypeForm() {
    this.courseTypeForm = this.fb.group({
      coursetypes: this.fb.array([])
    });
  }

  addCourseTypeItems() {
    const coursetypes = this.courseTypeForm.get('coursetypes') as FormArray;
    this.courseTypes.forEach(x => {
      coursetypes.push(this.fb.group({
        id: x.id,
        name: x.name,
        toBeDeleted: false
      }));
    });
  }

  addCourseTypeItem(id, name): void {
    const coursetypes = this.courseTypeForm.get('coursetypes') as FormArray;
    coursetypes.push(this.createCourseTypeItem(id, name));
  }

  createCourseTypeItem(id, name): FormGroup {
    return this.fb.group({
      id: [id],
      name: [name, Validators.required],
      toBeDeleted: [false]
    });
  }

  removeActivitytem(index) {
    const coursetypes = this.courseTypeForm.get('coursetypes') as FormArray;
    const id = coursetypes.at(index).get('id').value;
    if (id === 0) {
      coursetypes.removeAt(index);
    } else {
      coursetypes.at(index).get('toBeDeleted').setValue(true);
    }
  }

  removeAllItems() {
    const coursetypes = this.courseTypeForm.get('coursetypes') as FormArray;
    while (coursetypes.length > 0) {
      coursetypes.removeAt(0);
    }
  }

  resetCourseTypeItem(index) {
    const coursetypes = this.courseTypeForm.get('coursetypes') as FormArray;
    coursetypes.at(index).get('toBeDeleted').setValue(false);
  }

  addCourseType() {
    this.addCourseTypeItem(0, '');
  }

  getCourseTypes() {
    this.classService.getCourseTypes().subscribe(data => {
      this.courseTypes = data;
      this.addCourseTypeItems();
    }, error => {
      this.alertify.error('problème pour récupérer les données.');
    });
  }

  save() {
    let coursetypes = [];
    for (let i = 0; i < this.courseTypeForm.value.coursetypes.length; i++) {
      const type = this.courseTypeForm.value.coursetypes[i];
      const item = {id: type.id, name: type.name, toBeDeleted: type.toBeDeleted};
      coursetypes = [...coursetypes, item];
    }
    this.classService.saveCourseTypes(coursetypes).subscribe(() => {
      Utils.smoothScrollToTop(40);
      this.removeAllItems();
      this.getCourseTypes();
      this.alertify.success('les types de cours sont enregistrés.');
    }, error => {
      this.alertify.error('problème pour enregistrer les types de cours');
    });
}

}