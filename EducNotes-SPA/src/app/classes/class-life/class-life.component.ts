import { Component, OnInit } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { User } from 'src/app/_models/user';
import { Class } from 'src/app/_models/class';
import { ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

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
  nbAbsences = 0;
  nbSanctions = 0;
  nbRewards = 0;
  toggleFormAdd = false;
  absencesList: any = [];
  sanctionsList: any = [];
  title = true;

  allEvents = true;
  eventsWithNb: any = [];
  allSessions: any = [];
  sessions: any[] = [];
  filteredEvents;
  studentOptions: any[] = [];
  eventForm: FormGroup;

  constructor(private classService: ClassService, private alertify: AlertifyService,
    private fb: FormBuilder, private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      const classId = params['classId'];
      this.loadClass(classId);
    });
    this.createEventForm();
  }

  createEventForm() {
    this.eventForm = this.fb.group({
      student: [null, Validators.required],
      period: [null, Validators.required],
      eventType: [null, Validators.required]
    });
  }

  resetSessions() {
    this.filteredEvents = this.allSessions;
  }

  toggleForm() {
    this.toggleFormAdd = !this.toggleFormAdd;
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
    this.loadAbsences(classId);
    this.loadSanctions(classId);
   }, error => {
     this.alertify.error(error);
   });
  }

  loadEvents() {
    
  }

 getClass(classId) {
  this.classService.getClass(classId).subscribe((aclass: Class) => {
    this.selectedClass = aclass;
  });
 }

 showEventItems(courseId) {
  this.allEvents = false;
  this.filteredEvents = [];
  // this.sessionsByCourse = [];

  for (let i = 0; i < this.allSessions.length; i++) {
    const elt = this.allSessions[i];
    const result = elt.agendaItems.map(item => {
      if (item.courseId === courseId) {
        return item;
      }
    }).filter(item => !!item);
    console.log(result);
    if (result.length > 0) {
      const filteredElt = {
        'dueDate': elt.dueDate,
        'shortDueDate': elt.shortDueDate,
        'longDueDate': elt.longDueDate,
        'dueDateAbbrev': elt.DueDateAbbrev,
        'nbItems': result.length,
        'agendaItems': result
      };
      this.filteredEvents = [...this.filteredEvents, filteredElt];
      // this.sessionsByCourse = [...this.sessionsByDate, filteredElt];
    }
  }
}

  loadAbsences(classId) {
    this.classService.getClassAbsences(classId).subscribe((data: any) => {
      this.absencesList = data.absences;
      this.nbAbsences = data.nbAbsences;
    }, error => {
      this.alertify.error(error);
    });
  }

  loadSanctions(classId) {
    this.classService.getClassSanctions(classId).subscribe((data: any) => {
      this.sanctionsList = data.sanctions;
      this.nbSanctions = data.nbSanctions;
    }, error => {
      this.alertify.error(error);
    });
  }

}
