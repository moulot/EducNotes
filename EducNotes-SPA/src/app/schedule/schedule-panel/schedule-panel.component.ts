import { Component, OnInit, Input } from '@angular/core';
import { User } from '../../_models/user';
import { ClassService } from '../../_services/class.service';
import { AlertifyService } from '../../_services/alertify.service';
import { Class } from 'src/app/_models/class';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-schedule-panel',
  templateUrl: './schedule-panel.component.html',
  styleUrls: ['./schedule-panel.component.css']
})
export class SchedulePanelComponent implements OnInit {
  @Input() teacher: User;
  // @Input() selectedClass: any;
  classRoom: Class;
  weekDays = ['', '', '', '', '', '', ''];
  agendaParams: any = {};
  monday: Date;
  strMonday: string;
  strSunday: string;
  strShortMonday: string;
  strShortSunday: string;
  scheduleItems: any;
  schoolHours: any;
  tableHeight: any;
  scheduleHours: number;
  firstHour: number;
  firstMin: number;
  // hourSize = 60;
  monCourses = [];
  tueCourses = [];
  wedCourses = [];
  thuCourses = [];
  friCourses = [];
  satCourses = [];
  sunCourses = [];
  dayItems = [];
  btnGroupModel = {
    left: false,
    middle: false,
    right: false
  };

  constructor(private classService: ClassService, public alertify: AlertifyService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(params => {
      this.classRoom = params['class'];
    });
    this.loadWeekSchedule(this.classRoom.id);
  }

  loadWeekSchedule(classId) {
    this.resetSchedule();
    this.classService.getClassTimeTable(classId).subscribe((data: any) => {
      this.scheduleItems = data.scheduleItems;
      this.strMonday = data.strMonday;
      this.strSunday = data.strSunday;
      this.strShortMonday = data.strShortMonday;
      this.strShortSunday = data.strShortSunday;
      this.weekDays = data.weekDays;
      this.schoolHours = data.schoolHours;
      this.tableHeight = data.scheduleHeight;

      this.scheduleHours = Number(this.schoolHours[2]) - Number(this.schoolHours[0]);
      this.firstHour = Number(this.schoolHours[0]);
      this.firstMin = Number(this.schoolHours[1]);

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

      this.dayItems = [this.monCourses, this.tueCourses, this.wedCourses, this.thuCourses, this.friCourses, this.satCourses];
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

  prevWeek() {

  }

  nextWeek() {

  }

  counter(i: number) {
    return new Array(i);
  }
}
