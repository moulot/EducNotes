import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { UserService } from 'src/app/_services/user.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-user-validation',
  templateUrl: './user-validation.component.html',
  styleUrls: ['./user-validation.component.scss']
})
export class UserValidationComponent implements OnInit {
  parentTypeId = environment.parentTypeId;
  teacherTypeId = environment.teacherTypeId;
  studentTypeId = environment.studentTypeId;
  userTypes: any;
  usersForm: FormGroup;
  usersDivs: any[] = [];

  constructor(private fb: FormBuilder, private route: ActivatedRoute, private alertify: AlertifyService,
    private userService: UserService) { }

  ngOnInit() {
    this.createUsersForm();
    this.route.data.subscribe((data: any) => {
      this.userTypes = data['users'];
      this.setUsersDivs();
      for (let i = 0; i < this.userTypes.length; i++) {
        const type = this.userTypes[i];
        this.usersDivs[type.id].typeid = type.id;
        switch (type.id) {
          case this.studentTypeId:
            for (let j = 0; j < type.users.length; j++) {
              this.usersDivs[type.id].editDiv[j] = false;
              const user = type.users[j];
              this.addChildItem(user.id, user.email, user.cell, user.lastName, user.firstName);
            }
            break;

          case this.parentTypeId:
            break;

          case this.teacherTypeId:
            break;

          default:
            break;
        }
      }
    }, error => {
      this.alertify.error(error);
    });
  }

  createUsersForm() {
    this.usersForm = this.fb.group({
      students: this.fb.array([])
    });
  }

  addChildItem(id, email, cell, lname, fname): void {
    const children = this.usersForm.get('students') as FormArray;
    children.push(this.createChildItem(id, email, cell, lname, fname));
  }

  createChildItem(id, email, cell, lname, fname): FormGroup {
    return this.fb.group({
      id: id,
      lname: lname,
      fname: fname,
      email: email,
      cell: cell
    });
  }

  editUser(typeid, index) {
    this.toggleEditDiv(typeid, index);
    const user = this.usersForm.value.students[index];
    const userData = {id: user.id, email: user.email, cell: user.cell};
    this.userService.editUserToValidate(userData).subscribe(() => {
      this.alertify.success('email & cell de ' + user.lastName + ' ' + user.firstName + ' enregistrés');
      this.toggleEditDiv(typeid, index);
    }, error => {
      this.alertify.error(error);
    });
  }

  toggleEditDiv(typeid, index) {
    const showDiv = this.usersDivs[typeid].editDiv[index];
    this.usersDivs[typeid].editDiv[index] = !showDiv;
  }

  setUsersDivs() {
    const tableSize = 10; // size should be bigger than the max index of table userTypes
    for (let i = 0; i < 10; i++) {
      const elt = {typeid: 0, editDiv: []};
      this.usersDivs = [...this.usersDivs, elt];
    }
  }

}
