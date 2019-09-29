import { Component, OnInit, Input } from '@angular/core';
import { User } from '../../_models/user';
import { ClassService } from '../../_services/class.service';
import { AlertifyService } from '../../_services/alertify.service';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-schedule-panel',
  templateUrl: './schedule-panel.component.html',
  styleUrls: ['./schedule-panel.component.css']
})
export class SchedulePanelComponent implements OnInit {
  @Input() teacher: User;
  @Input() selectedClass: any;
  weekDays = ['', '', '', '', '', '', ''];
  agendaParams: any = {};
  monday: Date;
  strMonday: string;
  strSunday: string;
  scheduleItems: any;
  monCourses = [];
  tueCourses = [];
  wedCourses = [];
  thuCourses = [];
  friCourses = [];
  satCourses = [];
  sunCourses = [];

  constructor(private classService: ClassService, public alertify: AlertifyService) { }

  ngOnInit() {
    this.loadWeekSchedule(this.selectedClass.id);
  }

  loadWeekSchedule(classId) {

    this.resetSchedule();
    this.classService.getClassSchedule(classId).subscribe((response: any) => {
      this.scheduleItems = response.scheduleItems;
      // this.monday = response.firstDayWeek;
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
