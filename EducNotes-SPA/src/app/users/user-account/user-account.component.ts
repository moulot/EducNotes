import { Component, OnInit } from '@angular/core';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from 'src/app/_services/user.service';
import { Utils } from 'src/app/shared/utils';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { AuthService } from 'src/app/_services/auth.service';
import { OrderToValidate } from 'src/app/_models/orderToValidate';
import { OrderlineToValidate } from 'src/app/_models/orderlineToValidate';

@Component({
  selector: 'app-user-account',
  templateUrl: './user-account.component.html',
  styleUrls: ['./user-account.component.scss']
})
export class UserAccountComponent implements OnInit {

  constructor(private authService: AuthService, private route: ActivatedRoute) { }
  userid: any;

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.userid = params['id'];
    });
  }

  adminLoggedIn() {
    return this.authService.adminLoggedIn();
  }
  parentLoggedIn() {
    return this.authService.parentLoggedIn();
  }

  studentLoggedIn() {
    return this.authService.studentLoggedIn();
  }

  teacherLoggedIn() {
    return this.authService.teacherLoggedIn();
  }
}
