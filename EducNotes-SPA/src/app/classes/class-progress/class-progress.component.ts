import { Component, OnInit } from '@angular/core';
import { Session } from 'src/app/_models/session';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-class-progress',
  templateUrl: './class-progress.component.html',
  styleUrls: ['./class-progress.component.scss']
})
export class ClassProgressComponent implements OnInit {
  schedule: any;
  session = <Session>{};

  constructor(private classService: ClassService, private alertify: AlertifyService,
    private route: ActivatedRoute, private router: Router) { }

  ngOnInit() {
    this.route.data.subscribe((data: any) => {
      const sessionData = data['session'];
      this.schedule = sessionData.sessionSchedule;
      this.session = sessionData.session;
    });
  }

}
