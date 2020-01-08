import { Component, OnInit } from '@angular/core';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-user-account',
  templateUrl: './user-account.component.html',
  styleUrls: ['./user-account.component.scss']
})
export class UserAccountComponent implements OnInit {
  user: any;

  constructor(private alertify: AlertifyService, private route: ActivatedRoute) { }

  ngOnInit() {

    this.route.data.subscribe((data: any) => {
      this.user = data['user'];
      console.log(this.user);
    });
  }

}
