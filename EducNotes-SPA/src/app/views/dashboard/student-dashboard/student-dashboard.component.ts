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
  scheduleDay: any;
  evalsToCome: any;
  nbDayTasks = [];
  weekDays = [];
  firstDay: Date;
  toNbDays = 7;
  userCourses: any;
  studentAvg: any;
  isParentConnected = false;
  showChildrenList = false;
  currentChild: User;
  children: any[];
  url = '/home';
  parent: User;
  userIdFromRoute: any;
  searchText = '';
  searchText1 = '';
  previous: string;
  previous1: string;
  showUsersList = false;
  hourCols = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
  events: any;
  periodAvgs: any;

  constructor(private authService: AuthService, private classService: ClassService,
    private alertify: AlertifyService, private route: ActivatedRoute,
    private evalService: EvaluationService, private userService: UserService) { }

  ngOnInit() {
    if (this.authService.studentLoggedIn()) {
      this.showChildrenList = false;
      this.showUsersList = false;
      this.getUser(this.authService.currentUser.id);
    } else {
      this.isParentConnected = true;
      this.parent = this.authService.currentUser;
      this.showUsersList = true;
      this.authService.currentChild.subscribe(child => this.currentChild = child);
      if (this.currentChild.id === 0) {
        this.showChildrenList = true;
        this.getChildren(this.parent.id);
      } else {
        this.showChildrenList = false;
        this.getUser(this.currentChild.id);
      }
    }
  }

  getChildren(parentId: number) {
    this.userService.getChildren(parentId).subscribe((users: User[]) => {
      this.children = users;
    }, error => {
      this.alertify.error(error);
    });
  }

  getUser(id) {
    this.userService.getUser(id).subscribe((user: User) => {
      this.student = user;
      let parentId = 0;
      if (this.isParentConnected === true) {
        parentId = this.parent.id;
      }
      this.getUserInfos(this.student.id, parentId);
      // this.getAgenda(this.student.classId, this.toNbDays);
      // this.getEvalsToCome(this.student.classId);
      // this.getCoursesWithEvals(this.student.id, this.student.classId);
      // this.getScheduleDay(this.student.classId);
      // this.getEvents();
      this.showChildrenList = false;
    }, error => {
      this.alertify.error(error);
    });
  }

  getUserInfos(userId, parentId) {
    this.userService.getUserInfos(userId, parentId).subscribe((data: any) => {
      this.agendaItems = data.agendaItems;
      this.evalsToCome = data.evalsToCome;
      this.studentAvg = data.studentAvg;
      this.periodAvgs = data.periodAvgs;
      this.scheduleDay = data.coursesToday;
    }, error => {
      this.alertify.error(error);
      console.log(error);
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

  getScheduleDay(classId) {
    this.classService.getScheduleToday(classId).subscribe(data => {
      this.scheduleDay = data;
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

  parentLoggedIn() {
    return this.authService.parentLoggedIn();
  }

}
