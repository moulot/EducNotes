import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClassService } from 'src/app/_services/class.service';

@Component({
  selector: 'app-edit-level-classes',
  templateUrl: './edit-level-classes.component.html',
  styleUrls: ['./edit-level-classes.component.scss']
})
export class EditLevelClassesComponent implements OnInit {
  classes: any;
  classesForm: FormGroup;
  levelId: number;
  level: any;
  classTypeOptions = [];
  showType = false;

  constructor(private route: ActivatedRoute, private fb: FormBuilder, private classService: ClassService,
    private alertify: AlertifyService, private router: Router) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.classes = data['classes'];
      console.log(this.classes);
    });
    this.createClassesForm();
    this.addClassItems();
    this.route.params.subscribe(params => {
      this.levelId = params['levelId'];
      this.showClassType(this.levelId);
    });
    this.getClassTypes();
  }

  createClassesForm() {
    this.classesForm = this.fb.group({
      classes: this.fb.array([])
    });
  }

  addClassItems() {
    const classes = this.classesForm.get('classes') as FormArray;
    this.classes.forEach(x => {
      classes.push(this.fb.group({
        id: x.id,
        name: x.name,
        typeId: x.classTypeId,
        toBeDeleted: false
      }));
    });
  }

  addClassItem(id, name, typeid, toBeDeleted): void {
    const classes = this.classesForm.get('classes') as FormArray;
    classes.push(this.createClassItem(id, name, typeid, toBeDeleted));
  }

  createClassItem(id, name, typeid, toBeDeleted): FormGroup {
    return this.fb.group({
      id: [id],
      name: [name, Validators.required],
      typeId: [typeid, Validators.required],
      toBeDeleted: [false]
    });
  }

  removeClassItem(index) {
    const classes = this.classesForm.get('classes') as FormArray;
    const id = classes.at(index).get('id').value;
    if (id === 0) {
      classes.removeAt(index);
    } else {
      classes.at(index).get('toBeDeleted').setValue(true);
    }
  }

  resetClassItem(index) {
    const coursetypes = this.classesForm.get('classes') as FormArray;
    coursetypes.at(index).get('toBeDeleted').setValue(false);
  }

  addClass() {
    this.addClassItem(0, '', null, false);
  }

  getClassTypes() {
    this.classService.getClassTypes().subscribe((data: any) => {
      const classTypes = data;
      for (let i = 0; i < classTypes.length; i++) {
        const elt = classTypes[i];
        const type = {value: elt.id, label: elt.name};
        this.classTypeOptions = [...this.classTypeOptions, type];
      }
    }, () => {
      this.alertify.error('problème pour récupérer les données');
    });
  }

  showClassType(id) {
    if (id === '14' || id === '15' || id === '16') {
      this.showType = true;
    }
  }

  saveClasses() {
    let classesToSave = [];
    for (let i = 0; i < this.classesForm.value.classes.length; i++) {
      const elt = this.classesForm.value.classes[i];
      let aclass = <any>{};
      aclass = {id: elt.id, name: elt.name, classTypeId: elt.typeId};
      classesToSave = [...classesToSave, aclass];
    }

    this.classService.saveClasses(classesToSave).subscribe(() => {
      this.alertify.success('les classes ont été mises à jour');
      this.router.navigate(['classesPanel']);
    }, error => {
      this.alertify.error(error);
    });
  }

}
