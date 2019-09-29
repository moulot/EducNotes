import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/_services/user.service';
import { FormGroup,  FormBuilder, Validators } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { User } from 'src/app/_models/user';
import { environment } from 'src/environments/environment';
import { ClassService } from 'src/app/_services/class.service';
import { trigger, transition, animate, style } from '@angular/animations';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css'],
  animations: [
    trigger('slideInOut', [
      transition(':enter', [
        style({transform: 'translateY(-100%)'}),
        animate('500ms ease-in', style({transform: 'translateY(0%)'}))
      ]),
      transition(':leave', [
        animate('500ms ease-in', style({transform: 'translateY(-100%)'}))
      ])
    ])
  ]
})
export class UserListComponent implements OnInit {
  userForm: FormGroup;
  lastName: ' ';
   userDetailed: any;
   firstName: ' ';
  seachButton = 'rechercher';
  showSearchDiv = false;
  selectedUser: User;
  editModel: any = {};
  userTypeId: number;
  users: User[] = [];
  userTypes: any;
  parentTypeId = environment.parentTypeId;
  studentTypeId = environment.studentTypeId;
  teacherTypeId = environment.teacherTypeId;
  adminTypeId = environment.adminTypeId;
  visible = false;
  submitText = 'enregistrer';
  sexe = this.userService.sexe;

   constructor(private userService: UserService, private alertify: AlertifyService,
      private fb: FormBuilder, private classServie: ClassService) { }

  ngOnInit() {
    this.initializeParams();
    this.createUserForm();
  }

  initializeParams() {
    this.editModel = {};
   this.editModel.lastName = '';
   this.editModel.dateOfBirth = null;
   this.editModel.gender = null;
   this.editModel.userTypeId = null;
   this.editModel.firstName = '';
   this.editModel.phoneNumber = '';
   this.editModel.email = '';
   this.editModel.secondPhoneNumber = '';
  }


  search() {
    this.userDetailed = null;
    this.seachButton = 'patienter...';
    this.showSearchDiv = false;

    const data =  {lastName: this.lastName, firstName: this.firstName};
      this.userService.searchUsers(data).subscribe((response: User[]) => {
        this.users = response;
        this.seachButton = 'rechercher';
        this.showSearchDiv = true;

    });
  }

  edit(element: User) {
    this.selectedUser = element;
    this.editModel = element;
    this.visible = true;
    this.userService.getUserTypes().subscribe((response) => {
      this.userTypes = response;
    });

    this.createUserForm();
    //
  }

  details(element: User) {
    this.userTypeId = element.userTypeId;
    if (element.userTypeId === this.parentTypeId) {
      this.searchParent(element.id);
    }
    if (element.userTypeId === this.studentTypeId) {
      this.searchStudent(element.id);
    }
    if (element.userTypeId === this.teacherTypeId) {
      this.searchTeacher(element.id);
    }
    if (element.userTypeId >= this.adminTypeId) {
      this.searchAdmin(element.id);
    }
    //
  }

  createUserForm() {
    this.userForm = this.fb.group({
      dateOfBirth: [this.editModel.dateOfBirth, Validators.nullValidator],
      userTypeId: [this.editModel.userTypeId, Validators.required],
      gender: [this.editModel.gender, Validators.required],
      lastName: [this.editModel.lastName, Validators.required],
      firstName: [this.editModel.firstName, Validators.nullValidator],
      phoneNumber: [this.editModel.phoneNumber, Validators.required],
      email: [this.editModel.email, [Validators.required, Validators.email]],
      secondPhoneNumber: [this.editModel.secondPhoneNumber, Validators.nullValidator]});
  }

  submit() {
    this.submitText = 'patienter...';

    const dataFromForm =  Object.assign({}, this.userForm.value);
      this.selectedUser.lastName = dataFromForm.lastName;
      this.selectedUser.firstName = dataFromForm.firstName;
      this.selectedUser.dateOfBirth = dataFromForm.dateOfBirth;
      this.selectedUser.phoneNumber = dataFromForm.phoneNumber;
      this.selectedUser.secondPhoneNumber = dataFromForm.secondPhoneNumber;
      this.selectedUser.email = dataFromForm.email;
      this.selectedUser.gender = dataFromForm.gender;
      this.selectedUser.userTypeId = dataFromForm.userTypeId;
      this.userService.updatePerson(this.selectedUser.id, this.selectedUser).subscribe(response => {
        const itemIndex = this.users.findIndex(item => item.id === this.selectedUser.id);
        Object.assign(this.users[ itemIndex ], this.selectedUser);
        this.visible = false;
        this.alertify.success('modification terminée...');
       this.submitText = 'enregistrer';
     }, error => {
    this.submitText = 'enregistrer';
    console.log(error);
    this.alertify.error(error);

      });
  }

  close(): void {
    this.visible = false;
  }

  searchTeacher(id: number) {
    this.showSearchDiv = false;
    this.classServie.getAllTeacherCoursesById(id).subscribe((response: any) => {
      this.userDetailed = response;
    }, error => {
      this.alertify.error(error);
    });
  }

  searchStudent(id: number) {
    this.showSearchDiv = false;
    this.classServie.getStudentAllDetailsById(id).subscribe((response: any) => {
      this.userDetailed = response;
    }, error => {
      this.alertify.error(error);
      console.log(error);
    });
    //
  }

  searchParent(id: number) {
    this.showSearchDiv = false;
    this.classServie.getParentAllDetailsById(id).subscribe((response: any) => {
      this.userDetailed = response;
    }, error => {
      this.alertify.error(error);
      console.log(error);
    });
    //
  }

  searchAdmin(id: number) {
    //
  }

}
