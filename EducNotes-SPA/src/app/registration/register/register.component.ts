import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { AuthService } from '../../_services/auth.service';
import { AlertifyService } from '../../_services/alertify.service';
import { FormGroup,  Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { User } from '../../_models/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
 user: User ;
 registerForm: FormGroup;
 bsConfig: Partial<BsDatepickerConfig>;
 isReadOnly: boolean ;


  constructor(private authService: AuthService, private alert: AlertifyService,
    private fb: FormBuilder, private router: Router) { }

    ngOnInit() {
      this.bsConfig = {
        containerClass: 'theme-red'
      };
      this.createRegisterForm();
    }

    createRegisterForm() {
      this.registerForm = this.fb.group({
        gender: ['1'],
        lastName: ['', Validators.required],
        firstName: ['', Validators.required],
        dateOfBirth: [null, Validators.required],
         email: ['', [Validators.required, Validators.email]],
        password: ['', [Validators.required, Validators.minLength(4)]],
        confirmPassword: ['', Validators.required]
      }, {validator: this.passwordMatchValidator});
    }



  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value ? null : {'mismatch': true};
  }
  register() {
    this.isReadOnly = true;
    if (this.registerForm.valid) {
      this.user = Object.assign({}, this.registerForm.value);
      this.authService.register(this.user).subscribe(() => {
        this.alert.success('enregistrement terminé...');
        this.user.emailConfirmed = false;
        this.authService.newUser = this.user;

      }, error => {
        this.alert.error(error);
    this.isReadOnly = false;

      }, () => {
        this.router.navigate(['/firstFinishment']);
      });
    }

    }

  cancel() {
    this.cancelRegister.emit(false);
    console.log('cancelled');
  }

}
