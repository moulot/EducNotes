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
import * as XLSX from 'xlsx';


@Component({
  selector: 'app-teacher-management',
  templateUrl: './teacher-management.component.html',
  styleUrls: ['./teacher-management.component.css'],
  animations :  [SharedAnimations]
})
export class TeacherManagementComponent implements OnInit {
  teachersCourses: any[];
  exportedTeachers: any[];
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
 dateOfBirth: string;

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



  assignment(params: any) {
    let courses = [];
    let classIds = [];
    for (let i = 0; i < params.courseClasses.length; i++) {
      const cours = params.courseClasses[i].course;
      courses =  [...courses, cours];
      const classes =  params.courseClasses[i].classes;
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


  saveImport() {
    this.adminService.importTeachersFile(this.exportedTeachers).subscribe(() => {
     this.alertify.success('enregistrement terminé...');
     this.getTeachers();
     this.show = 'all';
    }, error => {
      this.alertify.error(error);
    });

  }


    onFileChange(ev) {
      let workBook = null;
      let jsonData = null;
      const reader = new FileReader();
      const file = ev.target.files[0];
      reader.onload = (event) => {
        const data = reader.result;
        workBook = XLSX.read(data, { type: 'binary' });
        jsonData = workBook.SheetNames.reduce((initial, name) => {
          const sheet = workBook.Sheets[name];
          initial[name] = XLSX.utils.sheet_to_json(sheet);
          return initial;
        }, {});

       this.exportedTeachers = [];
        const d = jsonData;
        for (let i = 0; i < d.exel.length; i++) {
            const la_ligne = d.exel[i];
            const element: any = {};
            element.teacherTypeId = this.teacherTypeId,
              element.lastName = la_ligne.nom;
              element.firstName = la_ligne.prenoms,
              element.phoneNumber = la_ligne.contact1,
              element.secondPhoneNumber = la_ligne.contact2,
              element.email = la_ligne.email;
              element.courses = [];
              if (la_ligne.cours1) {
                element.courses = [ ...element.courses, la_ligne.cours1];
              }if (la_ligne.cours2) {
                element.courses = [ ...element.courses, la_ligne.cours2];
              }if (la_ligne.cours3) {
                element.courses = [ ...element.courses, la_ligne.cours3];
              }if (la_ligne.cours4) {
                element.courses = [ ...element.courses, la_ligne.cours4];
              }if (la_ligne.cours5) {
                element.courses = [ ...element.courses, la_ligne.cours5];
              }if (la_ligne.cours6) {
                element.courses = [ ...element.courses, la_ligne.cours6];
              }if (la_ligne.cours7) {
                element.courses = [ ...element.courses, la_ligne.cours7];
              }if (la_ligne.cours8) {
                element.courses = [ ...element.courses, la_ligne.cours8];
              }if (la_ligne.cours9) {
                element.courses = [ ...element.courses, la_ligne.cours9];
              }
              if (la_ligne.cours10) {
                element.courses = [ ...element.courses, la_ligne.cours10];
              }
            this.exportedTeachers = [...this.exportedTeachers, element];
        }
        this.show = 'export';

        // this.setDownload(dataString);
      };
      reader.readAsBinaryString(file);
    }


    setDownload(data) {
     // this.willDownload = true;
      setTimeout(() => {
        const el = document.querySelector('#download');
        el.setAttribute('href', `data:text/json;charset=utf-8,${encodeURIComponent(data)}`);
        el.setAttribute('download', 'xlsxtojson.json');
      }, 1000);
    }
}
