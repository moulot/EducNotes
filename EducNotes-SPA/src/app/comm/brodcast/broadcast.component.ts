import { Component, OnInit } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { FormGroup, FormBuilder, FormArray, Validators } from '@angular/forms';
import { Email } from 'src/app/_models/email';
import { ClassForBroadCast } from 'src/app/_models/classForBroadCast';
import { environment } from 'src/environments/environment';
import { Router } from '@angular/router';
import { CommService } from 'src/app/_services/comm.service';

@Component({
  selector: 'app-broadcast',
  templateUrl: './broadcast.component.html',
  styleUrls: ['./broadcast.component.scss']
})
export class BroadcastComponent implements OnInit {
  parentTypeId = environment.parentTypeId;
  studentTypeId = environment.studentTypeId;
  teacherTypeId = environment.teacherTypeId;
  showClass = false;
  userTypeOptions = [];
  classLevelOptions = [];
  classOptions = [];
  msgForm: FormGroup;
  showWarning = false;
  email: Email;
  autocompletes$: any[] = [];
  cycles: any;
  educLevels: any;
  schools: any;
  classLevels: any;
  classes: ClassForBroadCast[] = [];
  emailTemplates: any;
  smsTemplates: any;
  templateOptions = [];
  emailBodyType: number;
  smsBodyType: number;
  showTo = true;
  toData: string;
  showGroups = true;
  educLevelData: string;
  schoolData: string;
  classesData: string;
  groupData: string;
  showBody = false;
  educLevelClassList: any[] = [];
  schoolClassList: any[] = [];
  classLevelClassList: any[] = [];
  toggleEmailSms = true;
  btnActive = 0;
  showRecapPage = false;
  recipients: any;
  usersNOK: any;
  subject: string;
  body: string;
  sendNotValidatedUsers = false;

  constructor(private classService: ClassService, private alertify: AlertifyService,
    private router: Router, private fb: FormBuilder, private commService: CommService) { }

  ngOnInit() {
    this.showTo = true;
    this.showGroups = true;
    this.emailBodyType = 1;
    this.smsBodyType = 1;
    this.createMsgForm();
    this.addUserTypeItem();
    this.getData();
    this.getClassLevels();
    this.getEmailTemplates();
    this.getSmsTemplates();
  }

  createMsgForm() {
    this.msgForm = this.fb.group({
      userTypes: this.fb.array([]),
      educLevels: this.fb.array([]),
      schools: this.fb.array([]),
      cycles: this.fb.array([]),
      classLevels: this.fb.array([]),
      classes: this.fb.array([]),
      sendToUsersNOK: [false],
      msgType: [0],
      msgChoice: [1],
      emailType: [1],
      emailSubject: ['', Validators.required],
      emailTemplate: [null],
      emailBody: [''],
      smsType: [1],
      smsTemplate: [null],
      smsBody: ['']
    }, {validator: this.formValidator});
  }

  formValidator(g: FormGroup) {
    const userTypes = g.get('userTypes') as FormArray;
    let userserror = false;
    let usersSelected = false;
    for (let i = 0; i < userTypes.length; i++) {
      const elt = userTypes.controls[i];
      if (elt.value.active === true) {
        usersSelected = true;
        break;
      }
    }
    if (!usersSelected) {
      userserror = true;
    }

    const classes = g.get('classes') as FormArray;
    let classeserror = false;
    let classesSelected = false;
    for (let i = 0; i < classes.length; i++) {
      const elt = classes.controls[i];
      if (elt.value.selected === true) {
        classesSelected = true;
        break;
      }
    }
    if (!classesSelected) {
      classeserror = true;
    }

    const msgChoice = g.get('msgChoice').value;
    let bodyerror = false;
    const emailtemplate = g.get('emailTemplate').value;
    const emailbody = g.get('emailBody').value;
    const emailType = g.get('emailType').value;
    // console.log('email: ' + msgChoice + '-' + emailtemplate + '-' + emailbody + '-' + emailType);
    if (msgChoice === 1 && emailType === 2) {
      if ((emailtemplate === null || emailtemplate === 'undefined') && emailbody === '') {
        bodyerror = true;
      }
    }

    const smstemplate = g.get('smsTemplate').value;
    const smsbody = g.get('smsBody').value;
    const smsType = g.get('smsType').value;
    // console.log('sms: ' + msgChoice + '-' + smstemplate + '-' + smsbody + '-' + smsType);
    if (msgChoice === 2 && smsType === 2) {
      if ((smstemplate === null || smstemplate === 'undefined') && smsbody === '') {
        bodyerror = true;
      }
    }

    // console.log('errors: usersNOK:' + userserror + ' classesNOK:' + classeserror + ' bodyerror:' + bodyerror);
    if (userserror === true || classeserror === true || bodyerror === true) {
      return {'usersNOK': userserror, 'classesNOK': classeserror, 'bodyNOK': bodyerror, 'formNOK': true};
    }

    return null;
  }

