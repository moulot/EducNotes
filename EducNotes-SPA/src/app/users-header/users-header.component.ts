import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { User } from '../_models/user';
import { UserService } from '../_services/user.service';
import { AuthService } from '../_services/auth.service';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'app-users-header',
  templateUrl: './users-header.component.html',
  styleUrls: ['./users-header.component.scss']
})
export class UsersHeaderComponent implements OnInit {
  @Input() children: any;
  @Input() student: User;
  @Output() getUser = new EventEmitter<any>();
  // childrenCtrl: FormControl = new FormControl();
  // unSelectedUsers: User[];
  selectedUser: User;
  // children: User[];
  // childrenOptions = [];
  parent: User;

  constructor(private userService: UserService, private authService: AuthService) { }

  ngOnInit() {
    // this.parent = this.authService.currentUser;
    // this.getChildren(this.parent.id);
    this.selectedUser = this.student;
  }

  // getChildren(parentId) {
  //   this.userService.getChildren(parentId).subscribe((users: User[]) => {
  //     this.children = users;
  //     this.selectedUser = this.student;
  //   });
  // }

  selectUser(childid) {
    this.selectedUser = this.children.find(c => c.id === childid);
    this.authService.changeCurrentChild(this.selectedUser);
    this.getUser.emit(this.selectedUser);
  }

}
