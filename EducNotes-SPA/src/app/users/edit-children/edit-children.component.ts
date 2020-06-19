import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray, FormControl } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Utils } from 'src/app/shared/utils';
import { ClassService } from 'src/app/_services/class.service';

@Component({
  selector: 'app-edit-children',
  templateUrl: './edit-children.component.html',
  styleUrls: ['./edit-children.component.scss']
})
export class EditChildrenComponent implements OnInit {
  childrenForm: FormGroup;
  birthDateMask = Utils.birthDateMask;
  phoneMask = Utils.phoneMask;
  sexOptions = [{value: 0, label: 'femme'}, {value: 1, label: 'homme'}];
  levelOptions: any[] = [];
  levels: any;

  constructor(private fb: FormBuilder, private alertify: AlertifyService,
    private classService: ClassService) { }

  ngOnInit() {
    this.createChildrenForm();
    this.getClassLevels();
    this.addChildItem('', '', '', '', null, '', '', null);
  }

  getClassLevels() {
    this.classService.getLevels().subscribe(data => {
      this.levels = data;
      for (let i = 0; i < this.levels.length; i++) {
        const elt = this.levels[i];
        const level = {value: elt.id, label: elt.name};
        this.levelOptions = [...this.levelOptions, level];
      }
    }, error => {
      this.alertify.error(error);
    });
  }

  createChildrenForm() {
    this.childrenForm = this.fb.group({
      children: this.fb.array([])
    });
  }

  addChildItem(username, lname, fname, dob, sex, email, cell, classLevelId): void {
    const children = this.childrenForm.get('children') as FormArray;
    children.push(this.createChildItem(username, lname, fname, dob, sex, email, cell, classLevelId));
  }

  createChildItem(username, lname, fname, dob, sex, email, cell, classLevelId): FormGroup {
    return this.fb.group({
      username: [username, Validators.required],
      lname: [lname, Validators.required],
      fname: [fname, Validators.required],
      dob: [dob, Validators.required],
      sex: [sex, Validators.required],
      classlevelId: [classLevelId, Validators.required],
      email: [email, Validators.email],
      cell: [cell],
      pwd: ['', Validators.required],
      checkpwd: ['', [ Validators.required, this.confirmationValidator ]]
    });
  }

  confirmationValidator = (control: FormControl): { [ s: string ]: boolean } => {
    if (!control.value) {
      return { required: true };
    } else if (control.value !== this.childrenForm.controls.pwd.value) {
      return { confirm: true, error: true };
    }
  }

  updateChildren() {

  }

}