  addUserTypeItem(): void {
    const userTypes = this.msgForm.get('userTypes') as FormArray;
    userTypes.push(this.createUserTypeItem(this.parentTypeId, 'tous les parents'));
    userTypes.push(this.createUserTypeItem(this.studentTypeId, 'tous les élèves'));
    userTypes.push(this.createUserTypeItem(this.teacherTypeId, 'tous les profs'));
    userTypes.push(this.createUserTypeItem(0, 'tous les employés'));
  }

  createUserTypeItem(id, name): FormGroup {
    return this.fb.group({
      userTypeId: id,
      name: name,
      active: [false]
    });
  }

  addEducLevelItem(id, name): void {
    const educLevels = this.msgForm.get('educLevels') as FormArray;
    educLevels.push(this.createEducLevelItem(id, name));
  }

  createEducLevelItem(id, name): FormGroup {
    return this.fb.group({
      educLevelId: id,
      name: name,
      active: [false]
    });
  }

  addSchoolItem(id, name): void {
    const schools = this.msgForm.get('schools') as FormArray;
    schools.push(this.createSchoolItem(id, name));
  }

  createSchoolItem(id, name): FormGroup {
    return this.fb.group({
      schoolId: id,
      name: name,
      active: [false]
    });
  }

  addCycleItem(id, name): void {
    const cycles = this.msgForm.get('cycles') as FormArray;
    cycles.push(this.createCycleItem(id, name));
  }

  createCycleItem(id, name): FormGroup {
    return this.fb.group({
      cycleId: id,
      name: name,
      active: [false]
    });
  }

  addClassLevelItem(id, name): void {
    const cycles = this.msgForm.get('classLevels') as FormArray;
    cycles.push(this.createClassLevelItem(id, name));
  }

  createClassLevelItem(id, name): FormGroup {
    return this.fb.group({
      classLevelId: id,
      name: name,
      active: [false]
    });
  }

  addClassItem(id, name, classLevelId, educationLevelId, schoolId, cycleId): void {
    const classes = this.msgForm.get('classes') as FormArray;
    classes.push(this.createClassItem(id, name, classLevelId, educationLevelId, schoolId, cycleId));
  }

  createClassItem(id, name, classLevelId, educationLevelId, schoolId, cycleId): FormGroup {
    return this.fb.group({
      classId: id,
      name: name,
      classLevelId: classLevelId,
      educationLevelId: educationLevelId,
      schoolId: schoolId,
      cycleId: cycleId,
      active: [false],
      selected: [false]
    });
  }

  getData() {
    this.commService.getEmailBroadCastData().subscribe((data: any) => {
      this.schools = data.schools;
      this.cycles = data.cycles;
      this.educLevels = data.educLevels;

      // set education level form controls
      for (let i = 0; i < this.educLevels.length; i++) {
        const elt = this.educLevels[i];
        this.addEducLevelItem(elt.id, elt.name);
      }

      // set school form controls
      for (let i = 0; i < this.schools.length; i++) {
        const elt = this.schools[i];
        this.addSchoolItem(elt.id, elt.name);
      }

      // set cycle form controls
      for (let i = 0; i < this.cycles.length; i++) {
        const elt = this.cycles[i];
        this.addCycleItem(elt.id, elt.name);
      }
    }, error => {
      this.alertify.error(error);
    });
  }

