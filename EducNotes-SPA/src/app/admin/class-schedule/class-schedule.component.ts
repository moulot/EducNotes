import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { ClassLevel } from 'src/app/_models/classLevel';
import { Class } from 'src/app/_models/class';
import { Course } from 'src/app/_models/course';
import { FormBuilder, Validators, FormGroup, FormArray } from '@angular/forms';

@Component({
  selector: 'app-class-schedule',
  templateUrl: './class-schedule.component.html',
  styleUrls: ['./class-schedule.component.scss']
})
export class ClassScheduleComponent implements OnInit {
  @ViewChild('classSelect', {static: false}) classSelect: ElementRef;
  classId: number;
  courseId: number;
  day: number;
  classes: Class[];
  classCourses: Course[];
  scheduleItems: any;
  scheduleForm: FormGroup;
  dayItems = [];
  loading: boolean;
  weekDays = ['lundi', 'mardi', 'mercredi', 'jeudi', 'vendredi', 'samedi'];
  daySelect = [{'value': 1, 'name': 'lundi'}, {'value': 2, 'name': 'mardi'}, {'value': 3, 'name': 'mercredi'},
    {'value': 4, 'name': 'jeudi'}, {'value': 5, 'name': 'vendredi'}, {'value': 6, 'name': 'samedi'}];
  monCourses = [];
  tueCourses = [];
  wedCourses = [];
  thuCourses = [];
  friCourses = [];
  satCourses = [];
  sunCourses = [];
  timeMask = [/\d/, /\d/, ':', /\d/, /\d/];

  constructor(private classService: ClassService, public alertify: AlertifyService,
    private route: ActivatedRoute, private fb: FormBuilder) { }

  ngOnInit() {
    this.createScheduleForm();
    // this.setItemLines();
    this.getClasses();
  }

  createScheduleForm() {
    this.scheduleForm = this.fb.group({
      aclass: [null, Validators.required],
      course: [null, Validators.required],
      // itemLines: this.fb.array([])
      item1: this.fb.group({
        day1: [null, Validators.required],
        hourStart1: ['', [Validators.required, Validators.minLength(4)]],
        hourEnd1: ['', [Validators.required, Validators.minLength(4)]]
      }),
      item2: this.fb.group({
        day2: [null, Validators.required],
        hourStart2: ['', [Validators.required, Validators.minLength(4)]],
        hourEnd2: ['', [Validators.required, Validators.minLength(4)]]
      }),
      item3: this.fb.group({
        day3: [null, Validators.required],
        hourStart3: ['', [Validators.required, Validators.minLength(4)]],
        hourEnd3: ['', [Validators.required, Validators.minLength(4)]]
      }),
      item4: this.fb.group({
        day4: [null, Validators.required],
        hourStart4: ['', [Validators.required, Validators.minLength(4)]],
        hourEnd4: ['', [Validators.required, Validators.minLength(4)]]
      }),
      item5: this.fb.group({
        day5: [null, Validators.required],
        hourStart5: ['', [Validators.required, Validators.minLength(4)]],
        hourEnd5: ['', [Validators.required, Validators.minLength(4)]]
      }),
      item6: this.fb.group({
        day6: [null, Validators.required],
        hourStart6: ['', [Validators.required, Validators.minLength(4)]],
        hourEnd6: ['', [Validators.required, Validators.minLength(4)]]
      }),
    });
  }

  setItemLines() {

    console.log('in setItemLines');
    const items = this.scheduleForm.controls['itemLines'] as FormArray;

    for (let i = 0; i < this.weekDays.length; i++) {
      items.push(this.fb.group({
        day: [null, Validators.required],
        hourStart: ['', [Validators.required, Validators.minLength(4)]],
        hourEnd: ['', [Validators.required, Validators.minLength(4)]]
      }));
    }

    console.log(items.value);
  }

  saveScheduleItem() {
    console.log(this.scheduleForm.value);
  }

  getClasses() {
    this.classService.getAllClasses().subscribe((data: Class[]) => {
      this.classes = data;
    }, error => {
      this.alertify.error(error);
    });
  }

  getClassCourses(classId) {
    this.classService.getClassCourses(classId).subscribe((courses: Course[]) => {
      this.classCourses = courses;
    });
  }

  onClassChanged() {
    const classId = this.scheduleForm.value.aclass; // this.classId;
    this.getClassCourses(classId);
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
