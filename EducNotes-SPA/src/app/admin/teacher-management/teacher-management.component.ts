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
  }

  getTeachers() {
    this.classService.getAllTeachersCourses().subscribe((res: any[]) => {
      this.teachersCourses = res;
    }, error => {
      console.log(error);
    });
  }


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


    cancel(): void {
      this.nzMessageService.info('suppression annulée');
    }

    resultMode(val: boolean) {
      if (val) {
       // reload data;
       this.getTeachers();

      }
      this.show = 'all';
    }
}
