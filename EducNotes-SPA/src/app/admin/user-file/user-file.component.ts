import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-user-file',
  templateUrl: './user-file.component.html',
  styleUrls: ['./user-file.component.scss']
})
export class UserFileComponent implements OnInit {
  userFile: any;
  showInfos = false;

  constructor( private route: ActivatedRoute, private alertify: AlertifyService) { }

  ngOnInit() {
    this.route.data.subscribe((data: any) => {
      this.userFile = data['file'];
    });
  }

  toggleInfos() {
    this.showInfos = !this.showInfos;
  }

}
