import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { FormGroup, Validators, FormBuilder, FormControl } from '@angular/forms';
import { AdminService } from 'src/app/_services/admin.service';
import { Email } from 'src/app/_models/email';
import { ClassLevel } from 'src/app/_models/classLevel';
import { UserService } from 'src/app/_services/user.service';
import { DataForBroadcast } from 'src/app/_models/dataForBroadcast';

@Component({
  selector: 'app-broadcast',
  templateUrl: './broadcast.component.html',
  styleUrls: ['./broadcast.component.scss'],
  animations :  [SharedAnimations]
})
export class BroadcastComponent implements OnInit {
  showClass = false;
  userTypeOptions = [];
  classLevelOptions = [];
  classOptions = [];
  emailForm: FormGroup;
  showWarning = false;
  email: Email;
  autocompletes$: any[] = [];


  constructor(private classService: ClassService, private alertify: AlertifyService,
    private adminService: AdminService, private fb: FormBuilder, private userService: UserService) { }


  ngOnInit() {
    this.createEmailForm();
    this.getUserTypes();
    this.getClassLevels();
    this.getAutoComplteList();
  }

  createEmailForm() {
    this.emailForm = this.fb.group({
      userType: [null, Validators.required],
      classLevel: [null, Validators.required],
      class: [null, Validators.required],
      subject: [''],
      body: ['']
    }, {validator: this.senderValidator});
  }

  senderValidator(g: FormGroup) {
    const sender = g.get('userType');
    const clevel = g.get('classLevel').value;
    const subject = g.get('subject').value;
    const body = g.get('body').value;

    if (sender.invalid && sender.dirty) {
      return {'senderNOK': true};
    } else {

    }
  }

  sendEmail() {
    const userTypeIds = this.emailForm.value.userType;
    const classLevelIds = this.emailForm.value.classLevel;
    const classIds = this.emailForm.value.class;
    const subject = this.emailForm.value.subject;
    const body = this.emailForm.value.body;

    const dataForEmails = <DataForBroadcast>{};
    dataForEmails.subject = subject;
    dataForEmails.body = body;

    dataForEmails.userTypeIds = userTypeIds;
    dataForEmails.classLevelIds = classLevelIds;
    dataForEmails.classIds = classIds;

    this.adminService.sendBroadcast(dataForEmails).subscribe(() => {
      this.alertify.successBar('messages envoyés.');
    }, error => {
      this.alertify.errorBar('problème avec l\'envoi des emails');
    });
  }

  getClassLevels() {
    this.classService.getLevels().subscribe((data: any) => {
      for (let i = 0; i < data.length; i++) {
        const elt = data[i];
        const option = {value: elt.id, label: elt.name};
        this.classLevelOptions = [...this.classLevelOptions, option];
      }
    }, error => {
      this.alertify.error(error);
    });
  }

  getUserTypes() {
    this.adminService.getUserTypes().subscribe((data: any) => {
      for (let i = 0; i < data.length; i++) {
        const elt = data[i];
        const option = {value: elt.id, label: elt.name};
        this.userTypeOptions = [...this.userTypeOptions, option];
      }
    });
  }

  getClasses(event) {

    const clevelIds = this.emailForm.value.classLevel;
    this.classOptions = [];

    if (clevelIds !== null && clevelIds.length !== 0) {
      if (clevelIds.length > 0) {
        this.showClass = true;
        this.classService.getClassLevelsWithClasses(clevelIds).subscribe((data: ClassLevel[]) => {
          for (let i = 0; i < data.length; i++) {
            const elt = data[i];
            const classes = data[i].classes;
            const optionGroup = {value: '', label: 'niveau ' + elt.name, group: true};
            this.classOptions = [...this.classOptions, optionGroup];
            for (let j = 0; j < classes.length; j++) {
              const aclass = classes[j];
              const nbStudents = aclass.students.length;
              if (nbStudents > 0) {
                const option = {value: aclass.id, label: 'classe ' + aclass.name + '(' + nbStudents + ')'};
                this.classOptions = [...this.classOptions, option];
              } else {
                const option = {value: aclass.id, label: 'classe ' + aclass.name + '(' + nbStudents + ')', disabled: true};
                this.classOptions = [...this.classOptions, option];
              }
            }
          }
        }, error => {
          this.alertify.error(error);
        });
      }
    } else {

      this.showClass = false;
      this.emailForm.value.class.reset();

    }
  }

  getAutoComplteList() {
    this.userService.getUsers().subscribe((data: any) => {
      for (let i = 0; i < data.length; i++) {
        const elt = data[i];
        const label = elt.lastName + ' ' + elt.firstName;
        const val = elt.email;
        const listElt = {display: label, value: val};
        this.autocompletes$ = [...this.autocompletes$, listElt];
      }
    });
  }

}
