import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth.service';
import { CourseUser } from 'src/app/_models/courseUser';
import { Period } from 'src/app/_models/period';
import { AdminService } from 'src/app/_services/admin.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl, FormGroup, FormBuilder, Validators } from '@angular/forms';
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
  evalsToCome: any;
  evalsToBeGraded: any;
  optionsCourse: any[] = [];
  sessionForm: FormGroup;

  constructor(private userService: UserService, private authService: AuthService,private fb: FormBuilder,
    private adminService: AdminService, public alertify: AlertifyService, private router: Router,
    private evalService: EvaluationService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.createSessionForm();
    this.teacher = this.authService.currentUser;
    this.getTeacherClasses(this.teacher.id);
    this.getTeacherNextCourses(this.teacher.id);
    this.getEvals(this.teacher.id);
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

  goToClass() {
    const scheduleId = this.sessionForm.value.course;
    this.router.navigate(['/classSession', scheduleId]); // , {queryParams: {sch: scheduleId}, skipLocationChange: true});
  }

  // showModal(p: any) {
  //   this.selectedClass = p;
  //   this.classService.getClassStudents(p.id).subscribe(data => {
  //     this.students = data;
  //   });
  // }
}
