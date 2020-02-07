import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../_services/auth.service';

@Component({
  selector: 'app-home-panel',
  templateUrl: './home-panel.component.html',
  styleUrls: ['./home-panel.component.css']
})
export class HomePanelComponent implements OnInit {

  constructor(public authService: AuthService) { }

  ngOnInit() {
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  adminLoggedIn() {
    return this.authService.adminLoggedIn();
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
}
