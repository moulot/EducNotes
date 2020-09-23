import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-user-file',
  templateUrl: './user-file.component.html',
  styleUrls: ['./user-file.component.scss']
})
export class UserFileComponent implements OnInit {
  userFile: any;
  showInfos = true;
  toggle = false;

  constructor(private userService: UserService, private route: ActivatedRoute,
    private router: Router, private alertify: AlertifyService) { }

  ngOnInit() {
    this.route.data.subscribe((data: any) => {
      this.userFile = data['file'];
    }, error => {
      this.alertify.error(error);
    });
  }

  toggleInfos() {
    this.showInfos = !this.showInfos;
  }

  changeUserFile(id) {
    const url = this.router.url;
    if (url.includes('userFile')) {
      this.router.navigate(['fileUser', id]);
    } else {
      this.router.navigate(['userFile', id]);
    }
  }

}
