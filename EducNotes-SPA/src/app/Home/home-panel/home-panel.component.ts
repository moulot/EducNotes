import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../_services/auth.service';
import { AlertifyService } from '../../_services/alertify.service';
import { Router, ActivatedRoute } from '@angular/router';
import { ClassService } from '../../_services/class.service';
import { User } from '../../_models/user';
import { environment } from 'src/environments/environment';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-home-panel',
  templateUrl: './home-panel.component.html',
  styleUrls: ['./home-panel.component.css']
})
export class HomePanelComponent implements OnInit {
  studentTypeId: number = environment.studentTypeId;
  teacherTypeId: number = environment.teacherTypeId;
  parentTypeId: number = environment.parentTypeId;
  adminTypeId: number = environment.adminTypeId;
  user: User;
  currentChild: User;
  userIsParent = true;
  childSelected = false;

  constructor(private http: HttpClient, public authService: AuthService,
      private alertify: AlertifyService, private router: Router,
      private userService: UserService, private classService: ClassService,
      private route: ActivatedRoute) { }

  ngOnInit() {
    this.user = this.authService.currentUser;
    // if (this.parentLoggedIn()) {
    //   this.userIsParent = true;
    // } else {
    //   this.userIsParent = false;
    // }
  }

  loggedIn() {
    return this.authService.loggedIn();
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

  // registerToggle() {
  //   this.registerMode = !this.registerMode;
  // }

  // cancelRegisterMode(registerMode: boolean) {
  //   this.registerMode = registerMode;
  // }
}
