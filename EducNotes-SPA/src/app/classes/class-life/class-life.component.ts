import { Component, OnInit } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { User } from 'src/app/_models/user';
import { Class } from 'src/app/_models/class';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { EvaluationService } from 'src/app/_services/evaluation.service';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-class-life',
  templateUrl: './class-life.component.html',
  styleUrls: ['./class-life.component.scss']
})
export class ClassLifeComponent implements OnInit {
  teacher: User;
  teacherClasses: any;
  selectedClass: Class;
  students: User[];
  toggleFormAdd = false;
  absencesList: any = [];
  sanctionsList: any = [];
  title = true;

  showAllEvents = true;
  eventsWithNb: any = [];
  allEvents: any = [];
  sessions: any[] = [];
  filteredEvents;
  studentOptions: any[] = [];
  eventForm: FormGroup;
  periods: any;
  periodOptions: any[] = [];
  events: any;
  page = 1;
  pageSize = 10;
  nbByEvents = [];

  constructor(private classService: ClassService, private alertify: AlertifyService,
    private authService: AuthService, private evalService: EvaluationService,
    private fb: FormBuilder, private router: Router, private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      const classId = params['classId'];
      this.loadClass(classId);
      this.getPeriods();
      // this.getClassEvents();
    });
    this.createEventForm();
  }

  createEventForm() {
    this.eventForm = this.fb.group({
      student: [null, Validators.required],
      period: [this.authService.currentPeriod.id, Validators.required],
      eventType: [null, Validators.required]
    });
  }

  resetSessions() {
    this.filteredEvents = this.allEvents;
  }

  toggleForm() {
    this.toggleFormAdd = !this.toggleFormAdd;
  }

  goToAddEventPage() {
    this.router.navigate(['addClassEvent', this.selectedClass.id]);
  }

  loadClass(classId) {
    this.getClass(classId);
    this.classService.getClassStudents(classId).subscribe(data => {
    this.students = data;
    for (let i = 0; i < data.length; i++) {
      const elt = data[i];
      const element = {value: elt.id, label: elt.lastName + ' ' + elt.firstName};
      this.studentOptions = [...this.studentOptions, element];
    }
    this.loadEvents(classId);
   }, error => {
     this.alertify.error(error);
   });
  }

  loadEvents(classId) {
    this.classService.getEvents(classId).subscribe((data: any) => {
      this.allEvents = data.events;
      this.eventsWithNb = data.eventsWithNb;
      this.filteredEvents = data.events;
    }, error => {
      this.alertify.error(error);
    });
  }

  getClassEvents() {
    this.classService.getClassEvents().subscribe((data: any) => {
      this.eventsWithNb = [...this.eventsWithNb, {id: 0, name: 'absence'}];
      for (let i = 0; i < data.length; i++) {
        const elt = data[i];
        const classLife = {id: elt.id, name: elt.name};
        this.eventsWithNb = [...this.eventsWithNb, classLife];
      }
    });
  }

  getPeriods() {
    this.classService.getPeriods().subscribe(data => {
      this.periods = data;
      for (let i = 0; i < data.length; i++) {
        const elt = data[i];
        const element = {value: elt.id, label: elt.name};
        this.periodOptions = [...this.periodOptions, element];
      }
    }, error => {
      this.alertify.error(error);
    });
  }

  getClass(classId) {
  this.classService.getClass(classId).subscribe((aclass: Class) => {
    this.selectedClass = aclass;
  });
  }

  showEventItems(eventId) {
    if (eventId === 0) {
      this.filteredEvents = this.allEvents;
    } else {
      this.filteredEvents = [];
      this.filteredEvents = this.allEvents.filter(elt => elt.classEventTypeId === eventId);
    }
  }

}
