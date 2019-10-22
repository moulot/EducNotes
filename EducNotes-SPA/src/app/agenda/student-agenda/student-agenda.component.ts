import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { Class } from 'src/app/_models/class';
import { ClassService } from 'src/app/_services/class.service';
import { ActivatedRoute } from '@angular/router';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-student-agenda',
  templateUrl: './student-agenda.component.html',
  styleUrls: ['./student-agenda.component.scss']
})
export class StudentAgendaComponent implements OnInit {
  student: User;
  classRoom: Class;
  coursesWithAgenda: any = [];
  filteredAgenda: any = [];
  initialData: any = [];
  classAgendaByDate: any = [];
  userIdFromRoute: any;
  strFirstDay: string;
  strLastDay: string;
  agendaItems: any;
  evalsToCome: any;
  nbDayTasks = [];
  weekDays = [];
  firstDay: Date;
  daysToNow = 0;
  daysFromNow = 7;
  allCourses = true;

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
      this.filteredAgenda = agenda;
    }, error => {
      this.alertify.error(error);
    });
  }

  showCourseItems(courseId) {
    this.allCourses = false;
    console.log(this.classAgendaByDate);
    // const courseAgenda = this.classAgendaByDate;
    // console.log(courseAgenda);
    this.filteredAgenda = [];

    for (let i = 0; i < this.classAgendaByDate.length; i++) {
      const elt = this.classAgendaByDate[i];
      const result = elt.agendaItems.map(item => {
        if (item.courseId === courseId) {
          return item;
        }
      }).filter(item => !!item);
      if (result.length > 0) {
        const filteredElt = {
          'dueDate': elt.dueDate,
          'shortDueDate': elt.shortDueDate,
          'longDueDate': elt.longDueDate,
          'nbItems': result.length,
          'agendaItems': result
        };
        this.filteredAgenda = [...this.filteredAgenda, filteredElt];
      }
    }
    console.log(this.filteredAgenda);
  }

  showAllCourses() {
    if (this.allCourses === true) {
      this.filteredAgenda = this.classAgendaByDate;
    }
  }

  getClass(classId) {
    this.classService.getClass(classId).subscribe(data => {
      this.classRoom = data;
    }, error => {
      this.alertify.error(error);
    });
  }

}
