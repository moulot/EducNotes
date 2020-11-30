import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Class } from 'src/app/_models/class';
import { User } from 'src/app/_models/user';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';
import { ClassService } from 'src/app/_services/class.service';
import { EvaluationService } from 'src/app/_services/evaluation.service';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-parent-dashboard',
  templateUrl: './parent-dashboard.component.html',
  styleUrls: ['./parent-dashboard.component.scss']
})
export class ParentDashboardComponent implements OnInit {
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

  constructor(private authService: AuthService, private classService: ClassService,
    private alertify: AlertifyService, private route: ActivatedRoute,
    private evalService: EvaluationService, private userService: UserService) { }

  ngOnInit() {
    this.dayIndex = 0;
    this.isParentConnected = true;
    this.parent = this.authService.currentUser;
    this.getChildren(this.parent.id);
    // console.log(this.children);
    // console.log(this.nbChildren);
    // if (this.nbChildren === 1) {
    //   this.showUsersList = false;
    //   this.showChildrenList = false;
    //   this.authService.currentChild.subscribe(child => this.currentChild = this.getChildren[0]);
    //   this.getUser(this.getChildren[0].id);
    // } else {
    //   this.showUsersList = true;
    //   this.showChildrenList = true;
    // }
    // this.authService.currentChild.subscribe(child => this.currentChild = child);
    // if (this.currentChild.id === 0) {
    //   this.showChildrenList = true;
    //   this.getChildren(this.parent.id);
    // } else {
    //   this.showChildrenList = false;
    //   this.getUser(this.currentChild.id);
    // }
  }

  getChildren(parentId: number) {
    this.userService.getChildren(parentId).subscribe((users: User[]) => {
      this.children = users;
      this.nbChildren = users.length;
      if (this.nbChildren === 1) {
        this.showUsersList = false;
        this.showChildrenList = false;
        this.authService.changeCurrentChild(this.children[0]);
        this.getUser(this.children[0].id);
      } else {
        this.authService.currentChild.subscribe(child => this.currentChild = child);
        if (this.currentChild.id === 0) {
          this.showUsersList = false;
          this.showChildrenList = true;
        } else {
          this.showUsersList = true;
          this.showChildrenList = false;
          this.student = this.currentChild;
          this.getUserInfos(this.student.id, this.parent.id);
        }
      }
    }, error => {
      this.alertify.error(error);
    });
  }

  getUser(id) {
    this.userService.getUser(id).subscribe((user: User) => {
      this.student = user;
      // let parentId = 0;
      // if (this.isParentConnected === true) {
      //   parentId = this.parent.id;
      // }
      this.getUserInfos(this.student.id, this.parent.id);
      this.showChildrenList = false;
      if (this.nbChildren > 1) {
        this.showUsersList = true;
      }
    }, error => {
      this.alertify.error(error);
    });
  }

  // getClassSchedule(classId) {
  //   this.classService.getClassScheduleByDay(classId).subscribe(data => {

  //   })
  // }

  getUserInfos(userId, parentId) {
    this.userService.getUserInfos(userId, parentId).subscribe((data: any) => {
      this.agendaItems = data.agendaItems;
      this.evalsToCome = data.evalsToCome;
      this.studentAvg = data.studentAvg;
      this.periodAvgs = data.periodAvgs;
      this.scheduleDays = data.scheduleDays;
      this.dayIndex = data.todayIndex;
      // console.log(this.scheduleDays);
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
      console.log('prev: ' + this.dayIndex);
    }
  }

  nextDay() {
    if (this.dayIndex < this.scheduleDays.length - 1) {
      this.dayIndex++;
      console.log('next: ' + this.dayIndex);
    }
  }

}
