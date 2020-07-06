import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-invalid-account',
  templateUrl: './invalid-account.component.html',
  styleUrls: ['./invalid-account.component.scss']
})
export class InvalidAccountComponent implements OnInit {
  user: User;
  phoneForm: FormGroup;
  // emailForm: FormGroup;

  constructor(private authService: AuthService, private fb: FormBuilder) { }

  ngOnInit() {
    this.user = this.authService.currentUser;
    this.createPhoneForm();
    this.phoneForm.setValue({phone: this.user.phoneNumber});
  }

  createPhoneForm() {
    this.phoneForm = this.fb.group({
      phone: ['', Validators.required]
    });
  }

  // createEmailForm() {
  //   this.emailForm = this.fb.group({
  //     email: ['', Validators.required]
  //   });
  // }

  validatePhone() {

  }

}
