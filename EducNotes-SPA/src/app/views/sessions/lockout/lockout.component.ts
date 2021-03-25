import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-lockout',
  templateUrl: './lockout.component.html',
  styleUrls: ['./lockout.component.scss']
})
export class LockoutComponent implements OnInit {
  resetPwdForm: FormGroup;
  emailSent = false;

  constructor(private authService: AuthService, private alertify: AlertifyService, private fb: FormBuilder) { }

  ngOnInit() {
    this.createResetPwdForm();
  }

  createResetPwdForm() {
    this.resetPwdForm = this.fb.group({
      username: ['', Validators.required],
      email: ['', Validators.required]
    });
  }

  sendLink() {
    this.authService.forgotPassord(this.resetPwdForm.value).subscribe(resetOk => {
      if (resetOk) {
        this.alertify.success('email de re-initialisation envoyé');
        this.emailSent = true;
      } else {
        this.alertify.warning('l\'utilisateur ou/et le email n\'existent pas. recommencez svp.');
      }
    }, error => {
      console.log(error);
    });
  }
}
