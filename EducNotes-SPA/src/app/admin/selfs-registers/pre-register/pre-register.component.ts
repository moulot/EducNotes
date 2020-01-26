import { Component, OnInit } from '@angular/core';
import { AdminService } from 'src/app/_services/admin.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { environment } from 'src/environments/environment';
import { ClassService } from 'src/app/_services/class.service';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-pre-register',
  templateUrl: './pre-register.component.html',
  styleUrls: ['./pre-register.component.scss'],
  animations: [SharedAnimations]
})
export class PreRegisterComponent implements OnInit {
  user: any;
  userTypes: any[] = [];
  totalChild: any[] = [];
  courses: any[] = [];
  registerForm: FormGroup;
  showNumber = false;
  showCourses = false;
  submitText = 'enregistrer';
  parentTypeId: number;
  teacherTypeId: number;

  constructor(private adminService: AdminService, private alertify: AlertifyService,
    private authService: AuthService, private classService: ClassService, private fb: FormBuilder) { }

  ngOnInit() {
    this.parentTypeId = environment.parentTypeId;
    this.teacherTypeId = environment.teacherTypeId;

    for (let i = 1; i <= environment.maxChildNumber; i++) {
      const element = { value: i, label: i };
      this.totalChild = [...this.totalChild, element];
    }
    this.getUserTypes();
    this.getCourses();
    this.createRegisterForm();
  }


  getUserTypes() {


    if (this.userTypes.length === 0) {
      this.adminService.getUserTypes().subscribe((res: any[]) => {

        for (let i = 0; i < res.length; i++) {
          const element = { value: res[i].id, label: res[i].name };
          if (Number(element.value) === this.teacherTypeId || Number(element.value) === this.parentTypeId) {
            this.userTypes = [...this.userTypes, element];
          }
        }
      });
    }

  }

  getCourses() {

    if (this.courses.length === 0) {
      this.classService.getAllCourses().subscribe((res: any[]) => {
        for (let i = 0; i < res.length; i++) {
          const element = { value: res[i].id, label: res[i].name };
          this.courses = [...this.courses, element];
        }
      });
    }

  }

  createRegisterForm() {
    this.registerForm = this.fb.group({
      userTypeId: [null, Validators.required],
      courseIds: [null],
      email: ['', [Validators.required, Validators.email]],
      totalChild: [null]
    }, { validator: this.formValidator });
  }

  formValidator(g: FormGroup) {
    const ctrl = g.controls['userTypeId'];
    if (ctrl.valid) {
      if (Number(ctrl.value) === environment.teacherTypeId && !g.get('courseIds').value) {
        return { 'formNOK': true };
      } else if (Number(ctrl.value) === environment.parentTypeId && !g.get('totalChild').value) {
        return { 'formNOK': true };
      } else {
        return false;

      }
    }
    return { 'formNOK': true };
  }


  submit() {
    this.submitText = 'patienter...';
    // this.user = Object.assign({}, this.registerForm.value);
    this.save();
  }
  // cancel() {
  //   this.registerForm.reset();
  // }
  typeCheicking() {
    this.showCourses = false;
    this.showNumber = false;
    const userTypeId = Number(this.registerForm.value.userTypeId);
    if (userTypeId === this.parentTypeId) {
      this.showNumber = true;
    }
    if (userTypeId === this.teacherTypeId) {
      this.showCourses = true;
    }
  }
  save() {
    this.adminService.sendRegisterEmail(this.authService.decodedToken.nameid, this.registerForm.value).subscribe(() => {
      this.alertify.success('enregistrement terminé');
      this.submitText = 'enregistrer';
      this.registerForm.reset();
      this.ngOnInit();
    }, error => {
      this.submitText = 'enregistrer';
    });

  }



}
