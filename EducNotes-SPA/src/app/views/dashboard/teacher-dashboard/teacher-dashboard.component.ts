import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth.service';
import { CourseUser } from 'src/app/_models/courseUser';
import { ClassService } from 'src/app/_services/class.service';
import { Period } from 'src/app/_models/period';
import { AdminService } from 'src/app/_services/admin.service';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl } from '@angular/forms';

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
  coursesControl: FormControl = new FormControl();
  nextCourses: any;

  constructor(private userService: UserService, private authService: AuthService,
    private adminService: AdminService, public alertify: AlertifyService, private router: Router,
    private classService: ClassService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.teacher = this.authService.currentUser;
    this.getTeacherClasses(this.teacher.id);
    this.getTeacherNextCourses(this.teacher.id);
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

  getTeacherNextCourses(teacherId) {
    this.userService.getTeacherNextCourses(teacherId).subscribe(data => {
      this.nextCourses = data;
    });
  }

  goToClass() {
    // const dataToArray = this.coursesControl.value.split('#');
    // const classId = dataToArray[0];
    // const scheduleId = dataToArray[1];
    const scheduleId = this.coursesControl.value;
    this.router.navigate(['/callSheet', scheduleId]); // , {queryParams: {sch: scheduleId}, skipLocationChange: true});
  }

  // showModal(p: any) {
  //   this.selectedClass = p;
  //   this.classService.getClassStudents(p.id).subscribe(data => {
  //     this.students = data;
  //   });
  // }
}
