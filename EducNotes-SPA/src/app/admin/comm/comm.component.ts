import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { AdminService } from 'src/app/_services/admin.service';
import { Email } from 'src/app/_models/email';
import { DataForEmail } from 'src/app/_models/dataForEmail';
import { ClassLevel } from 'src/app/_models/classLevel';

@Component({
  selector: 'app-comm',
  templateUrl: './comm.component.html',
  styleUrls: ['./comm.component.scss'],
  animations :  [SharedAnimations]
})
export class CommComponent implements OnInit {
  showClass = false;
  userTypeOptions = [];
  classLevelOptions = [];
  classOptions = [];
  emailForm: FormGroup;
  showWarning = false;
  email: Email;

  constructor(private classService: ClassService, private alertify: AlertifyService,
    private adminService: AdminService, private fb: FormBuilder) { }


  ngOnInit() {
    this.createEmailForm();
    this.getUserTypes();
    this.getClassLevels();
  }

  createEmailForm() {
    this.emailForm = this.fb.group({
      userType: [null, Validators.required],
      classLevel: [null, Validators.required],
      class: [null],
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

    const dataForEmail = <DataForEmail>{};
    dataForEmail.subject = subject;
    dataForEmail.body = body;

    if (userTypeIds) {
      userTypeIds.split(',');
      for (let i = 0; i < userTypeIds.length; i++) {
        const elt = userTypeIds[i];
        dataForEmail.userTypeIds = [...dataForEmail.userTypeIds, elt];
      }
    }

    if (classLevelIds) {
      classLevelIds.split(',');
      for (let i = 0; i < classLevelIds.length; i++) {
        const elt = classLevelIds[i];
        dataForEmail.classLevelIds = [...dataForEmail.classLevelIds, elt];
      }
    }

    if (classIds) {
      classIds.split(',');
      for (let i = 0; i < classIds.length; i++) {
        const elt = classIds[i];
        dataForEmail.classIds = [...dataForEmail.classIds, elt];
      }
    }

    // console.log(usertypeId + ' - ' + classLevelId + ' - ' + classIds);

    // this.adminService.sendEmails(dataForEmail).subscribe(() => {
    //   this.alertify.successBar('messages envoyés.');
    // }, error => {
    //   this.alertify.errorBar('problème avec l\'envoi des emails');
    // });
  }

  getClassLevels() {
    this.classService.getLevels().subscribe((data: any) => {
      this.classLevelOptions = [...this.classLevelOptions, {value: 'all', label: 'tous...'}];
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

  getClasses() {

    const clevelIds = this.emailForm.value.classLevel;
    this.classOptions = [];

    if (clevelIds !== null && clevelIds.length !== 0) {

      if (clevelIds.length > 0) {

        this.showClass = true;
        this.classService.getClassLevelsWithClasses(clevelIds).subscribe((data: ClassLevel[]) => {
          console.log(data);
          for (let i = 0; i < data.length; i++) {
            const elt = data[i];
            const classes = data[i].classes;
            const optionGroup = {value: '', label: 'niveau ' + elt.name, group: true};
            this.classOptions = [...this.classOptions, optionGroup];
            for (let j = 0; j < classes.length; j++) {
              const aclass = classes[j];
              if (aclass.totalStudent > 0) {
                const option = {value: aclass.id, label: 'classe ' + aclass.name + '(' + aclass.totalStudent + ')'};
                this.classOptions = [...this.classOptions, option];
              } else {
                const option = {value: aclass.id, label: 'classe ' + aclass.name + '(' + aclass.totalStudent + ')', disabled: true};
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

}