  getClassLevels() {
    this.classService.getLevelsWithClasses().subscribe(data => {
      this.classLevels = data;
      for (let i = 0; i < this.classLevels.length; i++) {
        const cl = this.classLevels[i];
        for (let j = 0; j < cl.classes.length; j++) {
          const aclass = cl.classes[j];
          const classData: ClassForBroadCast = {
            id: aclass.id,
            name: aclass.name,
            classLevelId: aclass.classLevelId,
            cycleId: aclass.cycleId,
            schoolId: aclass.schoolId,
            educationLevelId: aclass.educationLevelId,
            active: false
          };
          this.classes = [...this.classes, classData];
        }
      }

      // set classLevel form controls
      for (let i = 0; i < this.classLevels.length; i++) {
        const elt = this.classLevels[i];
        this.addClassLevelItem(elt.id, elt.name);
      }

      // set class form controls
      for (let i = 0; i < this.classes.length; i++) {
        const elt = this.classes[i];
        this.addClassItem(elt.id, elt.name, elt.classLevelId, elt.educationLevelId, elt.schoolId, elt.cycleId);
      }
    });
  }

  getEmailTemplates() {
    this.commService.getEmailTemplates().subscribe(data => {
      this.emailTemplates = data;
      for (let i = 0; i < this.emailTemplates.length; i++) {
        const elt = this.emailTemplates[i];
        const tpl = {value: elt.id, label: elt.name + ' (' + elt.emailCategoryName + ')'};
        this.templateOptions = [...this.templateOptions, tpl];
      }
    }, error => {
      this.alertify.error(error);
    });
  }

  getSmsTemplates() {
    this.commService.getSmsTemplates().subscribe(data => {
      this.smsTemplates = data;
      for (let i = 0; i < this.smsTemplates.length; i++) {
        const elt = this.smsTemplates[i];
        const tpl = {value: elt.id, label: elt.name + ' (' + elt.emailCategoryName + ')'};
        this.templateOptions = [...this.templateOptions, tpl];
      }
    }, error => {
      this.alertify.error(error);
    });
  }

  educLevelSelect(educLevel) {
    const active = educLevel.active;
    const id = educLevel.educLevelId;
    const classes = this.msgForm.get('classes') as FormArray;
    for (let i = 0; i < classes.length; i++) {
      const elt = classes.at(i);
      if (elt.value.educationLevelId === id) {
        elt.get('selected').setValue(active);
        this.educLevelClassList = [...this.educLevelClassList, {id: elt.value.classId, name: elt.value.name}];
      }
    }
    this.selectSchoolClasses();
    this.selectClassLevelClasses();
  }

  schoolSelect(school) {
    const active = school.active;
    const id = school.schoolId;
    const classes = this.msgForm.get('classes') as FormArray;
    for (let i = 0; i < classes.length; i++) {
      const elt = classes.at(i);
      if (elt.value.schoolId === id) {
        elt.get('selected').setValue(active);
        this.schoolClassList = [...this.schoolClassList, {id: elt.value.classId, name: elt.value.name}];
      }
    }
    this.selectEducLevelClasses();
    this.selectClassLevelClasses();
  }

  classLevelSelect(classLevel) {
    const active = classLevel.active;
    const id = classLevel.classLevelId;
    const classes = this.msgForm.get('classes') as FormArray;
    for (let i = 0; i < classes.length; i++) {
      const elt = classes.at(i);
      if (elt.value.classLevelId === id) {
        elt.get('selected').setValue(active);
        this.classLevelClassList = [...this.classLevelClassList, {id: elt.value.classId, name: elt.value.name}];
      }
    }
    this.selectEducLevelClasses();
    this.selectSchoolClasses();
  }

