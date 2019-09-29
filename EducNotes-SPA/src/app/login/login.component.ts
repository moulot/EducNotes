import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { Router, RouteConfigLoadStart, RouteConfigLoadEnd, ResolveStart, ResolveEnd } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { User } from '../_models/user';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  model: any = {};
  user: User;
  verifyModel: any = {};
  notConfirmed: boolean;
  showValidationForm: boolean;
  isVisible: boolean;
  passwordForm: FormGroup;
  signinForm: FormGroup;
  loading: boolean;
  loadingText: string;

  constructor(private authService: AuthService, private router: Router, private fb: FormBuilder,
    private alertify: AlertifyService) { }

  ngOnInit() {
    this.router.events.subscribe(event => {
      if (event instanceof RouteConfigLoadStart || event instanceof ResolveStart) {
          this.loadingText = 'Loading Dashboard Module...';

          this.loading = true;
      }
      if (event instanceof RouteConfigLoadEnd || event instanceof ResolveEnd) {
          this.loading = false;
      }
    });

    this.signinForm = this.fb.group({
      email: ['', Validators.required],
      password: ['', Validators.required]
    });

  this.showValidationForm = false;
    this.isVisible = false;

    this.passwordForm = this.fb.group({
      password: ['', [Validators.required, Validators.minLength(4)]],
      confirmPassword: ['', Validators.required]
    }, {validator: this.passwordMatchValidator});
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value ? null : {'mismatch': true};
  }

  login() {
    this.notConfirmed = false;
    this.authService.login(this.signinForm.value).subscribe((response) => {
    }, error => {
      this.alertify.error(error);
    }, () => {
      this.router.navigate(['/home']);
    });
  }

verification() {
  this.authService.codeValidation(this.verifyModel).subscribe((response: User) => {
    this.isVisible = true;
    this.user = response;
  }, error => {
    this.alertify.error(error);
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

  connetionShow() {
    this.showValidationForm = false;
  }

  activationShow() {
    this.showValidationForm = true;
  }

  handleOk(): void {

  }

  handleCancel(): void {
    this.isVisible = false;
  }

  savePassword() {
    this.authService.setUserPassword(this.user.id, this.passwordForm.value).subscribe((response) => {
    }, error => {
      this.alertify.error(error);
    }, () => {
      this.router.navigate(['/home']);
    });
  }

}
