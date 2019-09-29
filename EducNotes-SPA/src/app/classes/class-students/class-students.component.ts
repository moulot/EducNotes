import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { debounceTime } from 'rxjs/operators';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { Class } from 'src/app/_models/class';
import { SharedAnimations } from 'src/app/shared/animations/shared-animations';

@Component({
  selector: 'app-class-students',
  templateUrl: './class-students.component.html',
  styleUrls: ['./class-students.component.scss'],
  animations: [SharedAnimations]
})
export class ClassStudentsComponent implements OnInit {
  classRoom: Class;
  searchControl: FormControl = new FormControl();
  viewMode: 'list' | 'grid' = 'grid';
  allSelected: boolean;
  page = 1;
  pageSize = 12;
  students: any[] = [];
  filteredStudents;

  constructor(private classService: ClassService, private alertify: AlertifyService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      const classId = params['classId'];
      this.getClass(classId);
      this.loadStudents(classId);
    });

    this.searchControl.valueChanges.pipe(debounceTime(200)).subscribe(value => {
      this.filerData(value);
    });
  }

  getClass(classId) {
    this.classService.getClass(classId).subscribe((aclass: Class) => {
      this.classRoom = aclass;
    });
  }

  loadStudents(classId) {
    this.classService.getClassStudents(classId).subscribe(data => {
      this.students = data;
      this.filteredStudents = data;
    }, error => {
      this.alertify.error(error);
    });
  }

  loadUser() {

  }

  filerData(val) {
    if (val) {
      val = val.toLowerCase();
    } else {
      return this.students = [...this.students];
    }
    console.log('sess:' + this.students);
    const columns = Object.keys(this.students[0]);
    console.log('col:' + columns + ' - session0:' + this.students[0]);
    if (!columns.length) {
      return;
    }

    const rows = this.students.filter(function(d) {
      console.log('nb cols:' + columns.length);
      for (let i = 0; i <= columns.length; i++) {
        const column = columns[i];
        console.log('coli:' + column + ' - d[col]:' + d[column]);
        if (d[column] && d[column].toString().toLowerCase().indexOf(val) > -1) {
          return true;
        }
      }
    });
    this.filteredStudents = rows;
  }

}
