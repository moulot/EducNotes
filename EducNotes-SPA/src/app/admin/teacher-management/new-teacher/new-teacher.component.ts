import { Component, OnInit,  EventEmitter, Input, Output } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { Course } from 'src/app/_models/course';
import { Validators, FormGroup, FormBuilder } from '@angular/forms';
import { AdminService } from 'src/app/_services/admin.service';
import { environment } from 'src/environments/environment';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { User } from 'src/app/_models/user';
import { Utils } from 'src/app/shared/utils';

@Component({
  selector: 'app-new-teacher',
  templateUrl: './new-teacher.component.html',
  styleUrls: ['./new-teacher.component.scss'],
  animations :  [SharedAnimations]
})
export class NewTeacherComponent implements OnInit {
  @Input()  teacherDetail: any;
  courses: Course[] = [];
  teacherForm: FormGroup;
  submitText = 'enregistrer';
  teacherTypeId = environment.teacherTypeId;
  teacher: User;
  userId: number;
  editModel: any;
  editionMode = 'add';
  teachersCourses: any[];
  phoneMask = [/\d/, /\d/, '-', /\d/, /\d/, '-', /\d/, /\d/, '-', /\d/, /\d/];
  birthDateMask = [/\d/, /\d/, '/', /\d/, /\d/, '/', /\d/, /\d/, /\d/, /\d/];

  @Output() addUserResult = new EventEmitter();



  constructor(private classService: ClassService, private fb: FormBuilder,
    private adminService: AdminService, private alertify: AlertifyService) { }

  ngOnInit() {

    if (this.teacherDetail != null) {
      this.editionMode = 'edit';
      this.editModel = this.teacherDetail;
      this.userId = this.teacherDetail.id;
     } else {
       this.initializeParams();
     }

    this.getAllCourses();
    this.createTeacherForm();
  }


  getAllCourses() {
    if (this.courses.length === 0) {
      this.classService.getAllCourses().subscribe(data => {
        this.courses = data;
      }, error => {
        console.log(error);
      });

    }
  }



  createTeacherForm() {

    this.teacherForm = this.fb.group({
      lastName: [this.editModel.lastName, Validators.required],
      firstName: [this.editModel.firstName, Validators.nullValidator],
      dateOfBirth: [this.editModel.dateOfBirth, Validators.nullValidator],
      email: [this.editModel.email, [Validators.required, Validators.email]],
      courseIds: [this.editModel.courseIds, Validators.nullValidator],
      phoneNumber: [this.editModel.phoneNumber, Validators.required],
      secondPhoneNumber: [this.editModel.secondPhoneNumber, Validators.nullValidator]});
  }



  initializeParams() {
    this.editModel = {
      dateOfBirth : null,
      lastName : '',
      firstName : '',
      courseIds : null,
      phoneNumber : '',
      email: '',
      secondPhoneNumber : ''
    };


  }



  submit() {
    this.teachersCourses = [];
    this.submitText = 'patienter...';

    this.teacher = Object.assign({}, this.teacherForm.value);
    // console.log(this.teacher);
    const dtBirth = this.teacherForm.value.dateOfBirth;
    // console.log(dtBirth);
    if (dtBirth) {
      this.teacher.dateOfBirth = Utils.inputDateDDMMYY(this.teacherForm.value.dateOfBirth, '/');
    }
    console.log(this.teacher.dateOfBirth);

    this.teacher.userTypeId = this.teacherTypeId;
    if (this.editionMode === 'add') {
      this.adminService.emailExist(this.teacher.email).subscribe((res: boolean) => {
            if (res === true) {
              this.alertify.infoBar('l\'email ' + this.teacher.email + ' est déja utlilisé ');
              this.submitText = 'enregistrer';

            } else {
              this.adminService.saveTeacher(this.teacher).subscribe((response: any) => {
            //   this.visible = false;
              response.type = 'new';
              // this.teachersCourses = [...this.teachersCourses, response];
            // this.teachersCourses.unshift(response);
              this.alertify.success('enregistrement terminé...');
              this.submitText = 'enregistrer';
              this.addUserResult.emit(true);
            }, error => {
                this.submitText = 'enregistrer';
                this.alertify.error(error);
              });

            }
          });
    }

    if (this.editionMode === 'edit') {
      this.classService.updateTeacher(this.userId, this.teacher).subscribe(() => {
        this.alertify.success('enregistrement terminé');
        this.submitText = 'enregistrer';
        this.addUserResult.emit(true);
      }, error => {
        this.alertify.error(error);
        this.submitText = 'enregistrer';});
    }

  }


  close() {
    this.addUserResult.emit(false);
  }

}
