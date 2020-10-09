import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-user-file-notes',
  templateUrl: './user-file-notes.component.html',
  styleUrls: ['./user-file-notes.component.scss']
})
export class UserFileNotesComponent implements OnInit {
  @Input() id: any;
  showInfos = true;

  constructor(private userService: UserService, private route: ActivatedRoute,
    private router: Router, private alertify: AlertifyService) { }

  ngOnInit() {
  }

  toggleInfos() {
    this.showInfos = !this.showInfos;
  }
}
