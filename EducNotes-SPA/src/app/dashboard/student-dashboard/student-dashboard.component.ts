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
  strFirstDay: string;
  strLastDay: string;
  agendaItems: any;
  scheduleDays: any;
  evalsToCome: any;
  nbDayTasks = [];
  weekDays = [];
  firstDay: Date;
  toNbDays = 7;
  userCourses: any;
  studentAvg: any;
  isParentConnected = false;
  showChildrenList = false;
  showUsersList = false;
  currentChild: User;
  children: any[];
  url = '/home';
  parent: User;
  userIdFromRoute: any;
  searchText = '';
  searchText1 = '';
  previous: string;
  previous1: string;
  hourCols = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
  events: any;
  periodAvgs: any;
  nbChildren: number;
  dayIndex: number;
  todayIndex: number;
  lastGrades: any;

  constructor(private authService: AuthService, private classService: ClassService,
    private alertify: AlertifyService, private route: ActivatedRoute,
    private evalService: EvaluationService, private userService: UserService) { }

  ngOnInit() {
    this.dayIndex = 0;
    this.isParentConnected = true;
    this.student = this.authService.currentUser;
    this.getAgendaItems(this.student.classId);
    this.getEvalsToCome(this.student.classId);
    this.getScheduleDays(this.student.classId);
    // this.getCoursesWithEvals(this.student.id, this.student.classId);
    this.getStudentLastGrades(this.student.id, this.student.classId);
    // this.getUser(this.student.id);
    this.getClass(this.student.classId);
  }

  getUser(id) {
    this.userService.getUser(id).subscribe((user: User) => {
      this.student = user;
      // this.getUserInfos(this.student.id, 0);
      // this.getAgendaItems(this.student.classId);
      // this.getEvalsToCome(this.student.classId);
      // this.getScheduleDays(this.student.classId);
      // this.getStudentLastGrades(this.student.id, this.student.classId);
      // this.getCoursesWithEvals(this.student.id, this.student.classId);
    }, error => {
      this.alertify.error(error);
    });
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

  // getUserInfos(userId, parentId) {
  //   this.userService.getUserInfos(userId, parentId).subscribe((data: any) => {
  //     this.agendaItems = data.agendaItems;
  //     this.evalsToCome = data.evalsToCome;
  //     this.studentAvg = data.studentAvg;
  //     this.periodAvgs = data.periodAvgs;
  //     this.scheduleDays = data.scheduleDays;
  //     this.dayIndex = data.todayIndex;
  //     this.todayIndex = data.todayIndex;
  //   }, error => {
  //     this.alertify.error(error);
  //   });
  // }

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

  // getAgenda(classId, toNbDays) {
  //   this.classService.getTodayToNDaysAgenda(classId, toNbDays).subscribe((res: any) => {
  //     this.agendaItems = res.agendaItems;
  //     this.firstDay = res.firstDay;
  //     this.strFirstDay = res.strFirstDayy;
  //     this.strLastDay = res.strLastDay;
  //     this.weekDays = res.weekDays;
  //     this.nbDayTasks = res.nbDayTasks;
  //   }, error => {
  //     this.alertify.error(error);
  //   });
  // }

  getCoursesWithEvals(studentId, classId) {
    this.evalService.getUserCoursesWithEvals(classId, studentId).subscribe((data: any) => {
      this.userCourses = data.coursesWithEvals;
      // this.studentAvg = data.studentAvg;
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
    let userId = 0;
    if (this.isParentConnected === true) {
      userId = this.parent.id;
    } else {
      userId = this.student.id;
    }
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
