import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import { User } from 'src/app/_models/user';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router, ActivatedRoute } from '@angular/router';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { AccountService } from 'src/app/_services/account.service';
import { ConfirmEmailPhone } from 'src/app/_models/confirmEmailPhone';
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
  username: string;
  pwd: string;

  constructor(private authService: AuthService, private fb: FormBuilder, private route: ActivatedRoute,
    private alertify: AlertifyService,  private router: Router, private accountService: AccountService) { }

  ngOnInit() {
    // this.route.data.subscribe(data => {
    //   this.user = data['user'].user;
    // });
    this.createPhoneForm();
    this.createUserForm();

    const id: string = this.route.snapshot.queryParamMap.get('id');
    const orderid: string = this.route.snapshot.queryParamMap.get('orderid');
    const token: string = this.route.snapshot.queryParamMap.get('token');
    if (id !== null && token !== null) {
      const confirmEmail = <ConfirmEmailPhone>{};
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
    this.userForm.setValue({lastName: this.user.lastName, firstName: this.user.firstName, email: this.user.email,
      userName: '', cell: this.user.phoneNumber, pwd: '', checkpwd: ''});
    // console.log('phone:' + this.user.phoneNumber);
    this.phoneForm.setValue({phone: this.user.phoneNumber, code: ''});
  }

  changePhone(phonenum) {
    this.userForm.get('cell').setValue(phonenum);
  }

  sendPhoneNumber() {
    this.accountService.sendPhoneNumberToValidate(this.user.id, this.phoneForm.value.phone).subscribe(codeSent => {
      if (codeSent) {
        this.phoneValidationSteps = 1;
      }
    }, error => {
      this.alertify.error(error);
    });
  }

  sendEmailToConfirm() {

  }

  validateEmail(confirmEmail: ConfirmEmailPhone) {
    this.accountService.confirmEmail(confirmEmail).subscribe((data: any) => {
      this.emailOK = data.emailOK;
      this.user = data.user;
      this.children = data.children;
      this.parentOk = this.user.validated;
      this.validAccount = data.accountValidated;
      this.childrenOk = data.childrenValidated;
      this.initialValues();
    }, error => {
      this.alertify.error(error);
    });
  }

  validatePhone() {
    const confirmPhone = <ConfirmEmailPhone>{};
    confirmPhone.userId = this.user.id.toString();
    confirmPhone.token = this.phoneForm.value.code;
    this.accountService.validtePhone(confirmPhone).subscribe((data: any) => {
      this.user = data.user;
      this.phoneOk = data.phoneOk;
      if (this.phoneOk) {
        this.phoneValidationSteps = 2;
      }
    }, error => {
      this.alertify.error(error);
    });
  }

  userNameVerification() {
    const userName = this.userForm.value.userName;
    this.userNameExist = false;
    this.authService.userNameExist(userName).subscribe((res: boolean) => {
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
    this.authService.setUserLoginPassword(this.user.id, this.userForm.value).subscribe(() => {
      this.parentOk = true;
      this.childrenOk = true;
      this.validAccount = this.user.validated;
      for (let i = 0; i < this.children.length; i++) {
        const child = this.children[i];
        if (child.validated !== true) {
          this.childrenOk = false;
          break;
        }
      }
    }, error => {
      this.alertify.error(error);
    });
  }

  updateAccount() {
    this.authService.getUser(this.user.id).subscribe((parent: User) => {
      this.user = parent;
      this.validAccount = this.user.validated;
      this.childrenOk = true;
    }, error => {
      this.alertify.error(error);
    });
  }

}
