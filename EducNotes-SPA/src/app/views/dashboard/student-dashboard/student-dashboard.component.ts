import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth.service';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { EvaluationService } from 'src/app/_services/evaluation.service';
import { Class } from 'src/app/_models/class';

@Component({
  selector: 'app-student-dashboard',
  templateUrl: './student-dashboard.component.html',
  styleUrls: ['./student-dashboard.component.scss']
})
export class StudentDashboardComponent implements OnInit {
  student: User;
  classRoom: Class;
  strFirstDay: string;
  strLastDay: string;
  agendaItems: any;
  evalsToCome: any;
  nbDayTasks = [];
  weekDays = [];
  firstDay: Date;
  toNbDays = 7;

  constructor(private authService: AuthService, private classService: ClassService,
    private alertify: AlertifyService, private route: ActivatedRoute,
    private evalService: EvaluationService) { }

  ngOnInit() {
    this.student = this.authService.currentUser;
    this.getClass(this.student.classId);
    this.getAgenda(this.student.classId, this.toNbDays);
    this.getEvalsToCome(this.student.classId);
}

  getAgenda(classId, toNbDays) {

    this.classService.getTodayToNDaysAgenda(classId, toNbDays).subscribe((res: any) => {

      this.agendaItems = res.agendaItems;
      this.firstDay = res.firstDay;
      this.strFirstDay = res.strFirstDayy;
      this.strLastDay = res.strLastDay;
      this.weekDays = res.weekDays;
      this.nbDayTasks = res.nbDayTasks;

    }, error => {
      this.alertify.error(error);
    });
  }

  getEvalsToCome(classId) {
    this.evalService.getClassEvalsToCome(classId).subscribe(evals => {
      this.evalsToCome = evals;
    }, error => {
      this.alertify.error(error);
    });
  }

  getClass(classId) {
    this.classService.getClass(classId).subscribe(data => {
      this.classRoom = data;
    }, error => {
      this.alertify.error(error);
    });
  }

}
