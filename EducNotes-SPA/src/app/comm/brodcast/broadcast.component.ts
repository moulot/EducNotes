import { Component, OnInit } from '@angular/core';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { FormGroup, FormBuilder, FormArray, Validators } from '@angular/forms';
import { AdminService } from 'src/app/_services/admin.service';
import { Email } from 'src/app/_models/email';
import { UserService } from 'src/app/_services/user.service';
import { DataForBroadcast } from 'src/app/_models/dataForBroadcast';
import { ClassForBroadCast } from 'src/app/_models/classForBroadCast';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-broadcast',
  templateUrl: './broadcast.component.html',
  styleUrls: ['./broadcast.component.scss'],
  animations :  [SharedAnimations]
})
export class BroadcastComponent implements OnInit {
  parentTypeId = environment.parentTypeId;
  studentTypeId = environment.studentTypeId;
  teacherTypeId = environment.teacherTypeId;
  showClass = false;
  userTypeOptions = [];
  classLevelOptions = [];
  classOptions = [];
  emailForm: FormGroup;
  showWarning = false;
  email: Email;
  autocompletes$: any[] = [];
  cycles: any;
  educLevels: any;
  schools: any;
  classLevels: any;
  classes: ClassForBroadCast[] = [];
  templates: any;
  templateOptions = [];
  bodyType: number;
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

  constructor(private classService: ClassService, private alertify: AlertifyService,
    private adminService: AdminService, private fb: FormBuilder, private userService: UserService) { }

  ngOnInit() {
    this.showTo = true;
    this.showGroups = true;
    this.bodyType = 1;
    this.createEmailForm();
    this.addUserTypeItem();
    this.getData();
    this.getClassLevels();
    this.getEmailTemplatesData();
  }

  createEmailForm() {
    this.emailForm = this.fb.group({
      userTypes: this.fb.array([]),
      educLevels: this.fb.array([]),
      schools: this.fb.array([]),
      cycles: this.fb.array([]),
      classLevels: this.fb.array([]),
      classes: this.fb.array([]),
      subject: ['', Validators.required],
      type: [null],
      template: [null],
      body: ['']
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

    const template = g.get('template').value;
    const body = g.get('body').value;
    let bodyerror = false;
    console.log(template);
    if ((template === null || template === 'undefined') && body === '') {
      console.log('in body error');
      bodyerror = true;
    }

    if (userserror === true || classeserror === true || bodyerror === true) {
      return {'usersNOK': userserror, 'classesNOK': classeserror, 'bodyNOK': bodyerror, 'formNOK': true};
    }

    return null;
  }

  addUserTypeItem(): void {
    const userTypes = this.emailForm.get('userTypes') as FormArray;
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
    const educLevels = this.emailForm.get('educLevels') as FormArray;
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
    const schools = this.emailForm.get('schools') as FormArray;
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
    const cycles = this.emailForm.get('cycles') as FormArray;
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
    const cycles = this.emailForm.get('classLevels') as FormArray;
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
    const classes = this.emailForm.get('classes') as FormArray;
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
    this.adminService.getEmailBroadCastData().subscribe((data: any) => {
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

  getEmailTemplatesData() {
    this.adminService.getEmailTemplatesData().subscribe(data => {
      this.templates = data;
      for (let i = 0; i < this.templates.length; i++) {
        const elt = this.templates[i];
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
    const classes = this.emailForm.get('classes') as FormArray;
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
    const classes = this.emailForm.get('classes') as FormArray;
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
    const classes = this.emailForm.get('classes') as FormArray;
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
    const educLevels = this.emailForm.get('educLevels') as FormArray;
    const classes = this.emailForm.get('classes') as FormArray;
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
    const schools = this.emailForm.get('schools') as FormArray;
    const classes = this.emailForm.get('classes') as FormArray;
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
    const classLevels = this.emailForm.get('classLevels') as FormArray;
    const classes = this.emailForm.get('classes') as FormArray;
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
    const classes = this.emailForm.get('classes') as FormArray;
    for (let i = 0; i < classes.length; i++) {
      const elt = classes.at(i);
      if (elt.value.cycleId === id) {
        elt.get('selected').setValue(active);
      }
    }
  }

  selectBody() {
    this.bodyType = this.emailForm.value.type;
    this.emailForm.get('subject').setValue('');
    this.emailForm.get('body').setValue('');
    this.emailForm.get('template').reset();
  }

  setTemplateData() {
    const id = this.emailForm.value.template;
    if (id) {
      const tplSubject = this.templates.find(t => t.id === id).subject;
      this.emailForm.get('subject').setValue(tplSubject);
    }
  }

  getRecipients() {
    this.toData = '';
    for (let i = 0; i < this.emailForm.value.userTypes.length; i++) {
      const elt = this.emailForm.value.userTypes[i];
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

    for (let i = 0; i < this.emailForm.value.educLevels.length; i++) {
      const elt = this.emailForm.value.educLevels[i];
      if (elt.active) {
        if (this.groupData === '') {
          this.groupData = elt.name;
        } else {
          this.groupData += ', ' + elt.name;
        }
      }
    }

    for (let i = 0; i < this.emailForm.value.schools.length; i++) {
      const elt = this.emailForm.value.schools[i];
      if (elt.active) {
        if (this.groupData === '') {
          this.groupData = elt.name;
        } else {
          this.groupData += ', ' + elt.name;
        }
      }
    }

    for (let i = 0; i < this.emailForm.value.classLevels.length; i++) {
      const elt = this.emailForm.value.classLevels[i];
      if (elt.active) {
        if (this.groupData === '') {
          this.groupData = elt.name;
        } else {
          this.groupData += ', ' + elt.name;
        }
      }
    }

    for (let i = 0; i < this.emailForm.value.classes.length; i++) {
      const elt = this.emailForm.value.classes[i];
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

  sendEmail() {
    const userTypeIds = this.emailForm.value.userType;
    const classIds = this.emailForm.value.class;
    const subject = this.emailForm.value.subject;
    const body = this.emailForm.value.body;
    const templateId = this.emailForm.value.template;

    const dataForEmails = <DataForBroadcast>{};
    if (Number(this.bodyType) === 1) {
      dataForEmails.EmailTemplateId = templateId;
      dataForEmails.subject = '';
      dataForEmails.body = '';
    } else {
      dataForEmails.EmailTemplateId = 0;
      dataForEmails.subject = subject;
      dataForEmails.body = body;
    }

    dataForEmails.userTypeIds = userTypeIds;
    dataForEmails.classIds = classIds;

    this.adminService.sendEmailBroadcast(dataForEmails).subscribe(() => {
      this.alertify.successBar('messages envoyés.');
    }, error => {
      this.alertify.errorBar('problème avec l\'envoi des emails');
    });
  }

}
