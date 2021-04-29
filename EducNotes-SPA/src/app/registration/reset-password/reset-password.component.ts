import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router, ActivatedRoute } from '@angular/router';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { ConfirmToken } from 'src/app/_models/confirmToken';
import { AccountService } from 'src/app/_services/account.service';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss'],
  animations: [SharedAnimations]
})
export class ResetPasswordComponent implements OnInit {
  dataAvailable = false;
  userid: string;
  token: string;
  resetPwdForm: FormGroup;
  resetPwdError = false;
  pwdConfirmed = false;
  userName: string;
  userGender: number;
  userNameExist = false;
  wait = false;

  constructor(private accountService: AccountService, private fb: FormBuilder, private route: ActivatedRoute,
     private alertify: AlertifyService, private router: Router, private authService: AuthService) { }

  ngOnInit() {
    this.userid = this.route.snapshot.queryParamMap.get('id');
    this.token = this.route.snapshot.queryParamMap.get('token');
    if (this.userid !== null && this.token !== null) {
      this.dataAvailable = true;
    }
    this.createResetPwdForm();
  }

  createResetPwdForm() {
    this.resetPwdForm = this.fb.group({
      userName: ['', Validators.required],
      password: ['', Validators.required],
      confirmPwd: ['', [Validators.required, this.confirmationValidator]]
    });
  }

  confirmationValidator = (control: FormControl): { [ s: string ]: boolean } => {
    if (!control.value) {
      return { required: true };
    } else if (control.value !== this.resetPwdForm.controls.password.value) {
      return { confirmNOK: true, error: true };
    }
  }

  userNameVerification() {
    const userName = this.resetPwdForm.value.userName;
    this.userNameExist = false;
    this.authService.userNameExist(userName, Number(this.userid)).subscribe((res: boolean) => {
      if (res === true) {
        this.userNameExist = true;
      }
    });
  }

  resetPassword() {
    const resetPwdData = <ConfirmToken>{};
    resetPwdData.userId = this.userid;
    resetPwdData.token = this.token;
    resetPwdData.userName = this.resetPwdForm.value.userName;
    resetPwdData.password = this.resetPwdForm.value.password;
    this.accountService.resetPassword(resetPwdData).subscribe((data: any) => {
      this.pwdConfirmed = data.success;
      this.userName = data.userName;
      this.userGender = data.gender;
      if (!this.pwdConfirmed) {
        this.alertify.error('problème pour enregistrer le nouveau mot de passe');
        this.resetPwdError = true;
      }
    }, error => {
      this.alertify.error(error);
    });
  }

}
