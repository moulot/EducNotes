import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { AuthService } from './_services/auth.service';
import {JwtHelperService} from '@auth0/angular-jwt';
import { User } from './_models/user';
import { Period } from './_models/period';
import { PerfectScrollbarDirective } from 'ngx-perfect-scrollbar';
import { NavigationService } from './shared/services/navigation.service';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  jwtHelper = new JwtHelperService();
  @ViewChild(PerfectScrollbarDirective, { static: true }) perfectScrollbar: PerfectScrollbarDirective;
  userActivity;
  userInactive: Subject<any> = new Subject();

  constructor(public navService: NavigationService, private authService: AuthService) {
    // this.setTimeout();
    // this.userInactive.subscribe(() => console.log('user has been inactive for 3s'));
  }


  setTimeout() {
    this.userActivity = setTimeout(() => this.userInactive.next(undefined), 3000);
  }

  @HostListener('window:mousemove') refreshUserState() {
    clearTimeout(this.userActivity);
    this.setTimeout();
  }

  ngOnInit() {
    const token = localStorage.getItem('token');
    const user: User = JSON.parse(localStorage.getItem('user'));
    const currentPeriod: Period = JSON.parse(localStorage.getItem('currentPeriod'));
    const currentChild: User = JSON.parse(localStorage.getItem('currentChild'));
    const currentClassId = localStorage.getItem('currentClassId');
    if (token) {
      this.authService.decodedToken = this.jwtHelper.decodeToken(token);
    }
    if (user) {
      this.authService.currentUser = user;
      this.authService.currentPeriod = currentPeriod;
      this.authService.changeCurrentChild(currentChild);
      this.authService.changeUserPhoto(user.photoUrl);
      this.authService.changeCurrentClassId(Number(currentClassId));
    }
  }

  loggedIn() {
    return this.authService.loggedIn();
  }
}
