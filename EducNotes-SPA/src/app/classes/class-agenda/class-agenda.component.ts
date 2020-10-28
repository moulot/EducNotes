import { Component, OnInit } from '@angular/core';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClassService } from 'src/app/_services/class.service';
import { ActivatedRoute } from '@angular/router';
import { Class } from 'src/app/_models/class';

@Component({
  selector: 'app-class-agenda',
  templateUrl: './class-agenda.component.html',
  styleUrls: ['./class-agenda.component.scss']
})
export class ClassAgendaComponent implements OnInit {
  classRoom: Class;
  strMonday: string;
  strSaturday: string;
  strSunday: string;
  scheduleItems: any;
  classAgendaDays: any;
  allCourses = true;
  coursesWithTasks: any;
  filteredAgenda: any;
  nbDayTasks = [0, 0, 0, 0, 0, 0];
  weekDays = [0, 0, 0, 0, 0, 0];
  agendaParams: any = {};
  monday: Date;
  monCourses = [];
  tueCourses = [];
  wedCourses = [];
  thuCourses = [];
  friCourses = [];
  satCourses = [];
  sunCourses = [];

  constructor(private alertify: AlertifyService, private classService: ClassService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      const classId = params['classId'];
      this.getClass(classId);
      this.loadWeekAgenda(classId);
      this.loadWeekSchedule(classId);
    });
  }

  getClass(classId) {
    this.classService.getClass(classId).subscribe((aclass: Class) => {
      this.classRoom = aclass;
    });
  }

  loadWeekAgenda(classId) {
    this.classService.getClassCurrWeekAgenda(classId).subscribe((res: any) => {
      this.classAgendaDays = res.agendaItems;
      this.filteredAgenda = res.agendaItems;
      this.monday = res.firstDayWeek;
      this.strMonday = res.strMonday;
      this.strSaturday = res.strSaturday;
      this.weekDays = res.weekDays;
      this.nbDayTasks = res.nbDayTasks;
      this.coursesWithTasks = res.coursesWithTasks;
    }, error => {
      this.alertify.error(error);
    });
  }

  loadWeekSchedule(classId) {
    this.resetSchedule();
    this.classService.getClassSchedule(classId).subscribe((response: any) => {
      this.scheduleItems = response.scheduleItems;
      this.strMonday = response.strMonday;
      this.strSunday = response.strSunday;
      this.weekDays = response.weekDays;

      // add courses on the schedule
      for (let i = 1; i <= 7; i++) {
        const filtered = this.scheduleItems.filter(items => items.day === i);
        for (let j = 0; j < filtered.length; j++) {
          switch (i) {
            case 1:
            this.monCourses.push(filtered[j]);
            break;
            case 2:
            this.tueCourses.push(filtered[j]);
            break;
            case 3:
            this.wedCourses.push(filtered[j]);
            break;
            case 4:
            this.thuCourses.push(filtered[j]);
            break;
            case 5:
            this.friCourses.push(filtered[j]);
            break;
            case 6:
            this.satCourses.push(filtered[j]);
            break;
            case 7:
            this.sunCourses.push(filtered[j]);
            break;
            default:
              break;
          }
        }
      }

    }, error => {
      this.alertify.error(error);
    });
  }

  loadMovedWeek(move: number) {
    this.allCourses = true;
    this.agendaParams.dueDate = this.monday;
    this.agendaParams.moveWeek = move;
    this.classService.getClassMovedWeekAgenda(this.classRoom.id, this.agendaParams).subscribe((res: any) => {
      this.classAgendaDays = res.agendaItems;
      this.filteredAgenda = res.agendaItems;
      this.monday = res.firstDayWeek;
      this.strMonday = res.strMonday;
      this.strSaturday = res.strSaturday;
      this.weekDays = res.weekDays;
      this.nbDayTasks = res.nbDayTasks;
      this.coursesWithTasks = res.coursesWithTasks;
    }, error => {
      this.alertify.error(error);
    });
  }

  showAllCourses() {
    if (this.allCourses === true) {
      this.filteredAgenda = this.classAgendaDays;
    }
  }

  showCourseItems(courseId) {
    this.allCourses = false;
    this.filteredAgenda = [];

    for (let i = 0; i < this.classAgendaDays.length; i++) {
      const elt = this.classAgendaDays[i];
      const result = elt.courses.map(item => {
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
          'courses': result
        };
        this.filteredAgenda = [...this.filteredAgenda, filteredElt];
      }
    }
  }

  selectDay(index) {
  }

  resetSchedule() {
    this.monCourses = [];
    this.tueCourses = [];
    this.wedCourses = [];
    this.thuCourses = [];
    this.friCourses = [];
    this.satCourses = [];
    this.sunCourses = [];
    }

}
