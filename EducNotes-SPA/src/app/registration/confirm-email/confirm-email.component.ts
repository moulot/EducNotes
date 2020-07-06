import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import { User } from 'src/app/_models/user';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router, ActivatedRoute } from '@angular/router';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { AccountService } from 'src/app/_services/account.service';
import { ConfirmEmail } from 'src/app/_models/confirmEmail';
import { Utils } from 'src/app/shared/utils';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.css'],
  animations: [SharedAnimations]
})
export class ConfirmEmailComponent implements OnInit {
  user: User;
  userForm: FormGroup;
  userNameExist = false;
  emailOK = false;
  phoneMask = Utils.phoneMask;
  validAccount = false;

  constructor(private authService: AuthService, private fb: FormBuilder, private route: ActivatedRoute,
    private alertify: AlertifyService,  private router: Router, private accountService: AccountService) { }

  ngOnInit() {
    // this.route.data.subscribe(data => {
    //   this.user = data['user'].user;
    // });
    this.createUserForm();

    const id: string = this.route.snapshot.queryParamMap.get('id');
    const orderid: string = this.route.snapshot.queryParamMap.get('orderid');
    const token: string = this.route.snapshot.queryParamMap.get('token');
    if (id !== null && token !== null) {
      const confirmEmail = <ConfirmEmail>{};
      confirmEmail.userId = id;
      confirmEmail.token = token;
      this.validateEmail(confirmEmail);
    }
  }

  createUserForm() {
    this.userForm = this.fb.group({
      lastName: ['', Validators.required],
      firstName: ['', Validators.required],
      email: ['', Validators.required],
      userName: ['', Validators.required],
      cell: ['', Validators.required],
      pwd: ['', Validators.required],
      checkpwd: ['', [ Validators.required, this.confirmationValidator ] ]
    });
  }

  initialValues() {
    this.userForm.setValue({lastName: this.user.lastName, firstName: this.user.firstName, email: this.user.email,
      userName: '', cell: this.user.phoneNumber, pwd: '', checkpwd: ''});
  }

  sendEmailToConfirm() {
    
  }

  validateEmail(confirmEmail: ConfirmEmail) {
    this.accountService.confirmEmail(confirmEmail).subscribe((data: any) => {
      this.emailOK = data.emailOK;
      this.user = data.user;
      this.validAccount = data.accountValidated;
      this.initialValues();
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

  resultMode(val: boolean) {
    if (val) {
      this.alertify.success('votre mot de passe a bien été enregistré');
      this.router.navigate(['/Home']);
    } else {
      this.alertify.error('erreur survenue');
    }
  }

  confirmationValidator = (control: FormControl): { [ s: string ]: boolean } => {
    if (!control.value) {
      return { required: true };
    } else if (control.value !== this.userForm.controls.pwd.value) {
      return { confirm: true, error: true };
    }
  }

  updateConfirmValidator(): void {
    /** wait for refresh value */
    Promise.resolve().then(() => this.userForm.controls.checkPassword.updateValueAndValidity());
  }

  updateUser(): void {
    this.authService.setUserLoginPassword(this.user.id, this.userForm.value).subscribe(() => {
      this.router.navigate(['userAccount', this.user.id]);
    }, error => {
      this.alertify.error(error);
    });
  }

}
