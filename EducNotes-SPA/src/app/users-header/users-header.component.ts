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
  @Input() student: User;
  @Output() getUser = new EventEmitter<any>();
  // childrenCtrl: FormControl = new FormControl();
  // unSelectedUsers: User[];
  selectedUser: User;
  children: User[];
  // childrenOptions = [];
  parent: User;

  constructor(private userService: UserService, private authService: AuthService) { }

  ngOnInit() {
    this.parent = this.authService.currentUser;
    this.getChildren(this.parent.id);
    // this.childrenCtrl.setValue(this.student.id);

    // this.childrenCtrl.valueChanges.subscribe(value => {
    //   this.selectedUser = this.children.find(c => c.id === value);
    //   this.authService.changeCurrentChild(this.selectedUser);
    //   this.getUser.emit(this.selectedUser.id);
    // });
  }

  getChildren(parentId) {
    this.userService.getChildren(parentId).subscribe((users: User[]) => {
      this.children = users;
      this.selectedUser = this.student;
      // this.unSelectedUsers = [];
      // for (let i = 0; i < users.length; i++) {
      //   const elt = users[i];
      //   if (elt.id !== this.student.id) {
      //     this.unSelectedUsers = [...this.unSelectedUsers, elt];
      //   }
      //   mdb-select options
      //   const option = {value: elt.id, label: elt.lastName + ' ' + elt.firstName};
      //   this.childrenOptions = [...this.childrenOptions, option];
      // }
    });
  }

  selectUser(childid) {
    this.selectedUser = this.children.find(c => c.id === childid);
    this.authService.changeCurrentChild(this.selectedUser);
    this.getUser.emit(this.selectedUser);
  }

}
