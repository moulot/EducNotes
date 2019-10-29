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
  coursesWithTasks: any = [];
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
  nbDays = 7;
  allCourses = true;
  startDate = new Date();
  endDate: Date;
  strStartDate: string;
  strEndDate: string;
  agendaParams: any = {};
  model: any = [];

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

      const year = this.startDate.getFullYear();
      const month = this.startDate.getMonth() + 1;
      const date = this.startDate.getDate();

      this.agendaParams.currentDate = year + '-' + month + '-' + date + 'T00:00:00';
      this.agendaParams.nbDays = this.nbDays;
      this.agendaParams.isMovingPeriod = false;
      this.getClassAgendaByDate(this.student.classId, this.agendaParams);
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

  getClassAgendaByDate(classId, agendaParams) {
    this.classService.getClassAgendaByDate(classId, agendaParams).subscribe((agenda: any) => {
      this.classAgendaByDate = agenda.agendaList;
      this.filteredAgenda = agenda.agendaList;
      this.coursesWithTasks = agenda.coursesWithTasks;
      this.model = agenda.dones;
      this.startDate = agenda.startDate;
      this.strStartDate = agenda.strStartDate;
      this.endDate = agenda.endDate;
      this.strEndDate = agenda.strEndDate;
    }, error => {
      this.alertify.error(error);
    });
  }

  showCourseItems(courseId) {
    this.allCourses = false;
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
  }

  movePeriod(moveDays) {
    this.allCourses = true;
    this.agendaParams.isMovingPeriod = true;
    if (moveDays > 0) { // move forward
      this.agendaParams.currentDate = this.endDate;
      this.agendaParams.nbDays = moveDays;
    } else { // move backward of 'moveDays' days
      this.agendaParams.currentDate = this.startDate;
      this.agendaParams.nbDays = moveDays;
    }

    this.getClassAgendaByDate(this.student.classId, this.agendaParams);
  }

  showAllCourses() {
    if (this.allCourses === true) {
      this.filteredAgenda = this.classAgendaByDate;
    }
  }

  setCourseTask(index, agendaId) {
    this.classService.classAgendaSetDone(agendaId, this.model[index]).subscribe(() => {
      this.alertify.success('le devoir a bien été mis à jour');
    });
  }

}
