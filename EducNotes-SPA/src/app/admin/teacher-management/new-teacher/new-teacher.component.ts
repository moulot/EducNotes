import { Component, OnInit,  EventEmitter, Input, Output } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { Course } from 'src/app/_models/course';
import { Validators, FormGroup, FormBuilder } from '@angular/forms';
import { AdminService } from 'src/app/_services/admin.service';
import { environment } from 'src/environments/environment';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { Utils } from 'src/app/shared/utils';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from 'src/app/_services/auth.service';
import { UserForRegister } from 'src/app/_models/userForRegister';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-new-teacher',
  templateUrl: './new-teacher.component.html',
  styleUrls: ['./new-teacher.component.scss'],
  animations :  [SharedAnimations]
})
export class NewTeacherComponent implements OnInit {
  courses: Course[] = [];
  teacherForm: FormGroup;
  submitText = 'enregistrer';
  teacherTypeId = environment.teacherTypeId;
  teacher: UserForRegister;
  userId: number;
  editModel: any;
  editionMode = 'add';
  teachersCourses: any[];
  phoneMask = Utils.phoneMask;
  birthDateMask = Utils.birthDateMask;
  currentUserId: number;
  photoUrl = '';
  photoFile: File;
  selectedCourses = [];
  gender = [{value: 0, label: 'femme'}, {value: 1, label: 'homme'}];

  constructor(private classService: ClassService, private fb: FormBuilder, private authService: AuthService,
    private router: Router, private adminService: AdminService, private userService: UserService,
    private alertify: AlertifyService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.currentUserId = this.authService.currentUser.id;
    this.route.data.subscribe(data => {
      if (data.teacher) {
        this.editModel = data.teacher;
        this.editionMode = 'edit';
        this.userId = data.teacher.id;
        this.editModel.courseIds = [];
        for (let i = 0; i < data.teacher.courses.length; i++) {
          const course = data.teacher.courses[i];
          this.editModel.courseIds = [...this.editModel.courseIds, course.id];
        }
      } else {
        this.initializeParams();
      }
      this.getCourses();
    });

    this.createTeacherForm();
  }

  getCourses() {
    if (this.courses.length === 0) {
      this.classService.getAllCourses().subscribe(data => {
        this.courses = data;
        if (this.editionMode = 'add') {
          this.editModel.courseIds = [];
        }
        for (let i = 0; i < this.courses.length; i++) {
          const elt = this.courses[i];
          let selected = false;
          if (this.editModel.courseIds.findIndex(c => c.courseId === elt.id) !== -1) {
            selected = true;
          }
          const sel = {courseId: elt.id, active: selected};
          this.selectedCourses = [...this.selectedCourses, sel];
        }
        // console.log(this.selectedCourses);
    }, error => {
        this.alertify.error(error);
      });
    }
  }

  createTeacherForm() {
    this.teacherForm = this.fb.group({
      lastName: [this.editModel.lastName, Validators.required],
      firstName: [this.editModel.firstName, Validators.required],
      dateOfBirth: [this.editModel.dateOfBirth, Validators.required],
      gender: [this.editModel.gender, Validators.required],
      email: [this.editModel.email, [Validators.required, Validators.email]],
      courseId: [],
      courseIds: [this.editModel.courseIds],
      cell: [this.editModel.cell, Validators.nullValidator],
      phone2: [this.editModel.phone2, Validators.nullValidator]
    });
  }

  initializeParams() {
    this.editModel = {
      dateOfBirth : null,
      gender: null,
      lastName : '',
      firstName : '',
      courseIds : null,
      cell : '',
      email: '',
      phone2 : ''
    };
  }

  imgResult(event) {
    let file: File = null;
    file = <File>event.target.files[0];

    // recuperation de l'url de la photo
    const reader = new FileReader();
    reader.onload = (e: any) => {
      this.photoUrl = e.target.result;
    };
    reader.readAsDataURL(event.target.files[0]);

    this.photoFile = file;
    console.log(this.photoFile);
  }

  setCourse(courseId) {
    // set the course active state
    const active = this.teacherForm.value.courseId;
    this.selectedCourses.find(c => c.courseId === courseId).active = active;
    // console.log(this.selectedCourses);
    let ids = '';
    // set the courses selected for the teacher
    for (let i = 0; i < this.selectedCourses.length; i++) {
      const elt = this.selectedCourses[i];
      if (elt.active === true) {
        if (ids === '') {
          ids = elt.courseId;
        } else {
          ids += ',' + elt.courseId;
        }
      }
    }
    this.teacherForm.controls['courseIds'].setValue(ids);
    // console.log(this.teacherForm.value.courseIds);
  }

  addTeacher() {
    this.teachersCourses = [];
    this.submitText = 'patienter...';

    const formData = new FormData();
    formData.append('photoFile', this.photoFile, this.photoFile.name);
    formData.append('lastName', this.teacherForm.value.lastName);
    formData.append('firstName', this.teacherForm.value.firstName);
    formData.append('gender', this.teacherForm.value.gender);
    formData.append('dateOfBirth', this.teacherForm.value.dateOfBirth);
    formData.append('email', this.teacherForm.value.email);
    formData.append('phoneNumber', this.teacherForm.value.cell);
    formData.append('secondPhoneNumber', this.teacherForm.value.phone2);
    formData.append('courseIds', this.teacherForm.value.courseIds);
    formData.append('userTypeId', this.teacherTypeId.toString());

    // this.teacher = Object.assign({}, this.teacherForm.value);
    // const dtBirth = this.teacherForm.value.dateOfBirth;
    // console.log(dtBirth);
    // if (dtBirth) {
    //   this.teacher.dateOfBirth = Utils.inputDateDDMMYY(this.teacherForm.value.dateOfBirth, '/');
    // }
    // console.log(this.teacher.dateOfBirth);

    // this.teacher.courseIds = this.teacherForm.value.courseIds.split(',');
    // this.teacher.photoFile = this.photoFile;
    // console.log(this.teacher);
    // this.teacher.userTypeId = this.teacherTypeId;

    if (this.editionMode === 'add') {
      // nouvelle enregistrement
      this.userService.addUser(formData).subscribe(() => {
        this.alertify.success('enseignant ajouté avec succès');
        this.submitText = 'enregistrer';
        this.router.navigate(['/teachers']);
      }, error => {
        this.submitText = 'enregistrer';
        console.log(error);
        this.alertify.error(error);
      });
    }

    if (this.editionMode === 'edit') {
      this.classService.updateTeacher(this.userId, this.teacher).subscribe(() => {
        this.alertify.success('enregistrement terminé');
        this.submitText = 'enregistrer';
        this.router.navigate(['/teachers']);
      }, error => {
        this.alertify.error(error);
        this.submitText = 'enregistrer';
      });
    }
  }

  savePhotos() {
    const img = this.photoFile;
    // if (img) {
    //   const formData = new FormData();
    //   formData.append('file', img.image, img.image.name);
    //   this.authService.addUserPhoto(elt.userId, formData).subscribe(() => {
    //   }, error => {
    //     this.alertify.error(error);
    //   });
    // }
    // if (elt === this.usersSpaCode[this.usersSpaCode.length - 1]) {
    // this.showSuccessDiv = true;
    // }
  }

}
