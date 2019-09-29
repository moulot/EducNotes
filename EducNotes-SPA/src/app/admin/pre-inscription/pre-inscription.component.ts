import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { FormGroup,  FormBuilder, Validators } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ClassService } from 'src/app/_services/class.service';
import { AdminService } from 'src/app/_services/admin.service';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-pre-inscription',
  templateUrl: './pre-inscription.component.html',
  styleUrls: ['./pre-inscription.component.css']
})
export class PreInscriptionComponent implements OnInit {
 father: User ;
  mother: User ;
  editModel: any;
  children: any = [];
  classes: any[];
  visible: boolean;
  showDetails: boolean;
 editionMode = 'add';
   i = 1;
  selectedIndex = 0;
  position = 'left';
  size = 'large';
  fatherForm: FormGroup;
  motherForm: FormGroup;
  childForm: FormGroup;
  submitText = 'enregistrer';
 errorMessage = [];
  constructor(private fb: FormBuilder, private nzMessageService: NzMessageService, private authService: AuthService,
     private adminService: AdminService, private classService: ClassService, private alertify: AlertifyService) {
  }

  ngOnInit(): void {
    this.visible = false;
    this.showDetails = false;
   // this.children = [];
    this.initializeParams('');
    this.createParentsForms();
    this.createChildForm();
    this.classService.getAllClasses().subscribe((item) => {
      this.classes = item;
    }, error => {
      console.log(error);
    });
   // console.log(this.classes);
  }
  createParentsForms() {
    this.fatherForm = this.fb.group({
      lastName: ['', Validators.required],
      firstName: ['', Validators.nullValidator],
      phoneNumber: ['', Validators.required],
      email: ['', [Validators.nullValidator, Validators.email]],
      phoneNumberPrefix: [ '+225' ],
      secondPhoneNumber: ['', Validators.nullValidator]});

      this.motherForm = this.fb.group({
        lastName: ['', Validators.required],
        firstName: ['', Validators.nullValidator],
        phoneNumber: ['', Validators.required],
        email: ['', [Validators.nullValidator, Validators.email]],
        phoneNumberPrefix: [ '+225' ],
        secondPhoneNumber: ['', Validators.nullValidator]});
  }
  createChildForm() {

    this.childForm = this.fb.group({
      dateOfBirth: [this.editModel.dateOfBirth, Validators.required],
       lastName: [this.editModel.lastName, Validators.required],
      firstName: [this.editModel.firstName, Validators.nullValidator],
      gender: [this.editModel.gender, Validators.required],
      classId: [this.editModel.classId, Validators.required],
      phoneNumber: [this.editModel.phoneNumber, Validators.required],
      email: [this.editModel.email, [Validators.nullValidator, Validators.email]],
      secondPhoneNumber: [this.editModel.secondPhoneNumber, Validators.nullValidator]});
  }
      EmailsVerification(): void {

      }

  add() {
    this.initializeParams(this.fatherForm.value.lastName);
    this.createChildForm();
    this.open();

  }
  initializeParams(val: string) {
     this.editModel = {};
    this.editModel.lastName = val;
    this.editModel.dateOfBirth = null;
    this.editModel.firstName = '';
    this.editModel.gender = null;
    this.editModel.phoneNumber = '';
    this.editModel.email = '';
    this.editModel.classId = null;
    this.editModel.secondPhoneNumber = '';
  }
  submitChild(): void {
   let enfant: any;
    enfant = Object.assign({}, this.childForm.value);
    enfant.className = this.classes.find(item => item.id === enfant.classId).name;
     if (this.editionMode === 'add') {
      this.i = this.i + 1;
      enfant.id = this.i;
      this.children = [...this.children, enfant];
    } else {
      const itemIndex = this.children.findIndex(item => item.id === this.selectedIndex);
      Object.assign(this.children[ itemIndex ], enfant);

    }

    this.close();

  }
  open() {
    this.visible = !this.visible;
  }

  close(): void {
    this.visible = false;
  }
  cancel(): void {
    this.nzMessageService.info('suppression annulée');
  }
  back() {
    this.showDetails = false;
  }
  next() {
    this.showDetails = true;
    this.father = Object.assign({}, this.fatherForm.value);
    this.mother = Object.assign({}, this.motherForm.value);
    this.father.gender = 1;
    this.mother.gender = 0;
    this.emailsVerification();

  //  this.submitForm();
  }


  confirm(element: any): void {
    this.children.splice(this.children.findIndex(p => p.id === element.id), 1);

    this.nzMessageService.info('suppression éffectuée');
  }

  edit(element: any): void {
    this.selectedIndex = element.id;
   this.editModel = Object.assign({}, element);
   this.editionMode = 'edit';
   this.createChildForm();
    this.open();
  }
  save(id: number) {
    this.submitText = 'patienter...';
    this.alertify.confirm('Enregistrer ces information ?', () => {
          const data: any = {};
          data.father = this.father;
          data.mother = this.mother;
          data.children = this.children;
          this.adminService.savePreinscription(this.authService.decodedToken.nameid, data).subscribe(() => {
     this.submitText = 'enregistrer';
    this.createParentsForms();
    this.alertify.success('enregistrement terminé...');
    this.showDetails = false;
    this.father = null;
    this.mother = null;
    this.children = [];

          }, error => {
    this.submitText = 'enregistrer';
    this.alertify.error(error);

          });
    });
  }
  emailsVerification() {
    this.errorMessage = [];
    if (this.father.email) {
      this.adminService.emailExist(this.father.email).subscribe((response: boolean) => {
          if (response === true) {
          this.errorMessage.push('l\'email du père est déja utilisé');
          }
      });

    }
    if (this.mother.email) {
      this.adminService.emailExist(this.mother.email).subscribe((response: boolean) => {
          if (response === true) {
          this.errorMessage.push('l\'email de la mere est déja utilisé');
          }
      });

    }

    for (let i = 0; i < this.children.length; i++) {
      if (this.children[i].email) {
        this.adminService.emailExist(this.children[i].email).subscribe((response: boolean) => {
            if (response === true) {
              const msg = 'l\'email de l\' enfant' + this.children[i].lastName + ' ' + this.children[i].firstName + ' est déja utilisé';
            this.errorMessage.push(msg);
            }
        });

      }

    }
  }


}
