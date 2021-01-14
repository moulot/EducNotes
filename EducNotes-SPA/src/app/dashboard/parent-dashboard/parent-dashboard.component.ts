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
  agendaItems: any;
  scheduleDays: any;
  evalsToCome: any;
  nbDayTasks = [];
  userCourses: any;
  lastGrades: any;
  studentAvg: any;
  showChildrenList: boolean;
  showUsersList = false;
  currentChild: User;
  children: any[];
  parent: User;
  events: any;
  nbChildren: number;
  dayIndex: number;
  todayIndex: number;
  loader = true;
  loadingOk = 0;

  constructor(private authService: AuthService, private classService: ClassService,
    private alertify: AlertifyService, private route: ActivatedRoute,
    private evalService: EvaluationService, private userService: UserService) { }

  ngOnInit() {
    this.dayIndex = 0;
    this.parent = this.authService.currentUser;
    this.getChildren(this.parent.id);
  }

  getChildren(parentId: number) {
    this.loadingOk = 0;
    this.userService.getChildren(parentId).subscribe((users: User[]) => {
      this.children = users;
      this.nbChildren = users.length;
      if (this.nbChildren === 1) {
        this.showUsersList = false;
        this.showChildrenList = false;
        const child = this.children[0];
        this.authService.changeCurrentChild(child);
        this.student = child;
        this.getUserInfos(child.id, child.classId);
      } else {
        this.authService.currentChild.subscribe(child => this.currentChild = child);
        if (this.currentChild.id === 0) {
          this.showUsersList = false;
          this.showChildrenList = true;
        } else {
          this.showUsersList = true;
          this.showChildrenList = false;
          this.student = this.currentChild;
          this.getUserInfos(this.student.id, this.student.classId);
        }
        this.loadingOk = 5;
      }
      }, error => {
      this.alertify.error(error);
      this.loadingOk = 5;
    });
  }

  selectUser(user) {
    this.student = user;
    this.getUserInfos(user.id, user.classId);
  }

  getUserInfos(userId, classId) {
    this.loadingOk = 0;
    this.getAgendaItems(classId);
    this.getEvalsToCome(classId);
    this.getScheduleDays(classId);
    this.getStudentLastGrades(userId, classId);
    this.getClass(classId);
    this.showUsersList = true;
    this.showChildrenList = false;
  }

  getAgendaItems(classId) {
    this.classService.getClassAgendaNbDays(classId).subscribe(items => {
      this.agendaItems = items;
    }, error => {
      this.alertify.error(error);
    }, () => {
      this.loadingOk += 1;
    });
  }

  getEvalsToCome(classId) {
    this.evalService.getClassEvalsToCome(classId).subscribe(evals => {
      this.evalsToCome = evals;
    }, error => {
      this.alertify.error(error);
    }, () => {
      this.loadingOk += 1;
    });
  }

  getScheduleDays(classId) {
    this.classService.getScheduleNDays(classId).subscribe((data: any) => {
      this.scheduleDays = data.scheduleDays;
      this.dayIndex = data.todayIndex;
      this.todayIndex = data.todayIndex;
    }, error => {
      this.alertify.error(error);
    }, () => {
      this.loadingOk += 1;
    });
  }

  getStudentLastGrades(studentId, classId) {
    this.evalService.getStudentLastGrades(studentId, classId).subscribe((data: any) => {
      this.lastGrades = data.lastGrades;
      this.studentAvg = data.studentAvg;
      this.userCourses = data.coursesAvg;
    }, error => {
      this.alertify.error(error);
    }, () => {
      this.loadingOk += 1;
    });
  }

  getClass(id) {
    this.classService.getClass(id).subscribe((aclass: Class) => {
      this.classRoom = aclass;
    }, error => {
      this.alertify.error(error);
    }, () => {
      this.loadingOk += 1;
    });
  }

  // getEvents() {
  //   const userId = this.parent.id;
  //   this.userService.getEvents(userId).subscribe(data => {
  //     this.events = data;
  //   }, error => {
  //     this.alertify.error(error);
  //   });
  // }

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
