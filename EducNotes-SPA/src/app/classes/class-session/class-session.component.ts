import { Component, OnInit } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Session } from 'src/app/_models/session';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-class-session',
  templateUrl: './class-session.component.html',
  styleUrls: ['./class-session.component.scss']
})
export class ClassSessionComponent implements OnInit {
  schedule: any;
  session = <Session>{};

  constructor(private classService: ClassService, private alertify: AlertifyService,
    private route: ActivatedRoute, private router: Router, private authService: AuthService) { }

  ngOnInit() {
    this.route.data.subscribe((data: any) => {
      const sessionData = data['session'];
      this.schedule = sessionData.sessionSchedule;
      this.session = sessionData.session;
      this.authService.changeCurrentClassId(this.schedule.classId);
    });
  }

  goToCallSheet() {
    this.router.navigate(['/callSheet', this.schedule.id]);
  }

  goToProgress() {
    this.router.navigate(['/classProgram', this.schedule.courseId]);
  }
}
