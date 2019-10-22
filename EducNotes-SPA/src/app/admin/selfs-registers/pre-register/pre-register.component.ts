import { Component, OnInit } from '@angular/core';
import { AdminService } from 'src/app/_services/admin.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { environment } from 'src/environments/environment';
import { ClassService } from 'src/app/_services/class.service';
import { User } from 'src/app/_models/user';
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
courses;
registerForm: FormGroup;
showNumber = false;
showCourses = false;
submitText = 'enregistrer';
parentTypeId = Number(environment.parentTypeId);
teacherTypeId = Number(environment.teacherTypeId);

  constructor(private adminService: AdminService, private alertify: AlertifyService,
    private authService: AuthService, private classService: ClassService, private fb: FormBuilder) { }

  ngOnInit() {
    this.getUserTypes();
    this.createRegisterForm();
    this.getCourses();
  }

  getUserTypes() {
    this.adminService.getUserTypes().subscribe((res: any[]) => {
      for (let i = 0; i < res.length; i++) {
        const id = Number(res[i].id);
        if (id === this.teacherTypeId  || id === this.parentTypeId) {
          this.userTypes = [...this.userTypes, res[i]];
        }
      }
    });
  }

  getCourses() {
   this.classService.getAllCourses().subscribe((res) => {
        this.courses = res;
      });
    }

  createRegisterForm() {
    this.registerForm = this.fb.group({
      userTypeId: [null, Validators.required],
      courseIds: [null],
      email: ['', [Validators.required, Validators.email]],
       totalChild: [null]});
  }

  submit() {
    this.submitText = 'patienter...';
    this.user = Object.assign({}, this.registerForm.value);
    this.save();
    // this.adminService.emailExist(this.user.email).subscribe((res: boolean) => {
    //   if (res === true) {
    //     this.alertify.error('l\'email ' + this.user.email + ' est déja utlilisé ');
    //     this.submitText = 'enregistrer';

    //   } else {
    //       this.save();
    //     }
    // });

  }
  cancel() {
    this.registerForm.reset();
  }
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
    this.adminService.sendRegisterEmail(this.authService.decodedToken.nameid, this.user).subscribe(() => {
      this.alertify.success('enregistrement terminé');
      this.registerForm.reset();
      this.submitText = 'enregistrer';
    }, error => {
      console.log();
      this.submitText = 'enregistrer';
    });

  }



}
