import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Utils } from 'src/app/shared/utils';
import { City } from 'src/app/_models/city';
import { ConfirmToken } from 'src/app/_models/confirmToken';
import { AccountService } from 'src/app/_services/account.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-edit-account',
  templateUrl: './edit-account.component.html',
  styleUrls: ['./edit-account.component.scss']
})
export class EditAccountComponent implements OnInit {
  userid: any;
  user: any;
  userNameExist = false;
  userForm: FormGroup;
  validEmailRegex = Utils.validEmailRegex;
  wait = false;
  gender = [{value: 0, label: 'femme'}, {value: 1, label: 'homme'}];
  myDatePickerOptions = Utils.myDatePickerOptions;
  photoUrl = '';
  photoFile: File;
  updatePwd = false;
  dob: any;
  emailEdited = false;
  emailToken: any;
  phoneEdited = false;
  pwdEdited = false;
  phoneValidationSteps = 0;
  emailValidationSteps = 0;
  pwdValidationSteps = 0;
  phoneOk: Boolean = true;
  emailOk: Boolean = true;
  pwdOk: Boolean = true;
  cityOptions = [];
  districtOptions = [];
  showInfoBox = false;
  alertInfoTitle: string;
  alertInfo: string;

  constructor(private userService: UserService, private route: ActivatedRoute, private fb: FormBuilder,
    private alertify: AlertifyService, private authService: AuthService, private router: Router,
    private accountService: AccountService) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.userid = params['id'];
    });

    this.route.data.subscribe(data => {
      this.user = data['user'];
      this.getCities();
      if (!this.user.phoneNumber) {
        this.user.phoneNumber = '';
      }
      if (this.user.photoUrl) {
        this.photoUrl = this.user.photoUrl;
      }
      if (this.user.strDateOfBirth === '01/01/0001') {
        this.dob = '';
      } else {
        this.dob = this.user.strDateOfBirth;
      }
    });

    this.createUserForm();
  }

  createUserForm() {
    this.userForm = this.fb.group({
      lastName: [this.user.lastName, Validators.required],
      firstName: [this.user.firstName, Validators.required],
      userName: [this.user.userName, Validators.required],
      password: ['', Validators.required],
      oldPassword: ['', Validators.required],
      checkpwd: ['', [ Validators.required, this.pwdValidator ] ],
      dateOfBirth: [this.dob, Validators.required],
      gender: [this.user.gender, Validators.required],
      email: [this.user.email, [Validators.required, Validators.email ] ],
      cell: [this.user.phoneNumber, [Validators.required, this.phoneValidator] ],
      cityId: [''],
      districtId: [''],
      code: ['', [ this.codeValidator ] ],
      emailCode: ['', [ this.codeValidator ] ],
      pwdCode: ['', [ this.codeValidator ] ],
      phone2: [this.user.secondPhoneNumber, Validators.nullValidator],
      photoUrl: [this.user.photoUrl],
      pwdActive: [false]
    });
  }

  getCities() {
    this.authService.getAllCities().subscribe((res: City[]) => {
      for (let i = 0; i < res.length; i++) {
        const element = { value: res[i].id, label: res[i].name };
        this.cityOptions = [...this.cityOptions, element];
      }
    });
  }

  getCityDistricts() {
    const id = this.userForm.value.cityId;
    if (id) {
      this.userForm.value.cityId = '';
      this.authService.getDistrictsByCityId(id).subscribe((res: any[]) => {
        for (let i = 0; i < res.length; i++) {
          const elt = { value: res[i].id, label: res[i].name };
          this.districtOptions = [...this.districtOptions, elt];
        }
      }, error => {
        this.alertify.error(error);
      });
    }
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

  pwdValidator = (control: FormControl): { [ s: string ]: boolean } => {
    if (!control.value) {
      return { required: true };
    } else if (control.value !== this.userForm.controls.password.value) {
      return { confirmNOK: true };
    }
    return null;
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

  imgResult(event) {
    let file: File = null;
    file = <File>event.target.files[0];
    // recuperation de l'url de la photo
    const reader = new FileReader();
    reader.onload = (e: any) => {
      this.photoUrl = e.target.result;
    };
    reader.readAsDataURL(event.target.files[0]);
    this.photoFile = file;
  }

  showHidePwdEdit() {
    this.updatePwd = !this.updatePwd;
  }

  emailChanged() {
    const oldEmail = this.user.email;
    const newEmail = this.userForm.value.email;
    if (oldEmail !== newEmail && !this.userForm.get('email').errors) {
      this.emailValidationSteps = 0;
      this.emailEdited = true;
    } else {
      this.emailEdited = false;
    }
  }

  phoneChanged() {
    const oldPhone = this.user.phoneNumber;
    const newPhone = this.userForm.value.cell;
    if (oldPhone !== newPhone && newPhone.length === 10) {
      this.phoneValidationSteps = 0;
      this.phoneEdited = true;
    } else {
      this.phoneEdited = false;
    }
  }

  pwdChanged() {
    if (!this.userNameExist && !this.userForm.get('password').errors && !this.userForm.get('oldPassword').errors &&
      ((this.userForm.value.password).toLowerCase() === (this.userForm.value.checkpwd).toLowerCase())) {
      this.pwdValidationSteps = 0;
      this.pwdEdited = true;
    } else {
      this.pwdEdited = false;
    }
  }

  changePhone(phonenum) {
    this.userForm.get('cell').setValue(phonenum);
  }

  sendPwdCodeToMobile() {
    this.wait = true;
    const confirmEmail = <ConfirmToken>{};
    confirmEmail.userId = this.user.id.toString();
    confirmEmail.phoneNumber = this.user.phoneNumber;
    this.accountService.sendPwdCodeToSms(confirmEmail).subscribe(codeSent => {
      if (codeSent) {
        this.pwdValidationSteps = 1;
      }
      this.wait = false;
    }, error => {
      this.alertify.error(error);
      this.wait = false;
    });
  }

  validatePwd() {
    this.wait = true;
    const confirmPwd = <ConfirmToken>{};
    confirmPwd.userId = this.user.id.toString();
    confirmPwd.token = this.userForm.value.pwdCode;
    confirmPwd.userName = this.userForm.value.userName;
    confirmPwd.oldPassword = this.userForm.value.oldPassword;
    confirmPwd.password = this.userForm.value.password;
    this.accountService.editPwd(confirmPwd).subscribe((pwdOk: Boolean) => {
      this.pwdOk = pwdOk;
      if (this.pwdOk) {
        this.emailValidationSteps = 2;
        this.userForm.get('emailCode').setValue('');
        this.user.email = this.userForm.value.email;
        this.alertInfoTitle = 'votre email \'' + this.user.email + '\'est validé!';
        this.alertInfo = 'votre email a été modifié avec succès. merci';
        this.showAlert();
        } else {
        this.alertify.error('nous avons eu un problème pour modifier votre email.');
      }
      this.wait = false;
    }, error => {
      this.alertify.error(error);
      this.wait = false;
    });
  }

  sendCodeToEmail() {
    this.wait = true;
    const confirmEmail = <ConfirmToken>{};
    confirmEmail.userId = this.user.id.toString();
    confirmEmail.phoneNumber = this.user.phoneNumber;
    confirmEmail.email = this.userForm.value.email;
    this.accountService.sendCodeToEmail(confirmEmail).subscribe(codeSent => {
      if (codeSent) {
        this.emailValidationSteps = 1;
      }
      this.wait = false;
    }, error => {
      this.alertify.error(error);
      this.wait = false;
    });
  }

  validateEmail() {
    this.wait = true;
    const confirmEmail = <ConfirmToken>{};
    confirmEmail.userId = this.user.id.toString();
    confirmEmail.token = this.userForm.value.emailCode;
    confirmEmail.email = this.userForm.value.email;
    this.accountService.editEmail(confirmEmail).subscribe((emailOk: Boolean) => {
      this.emailOk = emailOk;
      if (this.emailOk) {
        this.emailValidationSteps = 2;
        this.userForm.get('emailCode').setValue('');
        this.user.email = this.userForm.value.email;
        this.alertInfoTitle = 'votre email \'' + this.user.email + '\'est validé!';
        this.alertInfo = 'votre email a été modifié avec succès. merci';
        this.showAlert();
        } else {
        this.alertify.error('nous avons eu un problème pour modifier votre email.');
      }
      this.wait = false;
    }, error => {
      this.alertify.error(error);
      this.wait = false;
    });
  }

  sendPhoneNumber() {
    this.wait = true;
    this.accountService.sendPhoneNumberToValidate(this.user.id, this.userForm.value.cell).subscribe(codeSent => {
      if (codeSent) {
        this.phoneValidationSteps = 1;
      }
      this.wait = false;
    }, error => {
      this.alertify.error(error);
      this.wait = false;
    });
  }

  validatePhone() {
    this.wait = true;
    const confirmPhone = <ConfirmToken>{};
    confirmPhone.userId = this.user.id.toString();
    confirmPhone.token = this.userForm.value.code;
    confirmPhone.phoneNumber = this.userForm.value.cell;
    this.accountService.editPhoneNumber(confirmPhone).subscribe((phoneNumOk: Boolean) => {
      this.phoneOk = phoneNumOk;
      if (this.phoneOk) {
        this.phoneValidationSteps = 2;
        this.userForm.get('code').setValue('');
        this.user.phoneNumber = this.userForm.value.cell;
        this.alertInfoTitle = 'votre numéro mobile \'' + Utils.formatPhoneNumber(this.user.phoneNumber) + '\'est validé!';
        this.alertInfo = 'votre numéro mobile a été modifié avec succès. merci';
        this.showAlert();
        } else {
        this.alertify.error('nous avons eu un problème pour modifier votre numéro.');
      }
      this.wait = false;
    }, error => {
      this.alertify.error(error);
      this.wait = false;
    });
  }

  addUser() {
    this.wait = true;
    const formData = new FormData();
    if (this.photoFile) {
      formData.append('photoFile', this.photoFile, this.photoFile.name);
    }
    formData.append('id', this.user.id.toString());
    formData.append('lastName', this.userForm.value.lastName);
    formData.append('firstName', this.userForm.value.firstName);
    formData.append('gender', this.userForm.value.gender);
    formData.append('strDateOfBirth', this.userForm.value.dateOfBirth);
    formData.append('email', this.userForm.value.email);
    formData.append('phoneNumber', this.userForm.value.cell);
    formData.append('secondPhoneNumber', this.userForm.value.phone2);
    // formData.append('userName', this.userForm.value.userName);
    // formData.append('password', this.userForm.value.password);
    // formData.append('pwdChanged', this.userForm.value.pwdActive);
    formData.append('oldEmail', this.user.email);
    formData.append('oldPhoneNumber', this.user.phoneNumber);
    formData.append('cityId', this.userForm.value.cityId);
    formData.append('districtId', this.userForm.value.districtId);

    this.userService.editUserAccountr(formData).subscribe((updateOK: boolean) => {
      if (updateOK) {
        this.alertify.success('le compte a été mis à jour avec succès');
        this.router.navigate(['/userAccount', this.user.id]);
      } else {
        this.alertify.error('problème pour editer le compte utilisateur.');
      }
    }, error => {
      this.alertify.error(error);
      this.wait = false;
    });
  }

  showAlert() {
    this.showInfoBox = true;
    setTimeout(() => this.showInfoBox = false, 5000); // soit 15 minutes d'inactivité
  }
}
