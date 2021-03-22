import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Schedule } from 'src/app/_models/schedule';
import { ModalScheduleComponent } from '../modal-schedule/modal-schedule.component';
import { ActivatedRoute, Router } from '@angular/router';
import { MDBModalService, MDBModalRef } from 'ng-uikit-pro-standard';

@Component({
  selector: 'app-class-schedule',
  templateUrl: './class-schedule.component.html',
  styleUrls: ['./class-schedule.component.scss']
})
export class ClassScheduleComponent implements OnInit {
  @ViewChild('classSelect', {static: false}) classSelect: ElementRef;
  classId: number;
  classes: any;
  scheduleItems: any;
  scheduleForm: FormGroup;
  modalRef: MDBModalRef;
  strMonday: string;
  strSunday: string;
  strShortMonday: string;
  strShortSunday: string;
  schoolHours: any;
  tableHeight: any;
  scheduleHours: number;
  firstHour: number;
  firstMin: number;
  dayItems = [];
  loading: boolean;
  monCourses = [];
  tueCourses = [];
  wedCourses = [];
  thuCourses = [];
  friCourses = [];
  satCourses = [];
  sunCourses = [];
  weekDays = ['lundi', 'mardi', 'mercredi', 'jeudi', 'vendredi', 'samedi'];
  timeMask = [/\d/, /\d/, ':', /\d/, /\d/];
  agendaItems: Schedule[] = [];
  showTimeline = false;
  classControl = new FormControl();
  optionsClass: any[] = [];

  constructor(private classService: ClassService, public alertify: AlertifyService,
    private modalService1: MDBModalService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.classes = data['classes'];
      this.getClasses();
    });
  }

  getClasses() {
    for (let i = 0; i < this.classes.length; i++) {
      const level = this.classes[i];
      if (level.classes.length > 0) {
        const elt = {value: '', label: 'niveau ' + level.levelName, group: true};
        this.optionsClass = [...this.optionsClass, elt];
        for (let j = 0; j < level.classes.length; j++) {
          const aclass = level.classes[j];
          const elt1 = {value: aclass.id, label: 'classe ' + aclass.name};
          this.optionsClass = [...this.optionsClass, elt1];
        }
      }
    }
}

  classChanged() {
    if (this.classControl.value !== '') {
      this.classId = this.classControl.value;
      this.loadWeekSchedule(this.classId);
      this.showTimeline = true;
    } else {
      this.showTimeline = false;
    }
  }

  reloadSchedule() {
    this.loadWeekSchedule(this.classControl.value);
  }

  loadWeekSchedule(classId) {
    this.resetSchedule();
    this.classService.getClassTimeTable(classId).subscribe((data: any) => {
      this.scheduleItems = data.scheduleItems;
      this.strMonday = data.strMonday;
      this.strSunday = data.strSunday;
      this.strShortMonday = data.strShortMonday;
      this.strShortSunday = data.strShortSunday;
      // this.weekDays = data.weekDays;
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

  openModal() {
    const modalOptions = {
      backdrop: true,
      keyboard: false,
      focus: true,
      show: false,
      scroll: true,
      ignoreBackdropClick: false,
      class: 'modal-xl',
      containerClass: '',
      animated: true,
      data: { classId: this.classId }
    };
    this.modalRef = this.modalService1.show(ModalScheduleComponent, modalOptions);
    this.modalRef.content.saveSchedule.subscribe((data) => {
      // this.saveSchedules(data);
    });
  }

  // saveSchedules(schedules: Schedule[]) {
  //   this.classService.saveSchedules(schedules).subscribe(() => {
  //     this.alertify.success('cours de l\'emploi du temps enregistrés');
  //     this.loadWeekSchedule(this.classId);
  //   }, error => {
  //     this.alertify.error(error);
  //     this.router.navigate(['/home']);
  //   });
  // }

  resetSchedule() {
    this.monCourses = [];
    this.tueCourses = [];
    this.wedCourses = [];
    this.thuCourses = [];
    this.friCourses = [];
    this.satCourses = [];
    this.sunCourses = [];
  }

  counter(i: number) {
    return new Array(i);
  }
}
