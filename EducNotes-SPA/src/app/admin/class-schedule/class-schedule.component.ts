import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Class } from 'src/app/_models/class';
import { FormGroup, FormControl } from '@angular/forms';
import { Schedule } from 'src/app/_models/schedule';
import { NgbModal, NgbModalRef, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { ModalScheduleComponent } from '../modal-schedule/modal-schedule.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-class-schedule',
  templateUrl: './class-schedule.component.html',
  styleUrls: ['./class-schedule.component.scss']
})
export class ClassScheduleComponent implements OnInit {
  @ViewChild('classSelect', {static: false}) classSelect: ElementRef;
  classId: number;
  // classes: Class[];
  scheduleItems: any;
  scheduleForm: FormGroup;
  ngbModalRef: NgbModalRef;
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
    private modalService: NgbModal, private router: Router) { }

  ngOnInit() {
    this.getClasses();
  }

  getClasses() {
    this.classService.getAllClasses().subscribe((data: Class[]) => {
      // this.classes = data;
      for (let i = 0; i < data.length; i++) {
        const elt = data[i];
        const element = {value: elt.id, label: 'classe ' + elt.name};
        this.optionsClass = [...this.optionsClass, element];
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
    this.classService.getClassSchedule(classId).subscribe((data: any) => {

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
    const options: NgbModalOptions = {
      ariaLabelledBy: 'modal-basic-title',
      size: 'lg',
      centered: true
    };
    const modalRef = this.modalService.open(ModalScheduleComponent, options);
    const instance = modalRef.componentInstance;
    instance.classId = this.classId;

    modalRef.result.then((result) => {
      if (result) {
        this.saveSchedules(result);
      }
    }, () => {

    });
  }

  saveSchedules(schedules: Schedule[]) {

    this.classService.saveSchedules(schedules).subscribe(() => {
      this.alertify.successBar('cours de l\'emploi du temps enregistrés');
      this.loadWeekSchedule(this.classId);
    }, error => {
      this.alertify.errorBar(error);
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
