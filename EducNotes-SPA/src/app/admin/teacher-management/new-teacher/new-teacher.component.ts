import { Component, OnInit } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { Course } from 'src/app/_models/course';
import { Validators, FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { AdminService } from 'src/app/_services/admin.service';
import { environment } from 'src/environments/environment';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { Utils } from 'src/app/shared/utils';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from 'src/app/_services/auth.service';
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
  teacherTypeId = environment.teacherTypeId;
  teacher: any; // = <TeacherForEdit>{};
  userId: number;
  editModel: any;
  editionMode = false;
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
      this.teacher = data['teacher'];
      if (this.teacher) {
        this.photoUrl = this.teacher.photoUrl;
        this.editionMode = true;
      } else {
        this.initValues();
      }
      this.getCourses();
    });

    this.createTeacherForm();
  }

  getCourses() {
    if (this.courses.length === 0) {
      this.classService.getAllCourses().subscribe(data => {
        this.courses = data;
        const assigned = this.teacher.classesAssigned;
        const ids = this.teacher.courseIds.split(',');
        for (let i = 0; i < this.courses.length; i++) {
          const elt = this.courses[i];
          let selected = false;
          let courseAssigned = false;
          if (assigned) {
            if (ids.findIndex((value) => value === elt.id.toString()) !== -1) {
              selected = true;
            }
            const courseClass = assigned.find(c => c.id === elt.id);
            if (courseClass) {
              courseAssigned = courseClass.classesAssigned;
            }
          }
          this.addCourseItem(elt.id, elt.name, selected, courseAssigned);
        }
      }, error => {
        this.alertify.error(error);
      });
    }
  }

  createTeacherForm() {
    this.teacherForm = this.fb.group({
      lastName: [this.teacher.lastName, Validators.required],
      firstName: [this.teacher.firstName, Validators.required],
      dateOfBirth: [this.teacher.strDateOfBirth, Validators.required],
      gender: [this.teacher.gender, Validators.required],
      email: [this.teacher.email, [Validators.required, Validators.email]],
      cell: [this.teacher.phoneNumber, Validators.nullValidator],
      phone2: [this.teacher.secondPhoneNumber, Validators.nullValidator],
      photoUrl: [this.teacher.photoUrl],
      courses: this.fb.array([])
    });
  }

  addCourseItem(id, name, active, assigned): void {
    const courses = this.teacherForm.get('courses') as FormArray;
    courses.push(this.createCourseItem(id, name, active, assigned));
  }

  createCourseItem(id, name, active, assigned): FormGroup {
    return this.fb.group({
      courseId: id,
      name: name,
      active: active,
      classAssigned: assigned
    });
  }

  initValues() {
    this.teacher = {
      id: 0,
      lastName: '',
      firstName: '',
      userName: '',
      phoneNumber: '',
      secondPhoneNumber: '',
      gender: null,
      email: '',
      dateOfBirth: null,
      strDateOfBirth: '',
      photoUrl: '',
      photoFile: null,
      courseIds: '',
      active: 1
    };
  }

  resetValues() {
    this.teacherForm.setValue({lastName: this.teacher.lastName, firstName: '', dateOfBirth: '', gender: null,
      email: '', cell: '', phone2: '', photoUrl: '', courses: this.fb.array([this.getCourses()])});
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
  }

  addTeacher() {
    // get the selected courses for the teacher
    let ids = '';
    for (let i = 0; i < this.teacherForm.value.courses.length; i++) {
      const elt = this.teacherForm.value.courses[i];
      if (elt.active === true) {
        if (ids === '') {
          ids = elt.courseId.toString();
        } else {
          ids += ',' + elt.courseId.toString();
        }
      }
    }

    const formData = new FormData();
    if (this.photoFile) {
      formData.append('photoFile', this.photoFile, this.photoFile.name);
    }
    formData.append('id', this.teacher.id.toString());
    formData.append('lastName', this.teacherForm.value.lastName);
    formData.append('firstName', this.teacherForm.value.firstName);
    formData.append('gender', this.teacherForm.value.gender);
    formData.append('strDateOfBirth', this.teacherForm.value.dateOfBirth);
    formData.append('email', this.teacherForm.value.email);
    formData.append('phoneNumber', this.teacherForm.value.cell);
    formData.append('secondPhoneNumber', this.teacherForm.value.phone2);
    formData.append('courseIds', ids);
    formData.append('userTypeId', this.teacherTypeId.toString());

    this.userService.addUser(formData).subscribe(() => {
      this.alertify.success('enseignant ajouté avec succès');
      this.router.navigate(['/teachers']);
    }, error => {
      // console.log(error);
      this.alertify.error(error);
    });
  }
}
