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
  // @Input() userIsParent: boolean;
  // @Input() childSelected: boolean;
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
  isParentConnected = false;
  showChildrenList = false;
  currentChild: User;
  children: User[];
  url = '/home';
  parent: User;
  userIdFromRoute: any;
  searchText = '';
  searchText1 = '';
  previous: string;
  previous1: string;
  showUsersList = false;

  constructor(private authService: AuthService, private classService: ClassService,
    private alertify: AlertifyService, private route: ActivatedRoute,
    private evalService: EvaluationService, private userService: UserService) { }

  ngOnInit() {

    // this.route.params.subscribe(params => {
    //   this.userIdFromRoute = params['id'];
    // });
    // console.log(this.userIdFromRoute);
    // is the child connected? // id coming from route params
    if (this.authService.studentLoggedIn()) {
      this.showChildrenList = false;
      this.showUsersList = false;
      this.getUser(this.authService.currentUser.id);
    } else {
      this.isParentConnected = true;
      this.showUsersList = true;
      this.authService.currentChild.subscribe(child => this.currentChild = child);
      if (this.currentChild.id === 0) {
        this.showChildrenList = true;
        this.parent = this.authService.currentUser;
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

  parentLoggedIn() {
    return this.authService.parentLoggedIn();
  }

}
