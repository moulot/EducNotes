import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from 'src/app/_services/auth.service';

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