  selectEducLevelClasses() {
    const educLevels = this.msgForm.get('educLevels') as FormArray;
    const classes = this.msgForm.get('classes') as FormArray;
    for (let i = 0; i < educLevels.length; i++) {
      const el = educLevels.at(i);
      const id = el.value.educLevelId;
      const active = el.value.active;
      if (active) {
        for (let j = 0; j < classes.length; j++) {
          const elt = classes.at(j);
          if (elt.value.educLevelId === id) {
            elt.get('selected').setValue(active);
          }
        }
      }
    }
  }

  selectSchoolClasses() {
    const schools = this.msgForm.get('schools') as FormArray;
    const classes = this.msgForm.get('classes') as FormArray;
    for (let i = 0; i < schools.length; i++) {
      const el = schools.at(i);
      const id = el.value.schoolId;
      const active = el.value.active;
      if (active) {
        for (let j = 0; j < classes.length; j++) {
          const elt = classes.at(j);
          if (elt.value.schoolId === id) {
            elt.get('selected').setValue(active);
          }
        }
      }
    }
  }

  selectClassLevelClasses() {
    const classLevels = this.msgForm.get('classLevels') as FormArray;
    const classes = this.msgForm.get('classes') as FormArray;
    for (let i = 0; i < classLevels.length; i++) {
      const el = classLevels.at(i);
      const id = el.value.classLevelId;
      const active = el.value.active;
      if (active) {
        for (let j = 0; j < classes.length; j++) {
          const elt = classes.at(j);
          if (elt.value.classLevelId === id) {
            elt.get('selected').setValue(active);
          }
        }
      }
    }
  }

  cycleSelect(cycle) {
    const active = cycle.active;
    const id = cycle.cycleId;
    const classes = this.msgForm.get('classes') as FormArray;
    for (let i = 0; i < classes.length; i++) {
      const elt = classes.at(i);
      if (elt.value.cycleId === id) {
        elt.get('selected').setValue(active);
      }
    }
  }

  selectEmailBody() {
    this.emailBodyType = this.msgForm.value.emailType;
    console.log('emailBodyType:' + this.emailBodyType);
    this.msgForm.get('emailSubject').setValue('');
    this.msgForm.get('emailBody').setValue('');
    this.msgForm.get('emailTemplate').reset();
  }

  selectSmsBody() {
     this.toggleEmailSms = false;
    this.smsBodyType = this.msgForm.value.smsType;
    this.msgForm.get('smsBody').setValue('');
    this.msgForm.get('smsTemplate').reset();
  }

  setEmailTemplateData() {
    const id = this.msgForm.value.emailTemplate;
    if (id) {
      const tplSubject = this.emailTemplates.find(t => t.id === id).subject;
      this.msgForm.get('emailSubject').setValue(tplSubject);
    }
  }

  // setSmsTemplateData() {
  //   const id = this.msgForm.value.smsTemplate;
  //   if (id) {
  //   }
  // }

  getRecipients() {
    this.toData = '';
    for (let i = 0; i < this.msgForm.value.userTypes.length; i++) {
      const elt = this.msgForm.value.userTypes[i];
      if (elt.active) {
        if (this.toData === '') {
          this.toData = elt.name;
        } else {
          this.toData += ', ' + elt.name;
        }
      }
    }
    this.showTo = false;
  }

  showToForm() {
    this.showTo = true;
  }

