import { Component, OnInit } from '@angular/core';
import { ClassService } from 'src/app/_services/class.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-class-teachers',
  templateUrl: './class-teachers.component.html',
  styleUrls: ['./class-teachers.component.scss']
})
export class ClassTeachersComponent implements OnInit {
  teachers: any;

  constructor(private classService: ClassService, alertify: AlertifyService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.params.subscribe(params => {
      const classId = params['classId'];
      this.getClassTeachers(classId);
    });
  }

  getClassTeachers(classId) {
    this.classService.getClassTeachers(classId).subscribe(teachers => {
      this.teachers = teachers;
    });
  }

}
