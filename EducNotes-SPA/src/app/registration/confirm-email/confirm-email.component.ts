import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import { User } from 'src/app/_models/user';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router, ActivatedRoute } from '@angular/router';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { AccountService } from 'src/app/_services/account.service';
import { ConfirmToken } from 'src/app/_models/confirmToken';
import { Utils } from 'src/app/shared/utils';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.css'],
  animations: [SharedAnimations]
})
export class ConfirmEmailComponent implements OnInit {
  user: User;
  children: any;
  userForm: FormGroup;
  phoneForm: FormGroup;
  userNameExist = false;
  emailOK = false;
  phoneMask = Utils.phoneMask;
  validAccount = false;
  phoneValidationSteps = 0;
  phoneOk = false;
  parentOk = false;
  childrenOk = false;
  initialUserName: string;
  pwd: string;
  wait = false;
  parentid: number;

  constructor(private authService: AuthService, private fb: FormBuilder, private route: ActivatedRoute,
    private alertify: AlertifyService,  private router: Router, private accountService: AccountService) { }

  ngOnInit() {
    this.authService.eraseSessionData();
    this.createPhoneForm();
    this.createUserForm();

    const id: string = this.route.snapshot.queryParamMap.get('id');
    this.parentid = Number(id);
    const orderid: string = this.route.snapshot.queryParamMap.get('orderid');
    const token: string = this.route.snapshot.queryParamMap.get('token');
    if (id !== null && token !== null) {
      const confirmEmail = <ConfirmToken>{};
      confirmEmail.userId = id;
      confirmEmail.token = token;
      this.validateEmail(confirmEmail);
    }
  }

  createPhoneForm() {
    this.phoneForm = this.fb.group({
      phone: ['', [ Validators.required, this.phoneValidator ] ],
      code: ['', [ this.codeValidator ] ]
    });
  }

  createUserForm() {
    this.userForm = this.fb.group({
      lastName: ['', Validators.required],
      firstName: ['', Validators.required],
      email: ['', Validators.required],
      userName: ['', Validators.required],
      cell: ['', [ Validators.required, this.phoneValidator ] ],
      pwd: ['', Validators.required],
      checkpwd: ['', [ Validators.required, this.confirmationValidator ] ]
    });
  }

  initialValues() {
    let login = '';
    if (this.user.accountDataValidated) {
      login = this.user.userName;
      this.parentid = this.user.id;
    }
    this.initialUserName = login;
    this.userForm.setValue({lastName: this.user.lastName, firstName: this.user.firstName, email: this.user.email,
      userName: login, cell: this.user.phoneNumber, pwd: '', checkpwd: ''});
    this.phoneForm.setValue({phone: this.user.phoneNumber, code: ''});
  }

  changePhone(phonenum) {
    this.userForm.get('cell').setValue(phonenum);
  }

  sendPhoneNumber() {
    this.wait = true;
    this.accountService.sendPhoneNumberToValidate(this.user.id, this.phoneForm.value.phone).subscribe(codeSent => {
      if (codeSent) {
        this.phoneValidationSteps = 1;
      }
      this.wait = false;
    }, error => {
      this.alertify.error(error);
      this.wait = false;
    });
  }

  sendEmailToConfirm() {

  }

  validateEmail(confirmEmail: ConfirmToken) {
    this.accountService.confirmEmail(confirmEmail).subscribe((data: any) => {
      this.emailOK = data.emailOK;
      this.user = data.user;
      this.children = data.children;
      this.parentOk = this.user.accountDataValidated;
      this.validAccount = data.accountValidated;
      this.childrenOk = data.childrenValidated;
      this.initialValues();
    }, error => {
      this.alertify.error(error);
    });
  }

  validatePhone() {
    this.wait = true;
    const confirmPhone = <ConfirmToken>{};
    confirmPhone.userId = this.user.id.toString();
    confirmPhone.token = this.phoneForm.value.code;
    this.accountService.validatePhone(confirmPhone).subscribe((data: any) => {
      this.user = data.user;
      this.phoneOk = data.phoneOk;
      if (this.phoneOk) {
        this.phoneValidationSteps = 2;
      }
      this.wait = false;
    }, error => {
      this.alertify.error(error);
      this.wait = false;
    });
  }

  userNameVerification() {
    const userName = this.userForm.value.userName;
    this.userNameExist = false;
    this.authService.userNameExist(userName, this.parentid).subscribe((res: boolean) => {
      if (res === true) {
        this.userNameExist = true;
      }
    });
  }

  confirmationValidator = (control: FormControl): { [ s: string ]: boolean } => {
    if (!control.value) {
      return { required: true };
    } else if (control.value !== this.userForm.controls.pwd.value) {
      return { confirm: true, error: true };
    }
  }

  phoneValidator = (control: FormControl): { [ s: string ]: boolean } => {
    if (!control.value) {
      return { required: true };
    } else if (control.value.length !== 8) {
      return { lenError: true };
    }
  }

  codeValidator = (control: FormControl): { [ s: string ]: boolean } => {
    if (!control.value) {
      return { empty: true };
    } else {
      return { empty: false };
    }
  }

  updateConfirmValidator(): void {
    /** wait for refresh value */
    Promise.resolve().then(() => this.userForm.controls.checkPassword.updateValueAndValidity());
  }

  updateUser(): void {
    this.wait = true;
    this.authService.setUserAccountData(this.user.id, this.userForm.value).subscribe(() => {
      this.parentOk = true;
      this.childrenOk = true;
      this.validAccount = this.user.accountDataValidated;
      for (let i = 0; i < this.children.length; i++) {
        const child = this.children[i];
        if (child.accountDataValidated !== true) {
          this.childrenOk = false;
          break;
        }
      }
      this.wait = false;
    }, error => {
      this.alertify.error(error);
      this.wait = false;
    });
  }

  updateAccount() {
    this.wait = true;
    this.authService.getUser(this.user.id).subscribe((parent: User) => {
      this.user = parent;
      this.validAccount = this.user.accountDataValidated;
      this.childrenOk = true;
      this.wait = false;
    }, error => {
      this.alertify.error(error);
      this.wait = false;
    });
  }

}
