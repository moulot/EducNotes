import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-forgot',
  templateUrl: './forgot.component.html',
  styleUrls: ['./forgot.component.scss'],
  animations: [SharedAnimations]
})
export class ForgotComponent implements OnInit {
  email: string;
  resetPwdForm: FormGroup;

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
    }, error => {
      this.alertify.error(error);
    });
  }

}
