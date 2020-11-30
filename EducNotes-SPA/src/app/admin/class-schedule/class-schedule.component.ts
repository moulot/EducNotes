import { Component, OnInit, ViewChild, ElementRef, ViewEncapsulation } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Class } from 'src/app/_models/class';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Schedule } from 'src/app/_models/schedule';
import { ModalScheduleComponent } from '../modal-schedule/modal-schedule.component';
import { Router } from '@angular/router';
import { MDBModalService, MDBModalRef } from 'ng-uikit-pro-standard';

@Component({
  selector: 'app-class-schedule',
  templateUrl: './class-schedule.component.html',
  styleUrls: ['./class-schedule.component.scss']
})
export class ClassScheduleComponent implements OnInit {
  @ViewChild('classSelect', {static: false}) classSelect: ElementRef;
  classId: number;
  scheduleItems: any;
  scheduleForm: FormGroup;
  modalRef: MDBModalRef;
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
  hourCols = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11];
  weekdays = [0, 1, 2, 3, 4, 5];
  optionsClass: any[] = [];

  constructor(private classService: ClassService, public alertify: AlertifyService,
    private modalService1: MDBModalService, private router: Router) { }

  ngOnInit() {
    this.getClasses();
  }

  getClasses() {
    this.classService.getClassesByLevel().subscribe((data: any) => {
      // this.classes = data;
      for (let i = 0; i < data.length; i++) {
        const level = data[i];
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
    }, error => {
      this.alertify.error(error);
    });
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
      this.saveSchedules(data);
    });
  }

  saveSchedules(schedules: Schedule[]) {

    this.classService.saveSchedules(schedules).subscribe(() => {
      this.alertify.success('cours de l\'emploi du temps enregistrés');
      this.loadWeekSchedule(this.classId);
    }, error => {
      this.alertify.error(error);
      this.router.navigate(['/home']);
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
