import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { AuthService } from './_services/auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { User } from './_models/user';
import { Period } from './_models/period';
import { PerfectScrollbarDirective } from 'ngx-perfect-scrollbar';
import { Subject } from 'rxjs';
import { MDBSpinningPreloader } from 'ng-uikit-pro-standard';
import { Router } from '@angular/router';
import { Setting } from './_models/setting';

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

  constructor(private mdbSpinningPreloader: MDBSpinningPreloader, private authService: AuthService, private router: Router) {
    this.setTimeout();
    this.userInactive.subscribe(() => {
      if (this.authService.loggedIn()) {
      // this.authService.redirectUrl = state.url;
      this.authService.redirectUrl = this.router.url;
      localStorage.setItem('url', this.authService.redirectUrl);
        alert('votre session a expiré');
        this.authService.logout();
      }
    }
    );
  }

  setTimeout() {
    this.userActivity = setTimeout(() => this.userInactive.next(undefined), 900000); // soit 15 minutes d'inactivité
  }

  @HostListener('window:mousemove') refreshUserStateMouse() {
    clearTimeout(this.userActivity);
    this.setTimeout();
  }

  @HostListener('window:keypress') refreshUserStateKey() {
    clearTimeout(this.userActivity);
    this.setTimeout();
  }

  ngOnInit() {
    const token = localStorage.getItem('token');
    const user: User = JSON.parse(localStorage.getItem('user'));
    const settings: Setting[] = JSON.parse(localStorage.getItem('settings'));
    const currentPeriod: Period = JSON.parse(localStorage.getItem('currentPeriod'));
    const currentChild: User = JSON.parse(localStorage.getItem('currentChild'));
    const currentClassId = localStorage.getItem('currentClassId');
    if (token) {
      this.authService.decodedToken = this.jwtHelper.decodeToken(token);
    }
    if (user) {
      this.authService.currentUser = user;
      this.authService.settings = settings;
      this.authService.currentPeriod = currentPeriod;
      this.authService.changeCurrentChild(currentChild);
      this.authService.changeUserPhoto(user.photoUrl);
      this.authService.changeCurrentClassId(Number(currentClassId));
    }
    this.setTimeout();
    this.mdbSpinningPreloader.stop();
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  accountValidated() {
    return this.authService.accountValidated();
  }
}
