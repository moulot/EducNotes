import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { User } from 'src/app/_models/user';
import { environment } from 'src/environments/environment';
import { City } from 'src/app/_models/city';
import { District } from 'src/app/_models/district';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { AuthService } from 'src/app/_services/auth.service';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Router } from '@angular/router';
import { Utils } from 'src/app/shared/utils';

@Component({
  selector: 'app-new-user',
  templateUrl: './new-user.component.html',
  styleUrls: ['./new-user.component.scss'],
  animations: [SharedAnimations]
})
export class NewUserComponent implements OnInit {
  @Input() user: User;
  @Input() selfRegister: boolean;
  @Output() addUserResult = new EventEmitter();
  teacherMode = false;
  parentMode = false;
  studentMode = false;
  levels: any;
  emailsList: any;
  userNamesList: any;
  cities: City[];
  districts: District[];
  userForm: FormGroup;
  submitText = 'enregister';
  email = '';
  userTypeId: number;
  userId: number;
  phoneMask = [/\d/, /\d/, '-', /\d/, /\d/, '-', /\d/, /\d/, '-', /\d/, /\d/];
  birthDateMask = [/\d/, /\d/, '/', /\d/, /\d/, '/', /\d/, /\d/, /\d/, /\d/];
  sexe = [
    { value: 1, label: 'Homme' },
    { value: 0, label: 'Femme' }
  ];

  userNameExist = false;



  courses: any;
  teacherTypeId = environment.teacherTypeId;
  parentTypeId = environment.parentTypeId;
  studentTypeId = environment.studentTypeId;


  constructor(private fb: FormBuilder, private authService: AuthService, private alertify: AlertifyService,
    private router: Router) { }

  ngOnInit() {

    if (this.user) {
      this.email = this.user.email;
      this.userId = this.user.id;
      this.userTypeId = this.user.userTypeId;
    }

    // récupération des emails existant
    // this.getEmails();
    this.createUserForm();
    // if is an child
    if (this.userTypeId === this.studentTypeId) {
      this.getClassLevels();
      this.studentMode = true;
    }

    if (this.userTypeId === this.parentTypeId) {
      this.getClassLevels();
      this.getCities();
      this.parentMode = true;

    }

    if (this.userTypeId === this.teacherTypeId) {
      // this.getCourses();
      this.teacherMode = true;

    }



  }

  userNameVerification() {
    const userName = this.userForm.value.userName;
    this.userNameExist = false;
    this.authService.userNameExist(userName).subscribe((res: boolean) => {
      if (res === true) {
        this.userNameExist = true;
        // this.user1Form.valid = false;
      }
    });
  }

  getEmails() {
    this.authService.getEmails().subscribe((res) => {
      this.emailsList = res;
    });
  }
  getUserNames() {
    this.authService.getUserNames().subscribe((res) => {
      this.userNamesList = res;
    });
  }

  getClassLevels() {
    this.authService.getLevels().subscribe((res) => {
      this.levels = res;
    });
  }

  // getCourses() {
  //   this.classService.getAllCourses().subscribe((res) => {
  //     this.courses = res;
  //   });
  // }
  getCities() {
    this.authService.getAllCities().subscribe((res: City[]) => {
      this.cities = res;
    });
  }


  getDistricts() {
    // // const id = this.motherForm.value.cityId;
    // // this.motherForm.value.cityId = '';
    // // this.motherDistricts = [];
    // // this.adminService.getDistrictsByCityId(id).subscribe((res: District[]) => {
    // // this.motherDistricts = res;
    // }, error => {
    //   console.log(error);
    // });
  }

  createUserForm() {

    this.userForm = this.fb.group({
      dateOfBirth: ['', Validators.required],
      lastName: ['', Validators.required],
      firstName: ['', Validators.nullValidator],
      userName: ['', Validators.nullValidator],
      password: [null, [Validators.required]],
      checkPassword: [null, [Validators.nullValidator, this.confirmationValidator]],
      gender: [null, Validators.required],
      // levelId: [null, Validators.nullValidator],
      // cityId: [null, Validators.nullValidator],
      // districtId: [null, Validators.nullValidator],
      phoneNumber: [null, Validators.nullValidator],
      // courseIds: [null, Validators.nullValidator],
      email: [this.email, [Validators.nullValidator, Validators.nullValidator, Validators.email]],
      secondPhoneNumber: ['', Validators.nullValidator]
    });
  }

  saveUser() {
    const teacher = Object.assign({}, this.userForm.value);
    console.log('1:' + teacher.dateOfBirth);
    const dd = teacher.dateOfBirth;
    console.log('2:' + dd);
    teacher.dateOfBirth = Utils.inputDateDDMMYY(dd, '/');
    console.log('3:' + teacher.dateOfBirth);
    teacher.userTypeId = this.teacherTypeId;

    this.authService.teacherSelfPreinscription(this.userId, teacher).subscribe(() => {
      this.alertify.success('enregistrement terminé...');
      this.router.navigate(['/parents']);

    }, error => {
      console.log(error);
    });

  }

  close() {

  }

  confirmationValidator = (control: FormControl): { [s: string]: boolean } => {
    if (!control.value) {
      return { required: true };
    } else if (control.value !== this.userForm.controls.password.value) {
      return { confirm: true, error: true };
    }
  }

  updateConfirmValidator(): void {
    /** wait for refresh value */
    Promise.resolve().then(() => this.userForm.controls.checkPassword.updateValueAndValidity());
  }




}
