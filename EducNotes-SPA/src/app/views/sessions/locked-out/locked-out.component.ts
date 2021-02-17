import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-locked-out',
  templateUrl: './locked-out.component.html',
  styleUrls: ['./locked-out.component.scss']
})
export class LockedOutComponent implements OnInit {
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
    this.authService.forgotPassord(this.resetPwdForm.value).subscribe(res => {
      this.alertify.success('email de re-initialisation envoyé');
      this.emailSent = true;
    }, error => {
      this.alertify.error(error);
    });
  }
}