  getGroups() {
    this.groupData = '';
    this.educLevelData = '';
    this.schoolData = '';
    this.classesData = '';

    for (let i = 0; i < this.msgForm.value.educLevels.length; i++) {
      const elt = this.msgForm.value.educLevels[i];
      if (elt.active) {
        if (this.groupData === '') {
          this.groupData = elt.name;
        } else {
          this.groupData += ', ' + elt.name;
        }
      }
    }

    for (let i = 0; i < this.msgForm.value.schools.length; i++) {
      const elt = this.msgForm.value.schools[i];
      if (elt.active) {
        if (this.groupData === '') {
          this.groupData = elt.name;
        } else {
          this.groupData += ', ' + elt.name;
        }
      }
    }

    for (let i = 0; i < this.msgForm.value.classLevels.length; i++) {
      const elt = this.msgForm.value.classLevels[i];
      if (elt.active) {
        if (this.groupData === '') {
          this.groupData = elt.name;
        } else {
          this.groupData += ', ' + elt.name;
        }
      }
    }

    for (let i = 0; i < this.msgForm.value.classes.length; i++) {
      const elt = this.msgForm.value.classes[i];
      if (elt.selected && this.educLevelClassList.findIndex(c => Number(c.id) === Number(elt.classId)) === -1 &&
          this.schoolClassList.findIndex(c => Number(c.id) === Number(elt.classId)) === -1 &&
          this.classLevelClassList.findIndex(c => Number(c.id) === Number(elt.classId)) === -1) {
        if (this.groupData === '') {
          this.groupData = elt.name;
        } else {
          this.groupData += ', ' + elt.name;
        }
      }
    }
    this.showGroups = false;
  }

  showGroupForm() {
    this.showGroups = true;
  }

  toggleBox($event) {
    this.toggleEmailSms = !this.toggleEmailSms;
  }

  selectBtn(index) {
    this.btnActive = index;
    this.msgForm.get('msgType').setValue(index);
  }

  setNotValidatedUsers() {
    this.sendNotValidatedUsers = !this.sendNotValidatedUsers;
  }

  getBroadcastRecap() {
    let userTypeIds = [];
    for (let i = 0; i < this.msgForm.value.userTypes.length; i++) {
      const userType = this.msgForm.value.userTypes[i];
      if (userType.active === true) {
        userTypeIds = [...userTypeIds, userType.userTypeId];
      }
    }
    let educLevelIds = [];
    for (let i = 0; i < this.msgForm.value.educLevels.length; i++) {
      const educLevel = this.msgForm.value.educLevels[i];
      if (educLevel.active === true) {
        educLevelIds = [...educLevelIds, educLevel.educLevelId];
      }
    }
    let schoolIds = [];
    for (let i = 0; i < this.msgForm.value.schools.length; i++) {
      const school = this.msgForm.value.schools[i];
      if (school.active === true) {
        schoolIds = [...schoolIds, school.schoolId];
      }
    }
    let classLevelIds = [];
    for (let i = 0; i < this.msgForm.value.classLevels.length; i++) {
      const classLevel = this.msgForm.value.classLevels[i];
      if (classLevel.active === true) {
        classLevelIds = [...classLevelIds, classLevel.schoolId];
      }
    }
    let classIds = [];
    for (let j = 0; j < this.msgForm.value.classes.length; j++) {
      const aclass = this.msgForm.value.classes[j];
      if (aclass.selected === true) {
        classIds = [...classIds, aclass.classId];
      }
    }

    const dataForMsgs = <any>{};
    const msgChoice = this.msgForm.value.msgChoice;
    if (msgChoice === 1) { // email
      const subject = this.msgForm.value.emailSubject;
      const body = this.msgForm.value.emailBody;
      const templateId = this.msgForm.value.emailTemplate;

      if (Number(this.emailBodyType) === 1) {
        dataForMsgs.templateId = templateId;
        dataForMsgs.subject = '';
        dataForMsgs.body = '';
      } else {
        dataForMsgs.emailTemplateId = 0;
        dataForMsgs.subject = subject;
        dataForMsgs.body = body;
      }
    } else { // sms
      const body = this.msgForm.value.smsBody;
      const templateId = this.msgForm.value.smsTemplate;

      if (Number(this.smsBodyType) === 1) {
        dataForMsgs.templateId = templateId;
        dataForMsgs.subject = '';
        dataForMsgs.body = '';
      } else {
        dataForMsgs.templateId = 0;
        dataForMsgs.subject = '';
        dataForMsgs.body = body;
      }
    }

    dataForMsgs.msgToRecipients = this.btnActive;
    dataForMsgs.msgType = msgChoice;
    dataForMsgs.sendToUSersNOK = this.msgForm.value.sendToUsersNOK;
    dataForMsgs.userTypeIds = userTypeIds;
    dataForMsgs.educLevelIds = educLevelIds;
    dataForMsgs.schoolIds = schoolIds;
    dataForMsgs.classLevelIds = classLevelIds;
    dataForMsgs.classIds = classIds;
    this.commService.getBroadcastRecap(dataForMsgs).subscribe((data: any) => {
      this.showRecapPage = true;
      this.recipients = data.recipients;
      this.usersNOK = data.usersNOK;
      this.subject = data.subject;
      this.body = data.body;
    }, error => {
      this.alertify.error('problème avec l\'envoi des emails');
    });
  }

