import { Component, Input, OnInit } from '@angular/core';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-admin-account',
  templateUrl: './admin-account.component.html',
  styleUrls: ['./admin-account.component.scss']
})
export class AdminAccountComponent implements OnInit {
  @Input() adminid: any;
  admin: any;

  constructor(private userService: UserService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.userService.getUser(this.adminid).subscribe(user => {
      this.admin = user;
      console.log(user);
    }, error => {
      this.alertify.error(error);
    });
  }

}
