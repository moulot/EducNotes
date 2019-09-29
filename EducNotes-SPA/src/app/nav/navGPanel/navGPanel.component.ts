import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router } from '@angular/router';
import { NavigationService } from 'src/app/shared/services/navigation.service';

@Component({
  selector: 'app-navGPanel',
  templateUrl: './navGPanel.component.html',
  styleUrls: ['./navGPanel.component.scss']
})
export class NavGPanelComponent implements OnInit {
  studentTypeId = environment.studentTypeId;
  teacherTypeId = environment.teacherTypeId;
  parentTypeId = environment.parentTypeId;
  adminTypeId = environment.adminTypeId;
  user?: User;
  model: any = {};
  photoUrl: string;
  notifications: any[];

    // editer par mohamed
  constructor(public authService: AuthService, private alertify: AlertifyService,
    private router: Router, private navService: NavigationService) {
      this.notifications = [
        {
          icon: 'i-Speach-Bubble-6',
          title: 'New message',
          badge: '3',
          text: 'James: Hey! are you busy?',
          time: new Date(),
          status: 'primary',
          link: '/chat'
        },
        {
          icon: 'i-Receipt-3',
          title: 'New order received',
          badge: '$4036',
          text: '1 Headphone, 3 iPhone x',
          time: new Date('11/11/2018'),
          status: 'success',
          link: '/tables/full'
        },
        {
          icon: 'i-Empty-Box',
          title: 'Product out of stock',
          text: 'Headphone E67, R98, XL90, Q77',
          time: new Date('11/10/2018'),
          status: 'danger',
          link: '/tables/list'
        },
        {
          icon: 'i-Data-Power',
          title: 'Server up!',
          text: 'Server rebooted successfully',
          time: new Date('11/08/2018'),
          status: 'success',
          link: '/dashboard/v2'
        },
        {
          icon: 'i-Data-Block',
          title: 'Server down!',
          badge: 'Resolved',
          text: 'Region 1: Server crashed!',
          time: new Date('11/06/2018'),
          status: 'danger',
          link: '/dashboard/v3'
        }
      ];
    }

  ngOnInit() {
    this.user = this.authService.currentUser;
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
  }

  toggelSidebar() {
    const state = this.navService.sidebarState;
    if (state.childnavOpen && state.sidenavOpen) {
      return state.childnavOpen = false;
    }
    if (!state.childnavOpen && state.sidenavOpen) {
      return state.sidenavOpen = false;
    }
    if (!state.sidenavOpen && !state.childnavOpen) {
        state.sidenavOpen = true;
        // setTimeout(() => {
        //     state.childnavOpen = false;
        // }, 50);
    }
  }

  login() {
    this.authService.login(this.model).subscribe(next => {
      this.alertify.success('logged in successfully');
    }, error => {
      this.alertify.error(error);
    }, () => {
      this.router.navigate(['/home']);
    });
  }

  logout() {
    this.authService.logout();
  }

  loggedIn() {
    return this.authService.loggedIn();
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

  adminLoggedIn() {
    return this.authService.adminLoggedIn();
  }

  // logout() {
  //   localStorage.removeItem('token');
  //   localStorage.removeItem('user');
  //   this.authService.decodedToken = null;
  //   this.authService.currentUser = null;
  //   this.alertify.infoBar('logged out');
  //   this.router.navigate(['/home']);
  // }

}
