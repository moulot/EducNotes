import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-navNotLogged',
  templateUrl: './navNotLogged.component.html',
  styleUrls: ['./navNotLogged.component.scss']
})
export class NavNotLoggedComponent implements OnInit {

  constructor(private authService: AuthService) { }
  user: User;
  photoUrl: string;
  acc_name: string;

  ngOnInit() {
    if (this.authService.loggedIn()) {
      this.user = this.authService.currentUser;
      this.acc_name = this.user.firstName.toLowerCase() + ' ' + this.user.lastName.substring(0, 1).toLowerCase() + '.';
      this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
    }
  }

  logout() {
    this.authService.logout();
  }

}
