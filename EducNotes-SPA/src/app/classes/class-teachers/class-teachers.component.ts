import { Component, OnInit } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-class-teachers',
  templateUrl: './class-teachers.component.html',
  styleUrls: ['./class-teachers.component.scss'],
  animations: [SharedAnimations]
})
export class ClassTeachersComponent implements OnInit {
  student: User;
  teachers: any;
  educLevelPrimary = environment.educLevelPrimary;
  isPrimaryClass = false;
  showChildrenList = false;
  children: User[];
  isParentConnected = false;
  url = '/classStaffP';
  parent: User;
  userIdFromRoute: any;

  constructor(private classService: ClassService, private alertify: AlertifyService, private route: ActivatedRoute,
    private authService: AuthService, private userService: UserService) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.userIdFromRoute = params['id'];
    });
    // is the parent connected?
    if (Number(this.userIdFromRoute) === 0) {
      this.showChildrenList = true;
      this.parent = this.authService.currentUser;
      this.getChildren(this.parent.id);
    } else {
      this.showChildrenList = false;
      this.getUser(this.userIdFromRoute);
    }
  }

  getChildren(parentId: number) {
    this.userService.getChildren(parentId).subscribe((users: User[]) => {
      this.children = users;
    }, error => {
      this.alertify.error(error);
    });
  }

  getUser(childId) {
    this.userService.getUser(childId).subscribe((user: User) => {
      this.student = user;
      this.getSiblings(this.student.id);
      const loggedUser = this.authService.currentUser;
      if (loggedUser.id !== this.student.id) {
        this.isParentConnected = true;
      }
      this.getClassTeachers(this.student.classId);
      this.showChildrenList = false;
    }, error => {
      this.alertify.error(error);
      this.showChildrenList = false;
    });
  }

  getSiblings(childId) {
    this.userService.getSiblings(childId).subscribe((users: User[]) => {
      this.children = users;
    }, error => {
      this.alertify.error(error);
    });
  }

  getClassTeachers(classId) {
    this.classService.getCourseWithTeacher(classId).subscribe(teachers => {
      this.teachers = teachers;
      // console.log(teachers);
      if (this.teachers[0].educLevelId === this.educLevelPrimary) {
        this.isPrimaryClass = true;
      } else {
        this.isPrimaryClass = false;
      }
    });
  }

}
