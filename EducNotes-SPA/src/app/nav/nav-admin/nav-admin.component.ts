import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router } from '@angular/router';
import { environment } from 'src/environments/environment';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-nav-admin',
  templateUrl: './nav-admin.component.html',
  styleUrls: ['./nav-admin.component.css']
})
export class NavAdminComponent implements OnInit {
  @Output() closeNav = new EventEmitter();
  adminTypeId = environment.adminTypeId;
  menu: any;
  user: User;
  photoUrl: string;

  constructor(public authService: AuthService, private alertify: AlertifyService,
    private adminService: AdminService, private router: Router) { }

  ngOnInit() {
    this.getMenu();
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
  }

  getMenu() {
    this.adminService.getUserTypeMenu(this.adminTypeId).subscribe(data => {
      this.menu = data;
      // console.log(this.menu);
    });
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
    this.router.navigate(['/home']);
  }
}
