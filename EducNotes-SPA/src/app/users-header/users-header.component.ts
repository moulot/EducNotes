import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { User } from '../_models/user';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-users-header',
  templateUrl: './users-header.component.html',
  styleUrls: ['./users-header.component.scss']
})
export class UsersHeaderComponent implements OnInit {
  @Input() student: User;
  @Output() getUser = new EventEmitter<number>();
  unSelectedUsers: User[];
  children: User[];
  parent: User;

  constructor(private userService: UserService, private alertify: AlertifyService,
    private router: Router, private authService: AuthService) { }

  ngOnInit() {
    this.parent = this.authService.currentUser;
    this.getChildren(this.parent.id);
  }

  selectChild(child) {
    this.student = this.children.find(c => c.id === child.id);
    console.log(this.children);
    this.unSelectedUsers = [];
    for (let i = 0; i < this.children.length; i++) {
      const elt = this.children[i];
      if (elt.id !== this.student.id) {
        this.unSelectedUsers = [...this.unSelectedUsers, elt];
      }
    }
    console.log(this.unSelectedUsers);
    this.authService.changeCurrentChild(child);
    this.getUser.emit(child.id);
  }

  getChildren(parentId) {
    this.userService.getChildren(parentId).subscribe((users: User[]) => {
      this.children = users;
      this.unSelectedUsers = [];
      for (let i = 0; i < users.length; i++) {
        const elt = users[i];
        if (elt.id !== this.student.id) {
          this.unSelectedUsers = [...this.unSelectedUsers, elt];
        }
      }
    });
  }

}
