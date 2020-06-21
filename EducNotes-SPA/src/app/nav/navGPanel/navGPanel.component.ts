import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { environment } from 'src/environments/environment';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router } from '@angular/router';
import { NavbarComponent } from 'ng-uikit-pro-standard';

@Component({
  selector: 'app-navGPanel',
  templateUrl: './navGPanel.component.html',
  styleUrls: ['./navGPanel.component.scss']
})
export class NavGPanelComponent implements OnInit {
  @ViewChild('navbarid', {static: false}) navbaridRef: NavbarComponent; // Get the NavbarComponent
  @Input() user: User;
  studentTypeId = environment.studentTypeId;
  teacherTypeId = environment.teacherTypeId;
  parentTypeId = environment.parentTypeId;
  adminTypeId = environment.adminTypeId;
  model: any = {};
  photoUrl: string;
  notifications: any[];
  acc_name: string;

    // editer par mohamed
  constructor(public authService: AuthService, private alertify: AlertifyService,
    private router: Router) {
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
    if (this.loggedIn()) {
      // this.user = this.authService.currentUser;
      this.acc_name = this.user.firstName.substring(0, 1).toLowerCase() + '. ' + this.user.lastName.toLowerCase();
      this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
    }
  }

  closeNav() {
    if (this.navbaridRef.shown) {
      this.navbaridRef.toggle(); // Hide the collapse menu after click
    }
  }

  logout() {
    this.authService.logout();
  }

  // goToAccount() {
  //   this.router.navigate(['/userAccount']);
  // }

  loggedIn() {
    return this.authService.loggedIn();
  }

  parentLoggedIn() {
    if (this.loggedIn()) {
      return this.authService.parentLoggedIn();
    }
    return false;
  }

  studentLoggedIn() {
    if (this.loggedIn()) {
      return this.authService.studentLoggedIn();
    }
    return false;
  }

  teacherLoggedIn() {
    if (this.loggedIn()) {
      return this.authService.teacherLoggedIn();
    }
    return false;
  }

  adminLoggedIn() {
    if (this.loggedIn()) {
      return this.authService.adminLoggedIn();
    }
    return false;
  }

}
