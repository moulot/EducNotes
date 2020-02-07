import { Component, OnInit } from '@angular/core';
import { Session } from 'inspector';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-class-session',
  templateUrl: './class-session.component.html',
  styleUrls: ['./class-session.component.scss']
})
export class ClassSessionComponent implements OnInit {
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

  goToCallSheet() {
    this.router.navigate(['/callSheet', this.schedule.id]); // , {queryParams: {sch: scheduleId}, skipLocationChange: true});
  }

  goToProgress() {
    this.router.navigate(['/callSheet', this.schedule.id]);
  }
}
