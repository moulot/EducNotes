import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { FormGroup, Validators, FormBuilder, FormControl } from '@angular/forms';
import { Email } from 'src/app/_models/email';
import { DataForEmail } from 'src/app/_models/dataForEmail';
import { ClassLevel } from 'src/app/_models/classLevel';
import { UserService } from 'src/app/_services/user.service';
import { CommService } from 'src/app/_services/comm.service';

@Component({
  selector: 'app-email',
  templateUrl: './email.component.html',
  styleUrls: ['./email.component.scss'],
  animations :  [SharedAnimations]
})
export class EmailComponent implements OnInit {
  showClass = false;
  userTypeOptions = [];
  classLevelOptions = [];
  classOptions = [];
  emailForm: FormGroup;
  showWarning = false;
  email: Email;
  autocompletes$: any[] = [];


  constructor(private classService: ClassService, private alertify: AlertifyService,
    private commService: CommService, private fb: FormBuilder, private userService: UserService) { }


  ngOnInit() {
    this.createEmailForm();
    this.getUserTypes();
    this.getClassLevels();
    this.getAutoComplteList();
  }

  createEmailForm() {
    this.emailForm = this.fb.group({
      tagsCtrlTos: ['', Validators.required],
      tagsCtrlCcs: [''],
      // tagsCtrlBccs: [''],
      subject: [''],
      body: ['']
    }, {validator: this.senderValidator});
  }

  senderValidator(g: FormGroup) {
    const subject = g.get('subject').value;
    const body = g.get('body').value;

  }

  sendEmail() {
    const Tos = this.emailForm.value.tagsCtrlTos;
    const Ccs = this.emailForm.value.tagsCtrlCcs;
    const subject = this.emailForm.value.subject;
    const body = this.emailForm.value.body;

    let toEmails = '';
    for (let i = 0; i < Tos.length; i++) {
      const to = Tos[i];
      const email = to.value;
      if (toEmails === '') {
        toEmails = email;
      } else {
        toEmails += ';' + email;
      }
    }

    let ccEmails = '';
    for (let i = 0; i < Ccs.length; i++) {
      const cc = Ccs[i];
      const email = cc.value;
      if (ccEmails === '') {
        ccEmails = email;
      } else {
        ccEmails += ';' + email;
      }
    }

    const dataForEmail = <DataForEmail>{};
    dataForEmail.subject = subject;
    dataForEmail.body = body;
    dataForEmail.tos = toEmails;
    dataForEmail.ccs = ccEmails;

    this.commService.sendEmail(dataForEmail).subscribe(() => {
      this.alertify.successBar('message envoyé.');
    }, error => {
      this.alertify.errorBar('problème avec l\'envoi du email');
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
    this.userService.getUserTypes().subscribe((data: any) => {
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
