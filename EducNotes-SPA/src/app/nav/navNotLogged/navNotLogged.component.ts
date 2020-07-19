import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-navNotLogged',
  templateUrl: './navNotLogged.component.html',
  styleUrls: ['./navNotLogged.component.scss']
})
export class NavNotLoggedComponent implements OnInit {

  constructor(private authService: AuthService) { }

  ngOnInit() {
  }

  logout() {
    this.authService.logout();
  }

}
