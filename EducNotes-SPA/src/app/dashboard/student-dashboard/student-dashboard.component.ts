import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth.service';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { EvaluationService } from 'src/app/_services/evaluation.service';
import { Class } from 'src/app/_models/class';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-student-dashboard',
  templateUrl: './student-dashboard.component.html',
  styleUrls: ['./student-dashboard.component.scss']
})
export class StudentDashboardComponent implements OnInit {
  student: User;
  classRoom: Class;
  agendaItems: any;
  scheduleDays: any;
  evalsToCome: any;
  userCourses: any;
  studentAvg: any;
  parent: User;
  events: any;
  dayIndex: number;
  todayIndex: number;
  lastGrades: any;

  constructor(private authService: AuthService, private classService: ClassService,
    private alertify: AlertifyService, private route: ActivatedRoute,
    private evalService: EvaluationService, private userService: UserService) { }

  ngOnInit() {
    this.dayIndex = 0;
    this.student = this.authService.currentUser;
    this.getAgendaItems(this.student.classId);
    this.getEvalsToCome(this.student.classId);
    this.getScheduleDays(this.student.classId);
    this.getStudentLastGrades(this.student.id, this.student.classId);
    this.getClass(this.student.classId);
  }

  getClass(id) {
    this.classService.getClass(id).subscribe((aclass: Class) => {
      this.classRoom = aclass;
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

  getScheduleDays(classId) {
    this.classService.getScheduleNDays(classId).subscribe((data: any) => {
      this.scheduleDays = data.scheduleDays;
      this.dayIndex = data.todayIndex;
      this.todayIndex = data.todayIndex;
    });
  }

  getStudentLastGrades(studentId, classId) {
    this.evalService.getStudentLastGrades(studentId, classId).subscribe((data: any) => {
      this.lastGrades = data.lastGrades;
      this.studentAvg = data.studentAvg;
      this.userCourses = data.coursesAvg;
    }, error => {
      this.alertify.error(error);
    });
  }

  getAgendaItems(classId) {
    this.classService.getClassAgendaNbDays(classId).subscribe(items => {
      this.agendaItems = items;
    }, error => {
      this.alertify.error(error);
    });
  }

  getScheduleDay(classId) {
    this.classService.getScheduleToday(classId).subscribe(data => {
      this.scheduleDays = data;
    }, error => {
      this.alertify.error(error);
    });
  }

  getEvents() {
    const userId = this.student.id;
    this.userService.getEvents(userId).subscribe(data => {
      this.events = data;
    }, error => {
      this.alertify.error(error);
    });
  }

  prevDay() {
    if (this.dayIndex > 0) {
      this.dayIndex--;
    }
  }

  nextDay() {
    if (this.dayIndex < this.scheduleDays.length - 1) {
      this.dayIndex++;
    }
  }

  goToToday() {
    this.dayIndex = this.todayIndex;
  }

}
