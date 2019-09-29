import { Component, OnInit } from '@angular/core';
import { FormGroup,  FormBuilder, Validators } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Course } from 'src/app/_models/course';
import { NzMessageService } from 'ng-zorro-antd';
import { ClassService } from 'src/app/_services/class.service';
import { environment } from 'src/environments/environment';
import { AdminService } from 'src/app/_services/admin.service';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';
import { Router, ActivatedRoute } from '@angular/router';


@Component({
  selector: 'app-teacher-management',
  templateUrl: './teacher-management.component.html',
  styleUrls: ['./teacher-management.component.css'],
  animations :  [SharedAnimations]
})
export class TeacherManagementComponent implements OnInit {
  teachersCourses: any[];
  teacherTypeId = environment.teacherTypeId;
  courses: Course[] = [];
  classes: any[] = [];
  courseId: any;
  classIds: any = [];
  affectatationCourses: Course[];
  teacherForm: FormGroup;
  selectedTeacher: any;
  teacher: any;
  editModel: any;
 editionMode = 'add';
 show = 'all';
 drawerTitle: string;
 userId: number;
 visible: boolean;
 affectaionVisible: boolean;
 submitText = 'enregistrer';

  constructor(private adminService: AdminService, private fb: FormBuilder,
     private classService: ClassService, private router: Router, private alertify: AlertifyService,
     private route: ActivatedRoute,  private nzMessageService: NzMessageService) { }

  ngOnInit() {

    this.route.data.subscribe(data => {
      this.teachersCourses = data.teachers;
   });

    //  this.classService.getAllTeachersCourses().subscribe((data: any[]) => {
    //   this.teachersCourses = data;
    //     }, error => {
    //       console.log(error);
    //     });

    // this.visible = false;
    // this.affectaionVisible = false;
    // this.initializeParams();
    //  this.createTeacherForm();
  }

  // getAllCourses() {
  //   if (this.courses.length === 0) {
  //     this.classService.getAllCourses().subscribe(data => {
  //       this.courses = data;
  //     }, error => {
  //       console.log(error);
  //     });

  //   }
  // }

  // getClasses() {
  //   this.classService.getAllClasses().subscribe(response => {
  //     this.classes = response;
  //   });
  // }
//   initializeParams() {
//     this.editModel = {};
//    this.editModel.lastName = '';
//    this.editModel.dateOfBirth = null;
//    this.editModel.courseIds = [];
//    this.editModel.firstName = '';
//    this.editModel.phoneNumber = '';
//    this.editModel.email = '';
//    this.editModel.secondPhoneNumber = '';
//  }

  // createTeacherForm() {

  //   this.teacherForm = this.fb.group({
  //     dateOfBirth: [this.editModel.dateOfBirth, Validators.required],
  //      lastName: [this.editModel.lastName, Validators.required],
  //     firstName: [this.editModel.firstName, Validators.nullValidator],
  //     courseIds: [this.editModel.courseIds, Validators.nullValidator],
  // phoneNumber: [this.editModel.phoneNumber, Validators.required],
  //     email: [this.editModel.email, [Validators.required, Validators.email]],
  //     secondPhoneNumber: [this.editModel.secondPhoneNumber, Validators.nullValidator]});
  // }


  add() {
    // this.editionMode = 'add';
    // this.teacher = {};
    // this.affectaionVisible = false;
    // this.getAllCourses();
    // this.initializeParams();
    // this.createTeacherForm();
    // this.visible = true;
    // this.drawerTitle = 'NOUVEL ENSEIGNANT';
    this.show = 'add';

  }

  edit( params: any): void {
    let courseIds = [];
    for (let i = 0; i < params.courseClasses.length; i++) {
      const element = params.courseClasses[i].course.id;
      courseIds =  [...courseIds, element];
    }
    params.courseIds = courseIds;
   this.teacher = params;

   this.show = 'edit';
   }


