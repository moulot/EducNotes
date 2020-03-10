import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { User } from 'src/app/_models/user';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { UserService } from 'src/app/_services/user.service';
import { Utils } from 'src/app/shared/utils';
import { ClassEvent } from 'src/app/_models/classEvent';
import { UserClassEvent } from 'src/app/_models/userClassEvent';
import { ActivatedRoute, Router } from '@angular/router';
import { Class } from 'src/app/_models/class';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-add-classLife',
  templateUrl: './add-classLife.component.html',
  styleUrls: ['./add-classLife.component.scss']
})
export class AddClassLifeComponent implements OnInit {
  students: User[];
  class: any;
  // @Output() toggleForm = new EventEmitter<boolean>();
  // @Output() loadSanctions = new EventEmitter<any>();
  classEventForm: FormGroup;
  classTeachers: User[];
  newEvent = <UserClassEvent>{};
  events: ClassEvent[];
  classEventOptions = [];
  studentOptions = [];
  teacherOptions = [];
  myDatePickerOptions = Utils.myDatePickerOptions;

  constructor(private classService: ClassService, private alertify: AlertifyService,
    private router: Router, private userService: UserService, private route: ActivatedRoute,
    private fb: FormBuilder, private authService: AuthService) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      const classId = params['id'];
      this.getClass(classId);
      this.loadStudents(classId);
    });

    this.createClassEventForm();
    // this.getClassTeachers(this.selectedClass.id);
    this.getClassLifes();
  }

  createClassEventForm() {
    this.classEventForm = this.fb.group({
      student: [null, Validators.required],
      event: [null, Validators.required],
      // date: [null, Validators.required],
      eventDate: [null, Validators.required],
      // doneBy: [null, Validators.required],
      reason: ['', Validators.required],
      comment: ['']
    });
  }

  getClass(classId) {
    this.classService.getClass(classId).subscribe((aclass: Class) => {
      this.class = aclass;
    });
  }

  loadStudents(classId) {
    this.classService.getClassStudents(classId).subscribe(data => {
      this.students = data;
      for (let i = 0; i < this.students.length; i++) {
        const student = this.students[i];
        const element = {value: student.id, label: student.lastName + ' ' + student.firstName};
        this.studentOptions = [...this.studentOptions, element];
      }
    }, error => {
      this.alertify.error(error);
    });
  }

  // getClassTeachers(classId) {
  //   this.classService.getClassTeachers(classId).subscribe(teachers => {
  //     this.classTeachers = teachers;
  //     for (let i = 0; i < teachers.length; i++) {
  //       const teacher = teachers[i];
  //       const element = {value: teacher.id, label: teacher.lastName + ' ' + teacher.firstName};
  //       this.teacherOptions = [...this.teacherOptions, element];
  //     }
  //   }, error => {
  //     this.alertify.error(error);
  //   });
  // }

  getClassLifes() {
    this.classService.getClassEvents().subscribe((events: ClassEvent[]) => {
      this.events = events;
      for (let i = 0; i < events.length; i++) {
        const event = events[i];
        const element = {value: event.id, label: event.name};
        this.classEventOptions = [...this.classEventOptions, element];
      }
    });
  }

  saveClassLife(more: boolean) {
    this.newEvent.userId = this.classEventForm.value.student;
    this.newEvent.classEventId = this.classEventForm.value.event;
    // this.newEvent.doneById = this.classEventForm.value.doneBy;
    this.newEvent.doneById = this.authService.currentUser.id;
    const dateData = this.classEventForm.value.eventDate.split('/');
    this.newEvent.startDate = new Date(dateData[2], dateData[1] - 1, dateData[0]);
    this.newEvent.reason = this.classEventForm.value.reason;
    this.newEvent.comment = this.classEventForm.value.comment;
    console.log(this.newEvent);
    this.userService.saveClassEvent(this.newEvent).subscribe(() => {
      this.alertify.success('ajout de l\'évènement OK.');
    }, error => {
      this.alertify.error(error);
    }, () => {
      this.classEventForm.reset();
      if (more === false) {
        this.router.navigate(['classLife', this.class.id]);
      }
    });
  }

  cancel() {
    this.classEventForm.reset();
  }
}
