import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router, ActivatedRoute } from '@angular/router';
import { UserService } from 'src/app/_services/user.service';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { ConfirmToken } from 'src/app/_models/confirmToken';
import { AccountService } from 'src/app/_services/account.service';

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
  wait = false;

  constructor(private accountService: AccountService, private fb: FormBuilder, private route: ActivatedRoute,
     private alertify: AlertifyService,  private router: Router, private userService: UserService) { }

  ngOnInit() {
    this.userid = this.route.snapshot.queryParamMap.get('id');
    this.token = this.route.snapshot.queryParamMap.get('token');
    // console.log(id);
    // console.log(token);
    if (this.userid !== null && this.token !== null) {
      this.dataAvailable = true;
    }
    this.createResetPwdForm();
  }

  createResetPwdForm() {
    this.resetPwdForm = this.fb.group({
      password: ['', Validators.required],
      confirm: ['', [Validators.required, this.confirmationValidator]]
    });
  }

  confirmationValidator = (control: FormControl): { [ s: string ]: boolean } => {
    if (!control.value) {
      return { required: true };
    } else if (control.value !== this.resetPwdForm.controls.password.value) {
      return { confirm: true, error: true };
    }
  }

  savePassword() {
    const resetPwdData = <ConfirmToken>{};
    resetPwdData.userId = this.userid;
    resetPwdData.token = this.token;
    resetPwdData.password = this.resetPwdForm.value.password;
    this.accountService.resetPassword(resetPwdData).subscribe((valid: boolean) => {
      
    }, error => {
      this.alertify.error(error);
    });
  }

  resetPassword() {

  }

  resultMode(val: boolean) {
    if (val) {
      this.alertify.success('votre mot de passe a bien été modifié');
      this.router.navigate(['/Home']);
    } else {
      this.alertify.success('erreur survenue');

    }
  }

}
