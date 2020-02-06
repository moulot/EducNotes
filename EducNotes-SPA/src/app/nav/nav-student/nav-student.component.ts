import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav-student',
  templateUrl: './nav-student.component.html',
  styleUrls: ['./nav-student.component.css']
})
export class NavStudentComponent implements OnInit {
  @Output() closeNav = new EventEmitter();
  user: User;
  photoUrl: string;

  constructor(public authService: AuthService, private alertify: AlertifyService,
    private router: Router) { }

  ngOnInit() {
    this.user = this.authService.currentUser;
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
  }

  closeNavOnClick() {
    this.closeNav.emit();
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    this.authService.decodedToken = null;
    this.authService.currentUser = null;
    this.alertify.infoBar('logged out');
    this.router.navigate(['/login']);
  }
}
