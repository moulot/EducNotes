import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth.service';
import { CourseUser } from 'src/app/_models/courseUser';
import { ClassService } from 'src/app/_services/class.service';

@Component({
  selector: 'app-home-teacher',
  templateUrl: './home-teacher.component.html',
  styleUrls: ['./home-teacher.component.css']
})
export class HomeTeacherComponent implements OnInit {
  teacher: User;
  teacherCourses: any;
  teacherClasses: any;
  selectedClass: any;
  students: User[];

  constructor(private userService: UserService, private authService: AuthService,
    public alertify: AlertifyService, private classService: ClassService) { }

  ngOnInit() {
    this.teacher = this.authService.currentUser;
    this.getTeacherScheduleToday(this.teacher.id);
    this.getTeacherClasses(this.teacher.id);
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

  showModal(p: any) {
    this.selectedClass = p;
    this.classService.getClassStudents(p.id).subscribe(data => {
      this.students = data;
    });
  }

}
