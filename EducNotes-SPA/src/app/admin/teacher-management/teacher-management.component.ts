import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormControl } from '@angular/forms';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Course } from 'src/app/_models/course';
import { environment } from 'src/environments/environment';
import { AdminService } from 'src/app/_services/admin.service';
import { ActivatedRoute } from '@angular/router';
import * as XLSX from 'xlsx';
import { debounceTime } from 'rxjs/operators';

@Component({
  selector: 'app-teacher-management',
  templateUrl: './teacher-management.component.html',
  styleUrls: ['./teacher-management.component.css']
})
export class TeacherManagementComponent implements OnInit {
  educLevelPrimary = environment.educLevelPrimary;
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
  page = 1;
  pageSize = 10;
  searchControl: FormControl = new FormControl();
  filteredTeachers: any[] = [];
  editionMode = 'add';
  show = 'all';
  drawerTitle: string;
  userId: number;
  visible: boolean;
  affectaionVisible: boolean;
  submitText = 'enregistrer';
  dateOfBirth: string;
  wait = false;

  constructor(private adminService: AdminService, private fb: FormBuilder,
    private alertify: AlertifyService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.teachersCourses = data.teachers;
      this.filteredTeachers = data.teachers;
    });

    this.searchControl.valueChanges.pipe(debounceTime(200)).subscribe(value => {
      this.filerData(value);
    });
  }

  filerData(val) {
    if (val) {
      val = val.toLowerCase();
    } else {
      return this.filteredTeachers = [...this.teachersCourses];
    }
    const columns = Object.keys(this.teachersCourses[0]);
    if (!columns.length) {
      return;
    }

    const rows = this.teachersCourses.filter(function (d) {
      for (let i = 0; i <= columns.length; i++) {
        const column = columns[i];
        if (d[column] && d[column].toString().toLowerCase().indexOf(val) > -1) {
          return true;
        }
      }
    });
    this.filteredTeachers = rows;
  }

}
