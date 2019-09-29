import { Component, OnInit } from '@angular/core';
import { FormGroup,  FormBuilder, Validators } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-administration-users',
  templateUrl: './administration-users.component.html',
  styleUrls: ['./administration-users.component.css']
})
export class AdministrationUsersComponent implements OnInit {
  userForm: FormGroup;
  userTypes: any;
  typeId: number;
  userId: number;
  user: any;
  users: any[] = [];
  editModel: any = {};
  editionMode = '';
 submitText = 'enregistrer';
 noResult = '';
 drawerTitle: string;
 visible = false;
 sexe: any;


  constructor(private adminService: AdminService,  private fb: FormBuilder, private alertify: AlertifyService) { }

  ngOnInit() {
    this.sexe = this.adminService.sexe;
    this.adminService.getAdministrationUserTypes().subscribe((response: any) => {
      this.userTypes = response;
    });
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

  add() {
    this.initializeParams();
    this.createUserForm();
    this.editionMode = 'add';
    this.drawerTitle = 'NOUVEL UTILISATEUR';
    this.visible = true;
  }
  search() {
    this.noResult = '';
    this.users = [];
   this.adminService.getUserByTypeId(this.typeId).subscribe((response: User[]) => {
     if (response.length !== 0) {
      this.users = response;
    } else {
      this.noResult = 'aucune personne trouvée';
    }
   }, error => {
     this.alertify.error(error);
   });
  }

  close(): void {
    this.visible = false;
  }
  submit() {
    this.submitText = 'patienter...';

    if (this.editionMode === 'add') {

      this.user = Object.assign({}, this.userForm.value);
      this.adminService.emailExist(this.user.email).subscribe((res: boolean) => {
        if (res === true) {
          this.alertify.infoBar('l\'email ' + this.user.email + 'est déja utlilisé ');
          this.submitText = 'enregistrer';

        } else {
          this.adminService.addUser(this.user).subscribe((response: any) => {
            this.visible = false;
          response.type = 'new';
          //  this.teachersCourses = [...this.teachersCourses, response];
         // this.teachersCourses.unshift(response);
            this.alertify.success('enregistrement terminé...');
           this.submitText = 'enregistrer';
         }, error => {
        this.submitText = 'enregistrer';
        console.log(error);
        this.alertify.error(error);
          });

        }
      });

    } else {
      const dataFromForm =  Object.assign({}, this.userForm.value);
      this.user.lastName = dataFromForm.lastName;
      this.user.firstName = dataFromForm.firstName;
      this.user.dateOfBirth = dataFromForm.dateOfBirth;
      this.user.phoneNumber = dataFromForm.phoneNumber;
      this.user.secondPhoneNumber = dataFromForm.secondPhoneNumber;
      this.user.email = dataFromForm.email;
      this.user.gender = dataFromForm.gender;
      this.user.userTypeId = dataFromForm.userTypeId;
      this.adminService.updatePerson(this.userId, this.user).subscribe(response => {
        const itemIndex = this.users.findIndex(item => item.id === this.userId);
        Object.assign(this.users[ itemIndex ], response);
        this.visible = false;
        this.alertify.success('modification terminée...');
       this.submitText = 'enregistrer';
     }, error => {
    this.submitText = 'enregistrer';
    console.log(error);
    this.alertify.error(error);

      });
    }


  }

  updatingUser(element: User) {
    this.user = element;
    this.userId = element.id;
    this.editModel = element;
    this.createUserForm();
    this.editionMode = 'edit';
    this.drawerTitle = 'MISE A JOUR UTILISATEUR';
    this.visible = true;
  }

  deleteUser(element: number) {

  }


}
