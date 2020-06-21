import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray, FormControl } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Utils } from 'src/app/shared/utils';
import { ClassService } from 'src/app/_services/class.service';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from 'src/app/_services/user.service';

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
  children: any;
  photoUrl: any[] = [];
  photoFile: File[] = [];

  constructor(private fb: FormBuilder, private alertify: AlertifyService, private userService: UserService,
    private classService: ClassService, private route: ActivatedRoute, private router: Router) { }

  ngOnInit() {
    this.createChildrenForm();
    this.route.data.subscribe((data: any) => {
      this.children = data['users'];
      console.log(this.children);
      for (let i = 0; i < this.children.length; i++) {
        const elt = this.children[i];
        this.addChildItem('', elt.lastName, elt.firstName, elt.strDateOfBirth, elt.gender, elt.email, elt.phoneNumber);
        // initialize photo file data
        this.photoFile[i] = null;
      }
    });
    this.getClassLevels();
    console.log(this.photoFile);
  }

  createChildrenForm() {
    this.childrenForm = this.fb.group({
      children: this.fb.array([])
    });
  }

  createChildItem(username, lname, fname, dob, sex, email, cell): FormGroup {
    return this.fb.group({
      username: [username, Validators.required],
      lname: [lname, Validators.required],
      fname: [fname, Validators.required],
      dob: [dob, Validators.required],
      sex: [sex, Validators.required],
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

  addChildItem(username, lname, fname, dob, sex, email, cell): void {
    const children = this.childrenForm.get('children') as FormArray;
    children.push(this.createChildItem(username, lname, fname, dob, sex, email, cell));
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

  imgResult(event, i) {
    let file: File = null;
    file = <File>event.target.files[0];

    const reader = new FileReader();
    reader.onload = (e: any) => {
      this.photoUrl[i] = e.target.result;
    };
    reader.readAsDataURL(event.target.files[0]);

    this.photoFile[i] = file;
    // console.log(this.photoFile);
    // console.log(this.photoUrl);
  }

  updateChildren() {
    const formData = new FormData();
    for (let i = 0; i < this.childrenForm.value.children.length; i++) {
      const child = this.childrenForm.value.children[i];
      console.log(child.lname + '-' + child.fname);
      if (this.photoFile[i]) {
        formData.append('photoFiles', this.photoFile[i], this.photoFile[i].name);
      }
      formData.append('id', this.children[i].id);
      formData.append('lastName', child.lname);
      formData.append('firstName', child.fname);
      // formData.append('gender', this.teacherForm.value.gender);
      // formData.append('strDateOfBirth', this.teacherForm.value.dateOfBirth);
      // formData.append('email', this.teacherForm.value.email);
      // formData.append('phoneNumber', this.teacherForm.value.cell);
      // formData.append('secondPhoneNumber', this.teacherForm.value.phone2);
      // formData.append('courseIds', ids);
      // formData.append('userTypeId', this.teacherTypeId.toString());
    }

    this.userService.addChild(formData).subscribe(() => {
      this.alertify.success('enseignant ajouté avec succès');
      // this.router.navigate(['/teachers']);
    }, error => {
      this.alertify.error(error);
    });
  }

}
