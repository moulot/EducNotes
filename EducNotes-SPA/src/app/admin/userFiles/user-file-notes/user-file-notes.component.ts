import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { EvaluationService } from 'src/app/_services/evaluation.service';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-user-file-notes',
  templateUrl: './user-file-notes.component.html',
  styleUrls: ['./user-file-notes.component.scss']
})
export class UserFileNotesComponent implements OnInit {
  @Input() childid: any;
  showInfos = true;
  userGrades: any;

  constructor(private evalService: EvaluationService, private route: ActivatedRoute,
    private alertify: AlertifyService) { }

  ngOnInit() {
    this.getUserGrades(this.childid);
  }

  getUserGrades(userId) {
    this.evalService.getUserGrades(userId).subscribe(data => {
      this.userGrades = data;
    }, error => {
      this.alertify.error(error);
    });
  }

  toggleInfos() {
    this.showInfos = !this.showInfos;
  }
}
