import { Component, Input, OnInit } from '@angular/core';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-teacher-account',
  templateUrl: './teacher-account.component.html',
  styleUrls: ['./teacher-account.component.scss']
})
export class TeacherAccountComponent implements OnInit {
  @Input() teacherid: any;
  teacher: any;

  constructor(private userService: UserService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.userService.getUser(this.teacherid).subscribe(user => {
      this.teacher = user;
      console.log(user);
    }, error => {
      this.alertify.error(error);
    });
  }

}
