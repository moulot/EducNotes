import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import { User } from 'src/app/_models/user';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute, Router } from '@angular/router';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { AccountService } from 'src/app/_services/account.service';
import { ConfirmToken } from 'src/app/_models/confirmToken';
import { Utils } from 'src/app/shared/utils';

@Component({
  selector: 'app-confirm-teacher-email',
  templateUrl: './confirm-teacher-email.component.html',
  styleUrls: ['./confirm-teacher-email.component.scss'],
  animations: [SharedAnimations]
})
export class ConfirmTeacherEmailComponent implements OnInit {
  user: User;
  userForm: FormGroup;
  phoneForm: FormGroup;
  userNameExist = false;
  emailOK = false;
  phoneMask = Utils.phoneMask;
  validAccount = false;
  phoneValidationSteps = 0;
  phoneOk = true;
  teacherOk = false;
  userid: any;
  userToken: any;
  username: string;
  pwd: string;
  wait = false;

  constructor(private authService: AuthService, private fb: FormBuilder, private route: ActivatedRoute,
    private alertify: AlertifyService, private accountService: AccountService) { }

  ngOnInit() {
    this.authService.eraseSessionData();
    this.createPhoneForm();
    this.createUserForm();

    this.userid = this.route.snapshot.queryParamMap.get('id');
    this.userToken = this.route.snapshot.queryParamMap.get('token');
    if (this.userid !== null && this.userToken !== null) {
      const confirmEmail = <ConfirmToken>{};
      confirmEmail.userId = this.userid;
      confirmEmail.token = this.userToken;
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
      userName: ['', Validators.required],
      pwd: ['', Validators.required],
      checkpwd: ['', [ Validators.required, this.confirmationValidator ] ]
    });
  }

  initialValues() {
    this.userForm.setValue({lastName: this.user.lastName, firstName: this.user.firstName, userName: '', pwd: '', checkpwd: ''});
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
    this.authService.sendTeacherConfirmEmail(this.userid).subscribe((emailSent: Boolean) => {
      if (emailSent) {
        // this.router.navigate(['EmailSent']);
        this.alertify.success('le email vous a été envoyé avec succès');
      } else {
        this.alertify.info('le email n\'a pu être envoyé, recommencez svp');
      }
    }, error => {
      this.alertify.error('problème pour envoyer le email. recommencez svp.');
    });
  }

  validateEmail(confirmEmail: ConfirmToken) {
    this.wait = true;
    this.accountService.confirmEmail(confirmEmail).subscribe((data: any) => {
      this.emailOK = data.emailOK;
      this.user = data.user;
      this.teacherOk = this.user.validated;
      this.validAccount = data.accountValidated;
      this.initialValues();
      this.wait = false;
    }, error => {
      this.alertify.error(error);
      this.wait = false;
    });
  }

  validatePhone() {
    const confirmPhone = <ConfirmToken>{};
    confirmPhone.userId = this.user.id.toString();
    confirmPhone.token = this.phoneForm.value.code;
    this.accountService.validatePhone(confirmPhone).subscribe((data: any) => {
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
    this.authService.userNameExist(userName, this.userid).subscribe((res: boolean) => {
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
    } else if (control.value.length !== 10) {
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
    this.authService.setUserAccountData(this.user.id, this.userForm.value).subscribe(() => {
      this.teacherOk = true;
      this.validAccount = this.user.validated;
    }, error => {
      this.alertify.error(error);
    });
  }

  updateAccount() {
    this.authService.getUser(this.user.id).subscribe((teacher: User) => {
      this.user = teacher;
      this.validAccount = this.user.validated;
    }, error => {
      this.alertify.error(error);
    });
  }

}
