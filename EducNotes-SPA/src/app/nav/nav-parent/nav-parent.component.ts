import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav-parent',
  templateUrl: './nav-parent.component.html',
  styleUrls: ['./nav-parent.component.css']
})
export class NavParentComponent implements OnInit {
  @Output() closeNav = new EventEmitter();
  user: User;
  photoUrl: string;
  currentChild: User;

  constructor(public authService: AuthService, private alertify: AlertifyService,
    private router: Router) { }

  ngOnInit() {
    this.user = this.authService.currentUser;
    this.authService.currentChild.subscribe(child => this.currentChild = child);
    if (!this.currentChild.id) {
      this.currentChild.id = 0;
    }
    this.authService.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
  }

  closeNavOnClick() {
    this.closeNav.emit();
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

}
