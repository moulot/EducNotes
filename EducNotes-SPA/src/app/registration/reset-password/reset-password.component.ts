import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';
import { User } from 'src/app/_models/user';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router, ActivatedRoute } from '@angular/router';
import { UserService } from 'src/app/_services/user.service';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss'],
  animations: [SharedAnimations]
})
export class ResetPasswordComponent implements OnInit {

  user: User;
  // registerForm: FormGroup;
 // isReadOnly: boolean ;
 userTypes: any;


  constructor(private authService: AuthService, private fb: FormBuilder, private route: ActivatedRoute,
     private alertify: AlertifyService,  private router: Router, private userService: UserService) { }

  ngOnInit() {

    this.route.data.subscribe(data => {
      this.user = data['user'].user;
      });
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
