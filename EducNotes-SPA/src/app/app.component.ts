import { Component, OnInit, ViewChild } from '@angular/core';
import { AuthService } from './_services/auth.service';
import {JwtHelperService} from '@auth0/angular-jwt';
import { User } from './_models/user';
import { Period } from './_models/period';
import { PerfectScrollbarDirective } from 'ngx-perfect-scrollbar';
import { NavigationService } from './shared/services/navigation.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  jwtHelper = new JwtHelperService();
  @ViewChild(PerfectScrollbarDirective, { static: true }) perfectScrollbar: PerfectScrollbarDirective;

  constructor(public navService: NavigationService, private authService: AuthService) {}

  ngOnInit() {
    const token = localStorage.getItem('token');
    const user: User = JSON.parse(localStorage.getItem('user'));
    const currentPeriod: Period = JSON.parse(localStorage.getItem('currentPeriod'));
    const currentChild: User = JSON.parse(localStorage.getItem('currentChild'));
    if (token) {
      this.authService.decodedToken = this.jwtHelper.decodeToken(token);
    }
    if (user) {
      this.authService.currentUser = user;
      this.authService.currentPeriod = currentPeriod;
      this.authService.changeCurrentChild(currentChild);
      this.authService.changeMemberPhoto(user.photoUrl);
    }
  }

  loggedIn() {
    return this.authService.loggedIn();
  }
}
