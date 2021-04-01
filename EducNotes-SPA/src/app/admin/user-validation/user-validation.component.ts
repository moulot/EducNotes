import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Utils } from 'src/app/shared/utils';
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
  validEmailRegex = Utils.validEmailRegex;
  userTypes: any;
  usersForm: FormGroup;
  usersDivs: any[] = [];
  btnActive = 0;
  studentpage = 1;
  studentpageSize = 20;
  parentpage = 1;
  parentpageSize = 20;
  teacherpage = 1;
  teacherpageSize = 20;
  list = true;
  nbTeachers = 0;
  nbParents = 0;

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
          // case this.studentTypeId:
          //   for (let j = 0; j < type.users.length; j++) {
          //     this.usersDivs[type.id].editDiv[j] = false;
          //     const user = type.users[j];
          //     this.addChildItem(user.id, user.email, user.cell, user.lastName, user.firstName, user.className,
          //       user.classLevelName, user.motherLastName, user.motherFirstName, user.motherEmail, user.motherCell,
          //       user.fatherLastName, user.fatherFirstName, user.fatherEmail, user.fatherCell);
          //   }
          //   break;

          case this.parentTypeId:
            this.nbParents = type.users.length;
            for (let j = 0; j < type.users.length; j++) {
              this.usersDivs[type.id].editDiv[j] = false;
              const user = type.users[j];
              this.addParentItem(user.id, user.email, user.cell, user.lastName, user.firstName, user.children);
            }
            break;

          case this.teacherTypeId:
            this.nbTeachers = type.users.length;
            for (let j = 0; j < type.users.length; j++) {
              this.usersDivs[type.id].editDiv[j] = false;
              const user = type.users[j];
              this.addTeacherItem(user.id, user.email, user.cell, user.lastName, user.firstName, user.className, user.classLevelName);
            }
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
      students: this.fb.array([]),
      parents: this.fb.array([]),
      teachers: this.fb.array([])
    });
  }

  // addChildItem(id, email, cell, lname, fname, className, levelName, mLastN, mFirstN, mEmail, mCell,
  //   fLastN, fFirstN, fEmail, fCell): void {
  //   const children = this.usersForm.get('students') as FormArray;
  //   children.push(this.createChildItem(id, email, cell, lname, fname, className, levelName, mLastN, mFirstN, mEmail, mCell,
  //     fLastN, fFirstN, fEmail, fCell));
  // }

  // createChildItem(id, email, cell, lname, fname, className, levelName, mLastN, mFirstN, mEmail, mCell,
  //   fLastN, fFirstN, fEmail, fCell): FormGroup {
  //   return this.fb.group({
  //     id: id,
  //     lname: lname,
  //     fname: fname,
  //     className: className,
  //     levelName: levelName,
  //     email: [email, Validators.pattern(this.validEmailRegex)],
  //     cell: cell,
  //     mLastN: mLastN,
  //     mFirstN: mFirstN,
  //     mEmail: mEmail,
  //     mCell: mCell,
  //     fLastN: fLastN,
  //     fFirstN: fFirstN,
  //     fEmail: fEmail,
  //     fCell: fCell
  //   });
  // }

  addParentItem(id, email, cell, lname, fname, children): void {
    const parents = this.usersForm.get('parents') as FormArray;
    parents.push(this.createParentItem(id, email, cell, lname, fname, children));
  }

  createParentItem(id, email, cell, lname, fname, children): FormGroup {
    return this.fb.group({
      id: id,
      lname: lname,
      fname: fname,
      email: [email, Validators.pattern(this.validEmailRegex)],
      cell: cell,
      children: this.fb.array([children])
    });
  }

  addParentChildItem(id, email, cell, lname, fname, className, levelName): void {
    const children = this.usersForm.get('children') as FormArray;
    children.push(this.createParentChildItem(id, email, cell, lname, fname, className, levelName));
  }

  createParentChildItem(id, email, cell, lname, fname, className, levelName): FormGroup {
    return this.fb.group({
      id: id,
      lname: lname,
      fname: fname,
      className: className,
      levelName: levelName,
      email: [email, Validators.pattern(this.validEmailRegex)],
      cell: cell,
    });
  }

  addTeacherItem(id, email, cell, lname, fname, className, levelName): void {
    const children = this.usersForm.get('teachers') as FormArray;
    children.push(this.createTeacherItem(id, email, cell, lname, fname, className, levelName));
  }

  createTeacherItem(id, email, cell, lname, fname, className, levelName): FormGroup {
    return this.fb.group({
      id: id,
      lname: lname,
      fname: fname,
      className: className,
      levelName: levelName,
      email: [email, Validators.pattern(this.validEmailRegex)],
      cell: cell
    });
  }

  sendUserConfirmEmail(typeid, id, email) {
    const userData = [{ userTypeId: typeid, userIds: [id]}];
    this.userService.resendConfirmEmail(userData).subscribe(() => {
      this.alertify.success('les emails ont bien été envoyés.');
    }, error => {
      this.alertify.error(error);
    });
  }

  editUser(typeid, index) {
    let user: any;
    switch (typeid) {
      case this.studentTypeId:
        user = this.usersForm.value.students[index];
        break;
      case this.parentTypeId:
        user = this.usersForm.value.parents[index];
        break;
      case this.teacherTypeId:
        user = this.usersForm.value.teachers[index];
        break;
      default:
        break;
    }
    const userData = {id: user.id, email: user.email, cell: user.cell};
    this.userService.editUserToValidate(userData).subscribe(() => {
      this.alertify.success('email & cell de ' + user.lastName + ' ' + user.firstName + ' enregistrés');
      // setup updated user data
      const typeIndex = this.userTypes.findIndex(u => u.id === typeid);
      const studentIndex = this.userTypes[typeIndex].users.findIndex(u => u.id === user.id);
      this.userTypes[typeIndex].users[studentIndex].email = user.email;
      this.userTypes[typeIndex].users[studentIndex].cell = user.cell;
      this.toggleEditDiv(typeid, index);
    }, error => {
      this.alertify.error(error);
    });
  }

  toggleEditDiv(typeid, index) {
    const showDiv = this.usersDivs[typeid].editDiv[index];
    this.usersDivs[typeid].editDiv[index] = !showDiv;
  }

  cancelEdit(typeid, userid, index) {
    this.toggleEditDiv(typeid, index);
    let users: any;
    switch (typeid) {
      case this.studentTypeId:
        users = this.usersForm.get('students') as FormArray;
        break;

      case this.parentTypeId:
        users = this.usersForm.get('parents') as FormArray;
        break;

      case this.teacherTypeId:
        users = this.usersForm.get('teachers') as FormArray;
        break;

      default:
        break;
    }
    // retrieve initial values
    const typeIndex = this.userTypes.findIndex(u => u.id === typeid);
    const studentIndex = this.userTypes[typeIndex].users.findIndex(u => u.id === userid);
    const user = this.userTypes[typeIndex].users[studentIndex];
    users.at(index).get('email').setValue(user.email);
    users.at(index).get('cell').setValue(user.cell);
  }

  selectType(typeid) {
    this.btnActive = typeid;
  }

  setUsersDivs() {
    const tableSize = 10; // size should be bigger than the max index of table userTypes
    for (let i = 0; i < 10; i++) {
      const elt = {typeid: 0, editDiv: []};
      this.usersDivs = [...this.usersDivs, elt];
    }
  }

  showList() {
    this.list = true;
  }

  showGrid() {
    this.list = false;
  }

}