  sendMessages() {
    let userTypeIds = [];
    for (let i = 0; i < this.msgForm.value.userTypes.length; i++) {
      const userType = this.msgForm.value.userTypes[i];
      if (userType.active === true) {
        userTypeIds = [...userTypeIds, userType.userTypeId];
      }
    }
    let educLevelIds = [];
    for (let i = 0; i < this.msgForm.value.educLevels.length; i++) {
      const educLevel = this.msgForm.value.educLevels[i];
      if (educLevel.active === true) {
        educLevelIds = [...educLevelIds, educLevel.educLevelId];
      }
    }
    let schoolIds = [];
    for (let i = 0; i < this.msgForm.value.schools.length; i++) {
      const school = this.msgForm.value.schools[i];
      if (school.active === true) {
        schoolIds = [...schoolIds, school.schoolId];
      }
    }
    let classLevelIds = [];
    for (let i = 0; i < this.msgForm.value.classLevels.length; i++) {
      const classLevel = this.msgForm.value.classLevels[i];
      if (classLevel.active === true) {
        classLevelIds = [...classLevelIds, classLevel.schoolId];
      }
    }
    let classIds = [];
    for (let j = 0; j < this.msgForm.value.classes.length; j++) {
      const aclass = this.msgForm.value.classes[j];
      if (aclass.selected === true) {
        classIds = [...classIds, aclass.classId];
      }
    }

    const dataForMsgs = <any>{};
    const msgChoice = this.msgForm.value.msgChoice;
    if (msgChoice === 1) {
      const subject = this.msgForm.value.emailSubject;
      const body = this.msgForm.value.emailBody;
      const templateId = this.msgForm.value.emailTemplate;

      dataForMsgs.msgType = msgChoice;
      if (Number(this.emailBodyType) === 1) {
        dataForMsgs.templateId = templateId;
        dataForMsgs.subject = '';
        dataForMsgs.body = '';
      } else {
        dataForMsgs.emailTemplateId = 0;
        dataForMsgs.subject = subject;
        dataForMsgs.body = body;
      }
    } else {
      const body = this.msgForm.value.smsBody;
      const templateId = this.msgForm.value.smsTemplate;

      if (Number(this.smsBodyType) === 1) {
        dataForMsgs.templateId = templateId;
        dataForMsgs.subject = '';
        dataForMsgs.body = '';
      } else {
        dataForMsgs.templateId = 0;
        dataForMsgs.subject = '';
        dataForMsgs.body = body;
      }
    }

    dataForMsgs.userTypeIds = userTypeIds;
    dataForMsgs.educLevelIds = educLevelIds;
    dataForMsgs.schoolIds = schoolIds;
    dataForMsgs.classLevelIds = classLevelIds;
    dataForMsgs.classIds = classIds;
    this.commService.ClassesBroadcastMessaging(dataForMsgs).subscribe(data => {
      this.alertify.success('messages envoyés.');
      this.router.navigate(['/home']);
    }, error => {
      this.alertify.error('problème avec l\'envoi des emails');
    });
  }

}
