import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav-teacher',
  templateUrl: './nav-teacher.component.html',
  styleUrls: ['./nav-teacher.component.css']
})
export class NavTeacherComponent implements OnInit {
  user: User;
  photoUrl: string;

  constructor(public authService: AuthService, private alertify: AlertifyService,
    private router: Router) { }

  ngOnInit() {
    this.user = this.authService.currentUser;
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    localStorage.removeItem('currentPeriod');
    this.authService.decodedToken = null;
    this.authService.currentUser = null;
    this.authService.currentPeriod = null;
    this.alertify.infoBar('logged out');
    this.router.navigate(['/home']);
  }
}
