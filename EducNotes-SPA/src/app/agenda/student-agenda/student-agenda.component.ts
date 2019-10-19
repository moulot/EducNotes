import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { Class } from 'src/app/_models/class';
import { ClassService } from 'src/app/_services/class.service';
import { ActivatedRoute } from '@angular/router';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';
import { Course } from 'src/app/_models/course';

@Component({
  selector: 'app-student-agenda',
  templateUrl: './student-agenda.component.html',
  styleUrls: ['./student-agenda.component.scss']
})
export class StudentAgendaComponent implements OnInit {
  student: User;
  classRoom: Class;
  coursesWithAgenda: any;
  classAgendaByDate: any;
  userIdFromRoute: any;
  strFirstDay: string;
  strLastDay: string;
  agendaItems: any;
  evalsToCome: any;
  nbDayTasks = [];
  weekDays = [];
  firstDay: Date;
  daysToNow = 7;
  daysFromNow = 7;

  constructor(private authService: AuthService, private classService: ClassService,
    private alertify: AlertifyService, private route: ActivatedRoute,
    private userService: UserService) { }

  ngOnInit() {

    this.route.params.subscribe(params => {
      this.userIdFromRoute = params['id'];
    });

    const loggedUser = this.authService.currentUser;

    if (this.userIdFromRoute) {
      this.getUser(this.userIdFromRoute);
    } else {
      this.student = loggedUser;
      this.getUser(this.student.id);
    }
}

  getUser(id) {
    this.userService.getUser(id).subscribe((user: User) => {
      this.student = user;
      this.getClassCoursesWithAgenda(this.student.classId, this.daysToNow, this.daysFromNow);
      this.getClassAgendaByDate(this.student.classId, this.daysToNow, this.daysFromNow);
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

  getClassCoursesWithAgenda(classId, daysToNow, daysFromNow) {
    this.classService.getClassCoursesWithAgenda(classId, daysToNow, daysFromNow).subscribe((courses: any) => {
      this.coursesWithAgenda = courses;
    }, error => {
      this.alertify.error(error);
    });
  }

  getClassAgendaByDate(classId, daysToNow, daysFromNow) {
    this.classService.getClassAgendaByDate(classId, daysToNow, daysFromNow).subscribe((agenda: any) => {
      this.classAgendaByDate = agenda;
    }, error => {
      this.alertify.error(error);
    });
  }

  showCourseItems(courseId) {

  }

  getClass(classId) {
    this.classService.getClass(classId).subscribe(data => {
      this.classRoom = data;
    }, error => {
      this.alertify.error(error);
    });
  }

}
