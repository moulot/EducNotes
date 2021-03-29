import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClassService } from 'src/app/_services/class.service';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-new-class',
  templateUrl: './new-class.component.html',
  styleUrls: ['./new-class.component.scss'],
  animations: [SharedAnimations]
})
export class NewClassComponent implements OnInit {
  cyle = environment.terminalCycle;
  levelOptions = [];
  classTypeOptions = [];
  levels: any;
  classForm: FormGroup;
  suffixes = [{ value: 1, label: 'A,B,C,...' }, { value: 2, label: '1,2,3,.....' }];
  wait = false;

  constructor(private fb: FormBuilder, private router: Router,
    private classService: ClassService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.getLevels();
    this.getCLClassTypes();
    this.createClassForm();
  }

  createClassForm() {
    this.classForm = this.fb.group({
      levelId: [null, Validators.required],
      classTypeId: [null],
      name: [''],
      suffixe: [null, Validators.nullValidator],
      maxStudent: [null, Validators.required],
      nbClass: [null, Validators.required]
    }, {validator: this.formValidator});
  }

  formValidator(g: FormGroup) {
    const levelid = g.get('levelId').value;
    const classId = g.get('classTypeId').value;
    if ((levelid === 14 || levelid === 15 || levelid === 16) && classId == null) {
      return {'classTypeNOK': true};
    }
    return null;
  }

  getLevels() {
    this.classService.getLevels().subscribe((res: any[]) => {
      for (let i = 0; i < res.length; i++) {
        const elt = { value: res[i].id, label: res[i].name };
        this.levelOptions = [...this.levelOptions, elt];
      }
    }, error => {
      this.alertify.error(error);
    });
  }

  getCLClassTypes() {
    this.classService.getLevelWithClassTypes().subscribe(data => {
      this.levels = data;
    }, error => {
      this.alertify.error(error);
    });
  }

  levelChanged() {
    this.classForm.patchValue({classTypeId: null});
    this.classTypeOptions = [];
    const levelid = this.classForm.value.levelId;
    if (levelid) {
      const level = this.levels.find(c => c.levelId === levelid);
      for (let i = 0; i < level.classTypes.length; i++) {
        const ctype = level.classTypes[i];
        const elt = { value: ctype.id, label: ctype.name };
         this.classTypeOptions = [...this.classTypeOptions, elt];
      }
    }
  }

  // save() {
  //   this.errorMessage = '';
  //   const classFromForm = Object.assign({}, this.classForm.value);
  //   if (classFromForm.suffixe) {
  //     if (!classFromForm.number) {
  //       this.alertify.error('veuillez saisir le nombre de classe');
  //     } else {
  //       this.saveClass(classFromForm);
  //     }
  //   } else if (!classFromForm.name) {
  //     if (!classFromForm.suffixe) {
  //       this.alertify.error('veuillez saisir au moins le nom de la classe');
  //     } else {
  //       this.saveClass(classFromForm);
  //     }
  //   } else {
  //     this.saveClass(classFromForm);
  //   }
  // }

  saveClass() {
    // this.wait = true;
    const levelid = this.classForm.value.levelId;
    const classtypeid = this.classForm.value.classTypeId;
    const classes = Object.assign({}, this.classForm.value);
    const level = this.levels.find(c => c.levelId === levelid);
    if (level.classTypes.length > 0) {
      classes.classTypeCode = level.classTypes.find(c => c.id === classtypeid).code;
    }
    this.classService.saveClasses(classes).subscribe(next => {
      this.alertify.success('les classes ont été ajoutées');
      this.wait = false;
      this.router.navigate(['classesPanel']);
    }, error => {
      this.alertify.error(error);
      this.wait = false;
    });
  }
}
