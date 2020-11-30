import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth.service';
import { CourseUser } from 'src/app/_models/courseUser';
import { Period } from 'src/app/_models/period';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { EvaluationService } from 'src/app/_services/evaluation.service';

@Component({
  selector: 'app-teacher-dashboard',
  templateUrl: './teacher-dashboard.component.html',
  styleUrls: ['./teacher-dashboard.component.scss']
})
export class TeacherDashboardComponent implements OnInit {
  // @ViewChild('classSelect', {static: false}) classSelect: ElementRef;
  teacher: User;
  teacherCourses: any;
  teacherClasses: any;
  teacherClassesWithEvals: any;
  selectedClass: any;
  students: User[];
  currentPeriod: Period;
  nextCourses: any;
  nextCoursesByClass: any;
  evalsToCome: any;
  evalsToBeGraded: any;
  optionsCourse: any[] = [];
  sessionForm: FormGroup;
  schedule: any;
  dayIndex: number;
  courseOptions = [];

  constructor(private userService: UserService, private authService: AuthService,
    public alertify: AlertifyService, private router: Router, private fb: FormBuilder,
    private evalService: EvaluationService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.dayIndex = 0;
    this.createSessionForm();
    this.teacher = this.authService.currentUser;
    this.getTeacherClasses(this.teacher.id);
    this.getTeacherNextCourses(this.teacher.id);
    this.getEvals(this.teacher.id);
    this.getTeacherScheduleNDays(this.teacher.id);
  }

  createSessionForm() {
    this.sessionForm = this.fb.group({
      course: [null, Validators.required]
    });
  }

  getTeacherScheduleToday(teacherId) {
    this.userService.getTeacherScheduleToday(teacherId).subscribe((courses: CourseUser[]) => {
      this.teacherCourses = courses;
    }, error => {
      this.alertify.error(error);
    });
  }

  getTeacherClasses(teacherId) {
    this.userService.getTeacherClasses(teacherId).subscribe((courses: CourseUser[]) => {
      this.teacherClasses = courses;
      this.getNextCoursesByClass(this.teacher.id);
    }, error => {
      this.alertify.error(error);
    });
  }

  getEvals(teacherId) {
    this.evalService.getTeacherEvalsToCome(teacherId).subscribe((evals: any) => {
      this.evalsToCome = evals.evalsToCome;
      this.evalsToBeGraded = evals.evalsToBeGraded;
    }, error => {
      this.alertify.error(error);
    });
  }

  getTeacherScheduleNDays(teacherId) {
    this.userService.getTeacherScheduleNDays(teacherId).subscribe(data => {
      this.schedule = data;
    }, error => {
      this.alertify.error(error);
    });
  }

  getTeacherNextCourses(teacherId) {
    this.userService.getTeacherNextCourses(teacherId).subscribe((data: any) => {
      this.nextCourses = data;
      for (let i = 0; i < data.length; i++) {
        const elt = data[i];
        const option = {value: elt.scheduleId, label: elt.className + ' ' + elt.courseName + ' ' +
          elt.startHourMin + ' - ' + elt.endHourMin};
        this.optionsCourse = [...this.optionsCourse, option];
      }
    });
  }

  getNextCoursesByClass(teacherId) {
    this.userService.getNextCoursesByClass(teacherId).subscribe((data: any) => {
      this.nextCoursesByClass = data;
      for (let i = 0; i < this.teacherClasses.length; i++) {
        const aclass = this.teacherClasses[i];
        const index = this.nextCoursesByClass.findIndex(item => item.classId === aclass.classId);
        const coursesByClass = this.nextCoursesByClass[index];
        let classOptions = [];
        for (let j = 0; j < coursesByClass.courses.length; j++) {
          const course = coursesByClass.courses[j];
          const option = {value: course.scheduleId, label: aclass.className + '. ' + course.courseName + '. '
            + course.startHourMin + ' à ' + course.endHourMin};
          classOptions = [...classOptions, option];
        }
        this.courseOptions = [...this.courseOptions, classOptions];
      }
    });
  }

  goToClass() {
    const scheduleId = this.sessionForm.value.course;
    this.router.navigate(['/classSession', scheduleId]);
  }

  prevDay() {
    if (this.dayIndex > 0) {
      this.dayIndex--;
    }
  }

  nextDay() {
    if (this.dayIndex < Number(this.schedule.length) - 1) {
      this.dayIndex++;
    }
  }
}
