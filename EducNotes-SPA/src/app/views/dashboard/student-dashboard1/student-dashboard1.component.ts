import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth.service';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { EvaluationService } from 'src/app/_services/evaluation.service';
import { Class } from 'src/app/_models/class';
import { UserService } from 'src/app/_services/user.service';


@Component({
  selector: 'app-student-dashboard1',
  templateUrl: './student-dashboard1.component.html',
  styleUrls: ['./student-dashboard1.component.scss']
})
export class StudentDashboard1Component implements OnInit {
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
  userCourses: any;
  studentAvg: any;
  userIdFromRoute: any;
  isParentConnected = false;

  constructor(private authService: AuthService, private classService: ClassService,
    private alertify: AlertifyService, private route: ActivatedRoute,
    private evalService: EvaluationService, private userService: UserService) { }

  ngOnInit() {

    this.route.params.subscribe(params => {
      this.userIdFromRoute = params['id'];
    });

    const loggedUser = this.authService.currentUser;

    if (this.userIdFromRoute) {
      this.getUser(this.userIdFromRoute);
      this.isParentConnected = true;
    } else {
      this.student = loggedUser;
      this.getUser(this.student.id);
    }
}

  getUser(id) {
    this.userService.getUser(id).subscribe((user: User) => {
      this.student = user;
      this.getAgenda(this.student.classId, this.toNbDays);
      this.getEvalsToCome(this.student.classId);
      this.getCoursesWithEvals(this.student.id, this.student.classId);
    }, error => {
      this.alertify.error(error);
    });
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

  getCoursesWithEvals(studentId, classId) {
    this.evalService.getUserCoursesWithEvals(classId, studentId).subscribe((data: any) => {
      this.userCourses = data.coursesWithEvals;
      this.studentAvg = data.studentAvg;
    }, error => {
      this.alertify.error(error);
    });
  }

}
