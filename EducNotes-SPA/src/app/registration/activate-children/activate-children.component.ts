import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { Validators, FormGroup, FormArray, FormBuilder } from '@angular/forms';
import { ClassService } from 'src/app/_services/class.service';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from 'src/app/_services/user.service';
import { Utils } from 'src/app/shared/utils';

@Component({
  selector: 'app-activate-children',
  templateUrl: './activate-children.component.html',
  styleUrls: ['./activate-children.component.scss']
})
export class ActivateChildrenComponent implements OnInit {
  @Input() parentId: any;
  @Input() children: any;
  @Output() updateAccount = new EventEmitter();
  childrenForm: FormGroup;
  birthDateMask = Utils.birthDateMask;
  phoneMask = Utils.phoneMask;
  myDatePickerOptions = Utils.myDatePickerOptions;
  sexOptions = [{value: 0, label: 'femme'}, {value: 1, label: 'homme'}];
  levelOptions: any[] = [];
  levels: any;
  photoUrl: any[] = [];
  photoFile: File[] = [];
  userNameExist = false;
  confirmedPwd: boolean;
  wait = false;

  constructor(private fb: FormBuilder, private alertify: AlertifyService, private userService: UserService,
    private classService: ClassService, private route: ActivatedRoute, private router: Router,
    private authService: AuthService) { }

  ngOnInit() {
    this.createChildrenForm();
    for (let i = 0; i < this.children.length; i++) {
      const elt = this.children[i];
      this.addChildItem('', elt.lastName, elt.firstName, elt.strDateOfBirth, elt.gender, elt.email, elt.phoneNumber);
      // initialize photo file data
      this.photoFile[i] = null;
    }
  // this.getClassLevels();
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
      checkpwd: ['', Validators.required]
    });
  }

  addChildItem(username, lname, fname, dob, sex, email, cell): void {
    const children = this.childrenForm.get('children') as FormArray;
    children.push(this.createChildItem(username, lname, fname, dob, sex, email, cell));
  }

  getParentChildren() {
    this.userService.getAccountChildren(this.parentId).subscribe((data: any) => {
      this.children = data.children;
    });
  }

  // getClassLevels() {
  //   this.classService.getLevels().subscribe(data => {
  //     this.levels = data;
  //     for (let i = 0; i < this.levels.length; i++) {
  //       const elt = this.levels[i];
  //       const level = {value: elt.id, label: elt.name};
  //       this.levelOptions = [...this.levelOptions, level];
  //     }
  //   }, error => {
  //     this.alertify.error(error);
  //   });
  // }

  pwdValidator(index) {
    const pwd = this.childrenForm.value.children[index].pwd;
    const checkpwd = this.childrenForm.value.children[index].checkpwd;
    if (pwd === checkpwd) {
      this.confirmedPwd = true;
    } else {
      this.confirmedPwd = false;
    }
  }
  // passwordMatchValidator(g: FormGroup) {
  //   return g.get('pwd').value === g.get('checkpwd').value ? null : {'mismatch': true};
  // }

  userNameVerification(index) {
    const userName = this.childrenForm.value.children[index].username;
    const userid = this.childrenForm.value.children[index].id;
    this.userNameExist = false;
    this.authService.userNameExist(userName, userid).subscribe((res: boolean) => {
      if (res === true) {
        this.userNameExist = true;
      }
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
  }

  updateChildren() {
    this.wait = true;
    const formData = new FormData();
    formData.append('parentId', this.parentId);
    for (let i = 0; i < this.childrenForm.value.children.length; i++) {
      const child = this.childrenForm.value.children[i];
      if (this.photoFile[i]) {
        formData.append('photoFiles', this.photoFile[i], this.photoFile[i].name);
        formData.append('photoIndex', this.children[i].id);
      }
      formData.append('id', this.children[i].id);
      formData.append('lastName', child.lname);
      formData.append('firstName', child.fname);
      formData.append('gender', child.sex);
      formData.append('strDateOfBirth', child.dob);
      formData.append('userName', child.username);
      formData.append('password', child.pwd);
      // optional data
      if (child.email) {
        formData.append('email', child.email);
      } else {
        formData.append('email', '');
      }
      if (child.cell) {
        formData.append('phoneNumber', child.cell.replace(/./g, ''));
      } else {
        formData.append('phoneNumber', '');
      }
    }

    this.authService.validateChildAccounts(formData).subscribe(() => {
      this.alertify.success('les comptes enfants sont valiés. merci.');
      this.updateAccount.emit();
      this.wait = false;
    }, error => {
      this.alertify.error(error);
      this.wait = false;
    });
  }

}
