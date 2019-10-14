import { Component, OnInit, EventEmitter, Output, Input } from '@angular/core';
import { User } from 'src/app/_models/user';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-member-password-setting',
  templateUrl: './member-password-setting.component.html',
  styleUrls: ['./member-password-setting.component.css']
})
export class MemberPasswordSettingComponent implements OnInit {
  passwordForm: FormGroup;
  @Input() user: User;

  @Output() passwordSetingResult = new EventEmitter();

  constructor(private fb: FormBuilder, private authService: AuthService) {
  }

  ngOnInit(): void {
    this.passwordForm = this.fb.group({
      password         : [ null, [ Validators.required ] ],
      checkPassword    : [ null, [ Validators.required, this.confirmationValidator ] ],
    });
  }

  submitForm(): void {
    // post de mot de passse;

    this.authService.setUserPassword(this.user.id, this.passwordForm.value.password).subscribe(() => {
      this.passwordSetingResult.emit(true);
      }, error => {
        this.passwordSetingResult.emit(false);
      });

  }

  confirmationValidator = (control: FormControl): { [ s: string ]: boolean } => {
    if (!control.value) {
      return { required: true };
    } else if (control.value !== this.passwordForm.controls.password.value) {
      return { confirm: true, error: true };
    }
  }

  updateConfirmValidator(): void {
    /** wait for refresh value */
    Promise.resolve().then(() => this.passwordForm.controls.checkPassword.updateValueAndValidity());
  }





}
