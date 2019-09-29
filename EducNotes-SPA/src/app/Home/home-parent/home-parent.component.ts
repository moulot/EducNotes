import { Component, OnInit, Input, ViewChild, ElementRef } from '@angular/core';
import { User } from 'src/app/_models/user';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import {from } from 'rxjs';
import { AuthService } from 'src/app/_services/auth.service';
import { ClassService } from 'src/app/_services/class.service';



@Component({
  selector: 'app-home-parent',
  templateUrl: './home-parent.component.html',
  styleUrls: ['./home-parent.component.css']
})
export class HomeParentComponent implements OnInit {
  @ViewChild('childselect', {static: false}) childselect: ElementRef;
  @Input() user: User;
  courses: any;
  children: User[];
  currentChild: User;

  constructor(private userService: UserService, private alertify: AlertifyService,
    private authService: AuthService, private classService: ClassService) { }

   ngOnInit() {
    this.getChildren(this.user.id);
  }

  getClassScheduleToday(classid: number) {
    this.classService.getScheduleToday(classid).subscribe(courses => {
      this.courses = courses;
    }, error => {
      this.alertify.error(error);
    });
  }

  getChildren(parentId: number) {
    this.userService.getChildren(parentId).subscribe((users: User[]) => {
      this.children = users;
    }, error => {
      this.alertify.error(error);
    });
  }

  onChangedChild() {
    this.userService.getUser(this.childselect.nativeElement.value).subscribe(user => {
      this.currentChild = user;
      this.getClassScheduleToday(this.currentChild.classId);
    }, error => {
      this.alertify.error(error);
    });
  }

}
