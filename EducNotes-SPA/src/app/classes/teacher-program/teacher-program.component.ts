import { Component, OnInit } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth.service';
import { FormControl } from '@angular/forms';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-teacher-program',
  templateUrl: './teacher-program.component.html',
  styleUrls: ['./teacher-program.component.scss']
})
export class TeacherProgramComponent implements OnInit {
  teacherCourses: any;
  courseProgram: any;
  teacher: User;
  course: FormControl = new FormControl();
  courseOptions: any = [];
  showProgram = false;

  constructor(private classService: ClassService, private alertify: AlertifyService,
    private authService: AuthService, private userService: UserService) { }

  ngOnInit() {
    this.teacher = this.authService.currentUser;
    this.getTeacherCourses(this.teacher.id);
  }

  getTeacherCourses(teacherId) {
    this.userService.getTeacherCourses(teacherId).subscribe((courses: any) => {
      this.teacherCourses = courses;
      for (let i = 0; i < courses.length; i++) {
        const elt = courses[i];
        this.courseOptions = [...this.courseOptions, {value: elt.id, label: elt.name}];
      }
    }, error => {
      this.alertify.error(error);
    });
  }

  getCourseProgram() {
    const courseId = this.course.value;
    if (courseId > 0) {
      this.showProgram = true;
      const teacherId = this.teacher.id;
      this.classService.getTeacherCourseProgram(courseId, teacherId).subscribe((program: any) => {
        this.courseProgram = program;
      }, error => {
        this.alertify.error(error);
      });
    } else {
      this.showProgram = false;
    }
  }
}