  submit() {
    if (this.teacherForm.valid) {
    this.submitText = 'patienter...';

      if (this.editionMode === 'add') {

        this.teacher = Object.assign({}, this.teacherForm.value);
        this.teacher.userTypeId = this.teacherTypeId;
        this.adminService.emailExist(this.teacher.email).subscribe((res: boolean) => {
          if (res === true) {
            this.alertify.infoBar('l\'email ' + this.teacher.email + 'est déja utlilisé ');
            this.submitText = 'enregistrer';

          } else {
            this.adminService.saveTeacher(this.teacher).subscribe((response: any) => {
              this.visible = false;
            response.type = 'new';
              this.teachersCourses = [...this.teachersCourses, response];
           // this.teachersCourses.unshift(response);
              this.alertify.success('enregistrement terminé...');
             this.submitText = 'enregistrer';
           }, error => {
          this.submitText = 'enregistrer';
          this.alertify.error(error);
            });

          }
        });

      } else {
        const dataFromForm =  Object.assign({}, this.teacherForm.value);
        this.teacher.lastName = dataFromForm.lastName;
        this.teacher.firstName = dataFromForm.firstName;
        this.teacher.dateOfBirth = dataFromForm.dateOfBirth;
        this.teacher.phoneNumber = dataFromForm.phoneNumber;
        this.teacher.secondPhoneNumber = dataFromForm.secondPhoneNumber;
        this.teacher.email = dataFromForm.email;
        this.teacher.courseIds = dataFromForm.courseIds;
        this.classService.updateTeacher(this.userId, this.teacher).subscribe(response => {
          const itemIndex = this.teachersCourses.findIndex(item => item.teacher.id === this.userId);
          Object.assign(this.teachersCourses[ itemIndex ], response);
          this.visible = false;
          this.alertify.success('modification terminée...');
         this.submitText = 'enregistrer';
       }, error => {
      this.submitText = 'enregistrer';
      console.log(error);
      this.alertify.error(error);

        });
      }


    }

  }

  close(): void {
    this.visible = false;
  }

  assignment(params: any) {
    let courses = [];
    let classIds = [];
    for (let i = 0; i < params.courseClasses.length; i++) {
      const cours = params.courseClasses[i].course;
      const classes =  params.courseClasses[i].classes;
      courses =  [...courses, cours];
      for (let j = 0; j < classes.length; j++) {
        const cl = classes[j];
        classIds = [...classIds, cl];

      }
    }
    params.classes = classIds;
    params.courses = courses;
   this.teacher = params;

   this.show = 'affect';
  }

  affecter(params: any) {
    this.drawerTitle = 'AFFECTATION ENSEIGNANT';
    this.selectedTeacher = params;
    this.userId = params.teacher.id;

    // recuperation des cours enseignés par le prof
    this.affectatationCourses = [];
    for (let index = 0; index < this.selectedTeacher.courses.length; index++) {
      const cc: any = {};
     cc.id = this.selectedTeacher.courses[index].course.id;
    cc.name = this.selectedTeacher.courses[index].course.name;
    cc.color = '';
      this.affectatationCourses = [...this.affectatationCourses, cc];
    }
    this.courseId = null;
    this.classIds = [];
      this.affectaionVisible = true;
      this.visible = true;
      // this.getClasses();

  }



  courseChange(): void {
   this.classIds = [];
   // matching selecting course from seleceted Teacher to get classes
    const teachercourses = this.selectedTeacher.courses.find(item => item.course.id === this.courseId);
         for (let index = 0; index < teachercourses.classes.length; index++) {
          if ( teachercourses.classes[index].teacherId === this.userId) {
            this.classIds = [...this.classIds, teachercourses.classes[index].classId];
          }
        }

  }



    affectation() {
      this.classService.saveTeacherAffectation(this.userId,
        this.courseId,
        this.classIds).subscribe(response => {
        const itemIndex = this.teachersCourses.findIndex(item => item.teacher.id === this.userId);
        Object.assign(this.teachersCourses[ itemIndex ], response);
        this.visible = false;
        this.alertify.success('affectation terminée...');
       this.submitText = 'enregistrer';
     }, error => {
          this.submitText = 'enregistrer';
          this.alertify.error(error);

      });
    }

    delete(id: number) {
    this.adminService.deleteTeacher(id).subscribe(() => {
      const itemIndex = this.teachersCourses.findIndex(item => item.teacher.id === id);
      alert(itemIndex);
      this.teachersCourses.splice(itemIndex, 1);
        this.alertify.success('suppression effectuée');
    }, error => {
      this.alertify.error(error);
    });
    }

    cancel(): void {
      this.nzMessageService.info('suppression annulée');
    }

    resultMode(val: boolean) {
      if (val) {
        this.router.navigateByUrl('/TeacherManagementComponent', {skipLocationChange: true}).then(() =>
         this.router.navigate(['/teachers']));
      } else {
        this.show = 'all';
      }
    }
}
